using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    private int BOARD_SIZE;
    private List<Tile> board;
    private Piece.Owner EMPTY = Piece.Owner.NONE;
    private Piece.Owner BLACK;
    private Piece.Owner WHITE;
    private Piece.Owner currentPlayer;
    private bool gameOver;
    private Piece.Owner winner;
    private List<(int, int)> forbiddenMoves;
    private List<GameObject> forbiddenMovesOnMap = new List<GameObject>();

    readonly int[] dx = { 0, 1, 1, 1 };
    readonly int[] dy = { 1, 1, 0, -1 };

    #region forbiddenMoves and win
    public void Init(List<Tile> mapTiles, Piece.Owner firstTurnPlayer)
    {
        // 보드 크기가 8x8라고 가정
        BOARD_SIZE = 8;
        board = mapTiles;
        BLACK = firstTurnPlayer;
        if (firstTurnPlayer == Piece.Owner.PLAYER_A)
        {
            WHITE = Piece.Owner.PLAYER_B;
        }
        else if (firstTurnPlayer == Piece.Owner.PLAYER_B)
        {
            WHITE = Piece.Owner.PLAYER_A;
        }
        gameOver = false;
        winner = EMPTY;
        forbiddenMoves = new List<(int, int)>();
    }

    /// <summary>
    /// 선공만 금수 계산
    /// </summary>
    public void UpdateForbiddenMoves(Piece.Owner currentPlayer)
    {
        if (currentPlayer != BLACK) return;
        forbiddenMoves.Clear();
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                int index = y * BOARD_SIZE + x;
                if (board[index].Piece == null && IsForbiddenMove(x, y))
                {
                    forbiddenMoves.Add((x, y));
                }
            }
        }
        if (forbiddenMoves.Count > 0)
        {
            ForviddensOnMap();
        }
    }

    private void ForviddensOnMap()
    {
        foreach (var move in forbiddenMoves)
        {
            int x = move.Item1;
            int y = move.Item2;
            int index = y * BOARD_SIZE + x;
            board[index].isForbiddenMove = true;
            // Instantiate forbidden marker as child of 해당 타일
            forbiddenMovesOnMap.Add(Instantiate(GameManager.Instance.forbiddenMoveObject, board[index].transform));
        }
    }

    public void DeleteForviddensOnMap()
    {
        foreach (var move in forbiddenMoves)
        {
            int x = move.Item1;
            int y = move.Item2;
            int index = y * BOARD_SIZE + x;
            board[index].isForbiddenMove = false;
        }
        if (forbiddenMovesOnMap != null)
        {
            foreach (var forbidden in forbiddenMovesOnMap)
            {
                Destroy(forbidden);
            }
            forbiddenMovesOnMap.Clear();
        }
    }

    /// <summary>
    /// 금수 체크 (임시 돌을 놓은 후 해당 위치가 금수라면 true)
    /// </summary>
    private bool IsForbiddenMove(int x, int y)
    {
        int index = y * BOARD_SIZE + x;
        if (board[index].Piece != null)
            return false;
        else
        {
            // 임시로 흑돌 놓기
            board[index].SetPiece(GameManager.Instance.SetTemporaryPiece(index, BLACK));

            bool isOver = IsOverline(x, y);
            bool isDoubleFour = IsDoubleFour(x, y);
            bool isDoubleThree = IsDoubleThree(x, y);
            bool canWin = CheckWinFORForbidden(x, y,BLACK);
            bool isForbidden = (isOver || isDoubleFour || isDoubleThree) && (!canWin);

            // 임시 돌 제거
            if (board[index].Piece != null)
            {
                Destroy(board[index].Piece.gameObject);
            }
            board[index].SetPiece(null);

            return isForbidden;
        }
    }

    private bool IsInBoard(int x, int y)
    {
        return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
    }

    private int CountStonesInDirection(int x, int y, int dx, int dy,Piece.Owner owner)
    {
        int count = 0;
        int nx = x + dx;
        int ny = y + dy;
        while (IsInBoard(nx, ny))
        {
            int idx = ny * BOARD_SIZE + nx;
            if (board[idx].Piece?.pieceOwner == owner)
            {
                count++;
                nx += dx;
                ny += dy;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    private bool IsOverline(int x, int y)
    {
        for (int dir = 0; dir < 4; dir++)
        {
            int _count = 1;
            _count += CountStonesInDirection(x, y, dx[dir], dy[dir],BLACK);
            _count += CountStonesInDirection(x, y, -dx[dir], -dy[dir], BLACK);
            if (_count >= 6)
                return true;
        }
        return false;
    }

    private bool IsDoubleFour(int x, int y)
    {
        int fourCount = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            if (HasFour(x, y, dx[dir], dy[dir]))
                fourCount++;
            if (fourCount >= 2)
                return true;
        }
        return false;
    }

    private bool HasFour(int x, int y, int dx, int dy)
    {
        int index = y * BOARD_SIZE + x;
        Piece originalValue = board[index].Piece;
        board[index].SetPiece(null);

        for (int i = -4; i <= 0; i++)
        {
            bool valid = true;
            int blackCount = 0;
            int emptyCount = 0;

            for (int j = 0; j < 5; j++)
            {
                int nx = x + (i + j) * dx;
                int ny = y + (i + j) * dy;
                if (!IsInBoard(nx, ny))
                {
                    valid = false;
                    break;
                }
                int idx2 = ny * BOARD_SIZE + nx;
                if (nx == x && ny == y)
                {
                    blackCount++;
                }
                else if (board[idx2].Piece != null)
                {
                    if (board[idx2].Piece.pieceOwner == BLACK)
                        blackCount++;
                }
                else
                {
                    emptyCount++;
                }
            }

            if (valid && blackCount == 4 && emptyCount == 1)
            {
                board[index].SetPiece(originalValue);
                return true;
            }
        }
        board[index].SetPiece(originalValue);
        return false;
    }

    private bool IsDoubleThree(int x, int y)
    {
        int openThreeCount = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            if (HasOpenThree(x, y, dx[dir], dy[dir]))
                openThreeCount++;
            if (openThreeCount >= 2)
                return true;
        }
        return false;
    }

    private bool HasOpenThree(int x, int y, int dx, int dy)
    {
        int index = y * BOARD_SIZE + x;
        Piece originalValue = board[index].Piece;
        board[index].SetPiece(null);

        bool isOpenThree = CheckOpenThreePattern(x, y, dx, dy, "_OOO_") ||
                             CheckOpenThreePattern(x, y, dx, dy, "_OO_O_") ||
                             CheckOpenThreePattern(x, y, dx, dy, "_O_OO_");

        board[index].SetPiece(originalValue);
        return isOpenThree;
    }

    private bool CheckOpenThreePattern(int x, int y, int dx, int dy, string pattern)
    {
        int patternLength = pattern.Length;
        for (int start = -(patternLength - 1); start <= 0; start++)
        {
            bool valid = true;
            for (int i = 0; i < patternLength; i++)
            {
                int nx = x + (start + i) * dx;
                int ny = y + (start + i) * dy;
                if (!IsInBoard(nx, ny))
                {
                    valid = false;
                    break;
                }
                int idx2 = ny * BOARD_SIZE + nx;
                char expected = pattern[i];
                if (expected == '_')
                {
                    if (board[idx2].Piece != null)
                    {
                        valid = false;
                        break;
                    }
                }
                else if (expected == 'O')
                {
                    if (!(nx == x && ny == y) && board[idx2].Piece?.pieceOwner != BLACK)
                    {
                        valid = false;
                        break;
                    }
                }
            }
            if (valid)
                return true;
        }
        return false;
    }

    public (bool, Piece.Owner) CheckGameOver()
    {
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                int index = y * BOARD_SIZE + x;
                if (board[index].Piece != null)
                {
                    if (CheckFiveInARow(x, y))
                    {
                        gameOver = true;
                        winner = board[index].Piece.pieceOwner;
                        return (true, winner);
                    }
                }
            }
        }
        return (false, EMPTY);
    }

    private bool CheckFiveInARow(int x, int y)
    {
        int index = y * BOARD_SIZE + x;
        Piece.Owner stone = board[index].Piece.pieceOwner;
        for (int dir = 0; dir < 4; dir++)
        {
            int count = 1;
            bool continuousLine = true;
            for (int i = 1; i < 5; i++)
            {
                int nx = x + dx[dir] * i;
                int ny = y + dy[dir] * i;
                if (!IsInBoard(nx, ny) || board[ny * BOARD_SIZE + nx].Piece?.pieceOwner != stone)
                {
                    continuousLine = false;
                    break;
                }
                count++;
            }
            if (continuousLine && count == 5)
                return true;
        }
        return false;
    }

    public Piece.Owner NotFinishedOnPlayingGame()
    {
        int APoint = 0;
        int BPoint = 0;
        foreach (var tile in board)
        {
            if (tile.Piece?.pieceOwner == Piece.Owner.PLAYER_A)
                APoint += tile.Piece.cost;
            else if (tile.Piece?.pieceOwner == Piece.Owner.PLAYER_B)
                BPoint += tile.Piece.cost;
        }
        
        Debug.Log("A:"+APoint +"점, B:"+BPoint+ "점");

        if (APoint > BPoint)
        {
            gameOver = true;
            winner = Piece.Owner.PLAYER_A;
            return winner;
        }
        else if (APoint < BPoint)
        {
            gameOver = true;
            winner = Piece.Owner.PLAYER_B;
            return winner;
        }
        else
        {
            return Piece.Owner.NONE;
        }
    }

    private bool CheckWinFORForbidden(int x, int y,Piece.Owner owner)
    {
        int index = y * BOARD_SIZE + x;
        Piece stone = board[index].Piece;
        for (int dir = 0; dir < 4; dir++)
        {
            int _count = 1;
            _count += CountStonesInDirection(x, y, dx[dir], dy[dir],owner);
            _count += CountStonesInDirection(x, y, -dx[dir], -dy[dir], owner);
            if (stone?.pieceOwner == owner && _count == 5)
                return true;
        }
        return false;
    }

    private bool CheckWinFORSetPiece(int x, int y, Piece.Owner owner)
    {
        int index = y * BOARD_SIZE + x;
        for (int dir = 0; dir < 4; dir++)
        {
            int _count = 1;
            _count += CountStonesInDirection(x, y, dx[dir], dy[dir], owner);
            _count += CountStonesInDirection(x, y, -dx[dir], -dy[dir], owner);
            if (_count == 5)
                return true;
        }
        return false;
    }
    #endregion



    #region SetPieceAI
    // FindOptimalMove 와 평가 함수 (오목에서 최적의 수를 찾음)
    public (int, int) FindOptimalMove(Piece.Owner currentPlayer)
    {
        List<(int, int, double)> candidateMoves = new List<(int, int, double)>();

        // 모든 빈 타일 순회
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                int index = y * BOARD_SIZE + x;
                if (board[index].Piece != null)
                    continue;
                // 흑인 경우 금수 건너뛰기
                if (BLACK == currentPlayer && IsForbiddenMove(x, y))
                    continue;

                double moveScore = EvaluateMove(x, y, currentPlayer);
                candidateMoves.Add((x, y, moveScore));
            }
        }

        if (board.All(tile => tile.Piece == null)) {
            System.Random random = new System.Random();
            int x = random.Next(BOARD_SIZE / 2 - 2, BOARD_SIZE / 2 + 2);
            int y = random.Next(BOARD_SIZE / 2 - 2, BOARD_SIZE / 2 + 2);
            return (x, y);
        }

        candidateMoves.Sort((a, b) => b.Item3.CompareTo(a.Item3));
        return (candidateMoves[0].Item1, candidateMoves[0].Item2);
    }

    private double EvaluateMove(int x, int y, Piece.Owner currentPlayer)
    {
        double score = 0;
        int index = y * BOARD_SIZE + x;
        try
        {
            // 1. 승리 가능성
            if (CheckWinFORSetPiece(x, y,currentPlayer)) score += 10000; // 승리하면 최고 점수

            // 2. 상대 승리 차단 가능성 (여기서 3개 이상의 위협부터 고려)
            score += EvaluateBlockingPotential(x, y, currentPlayer);


            // 임시 돌 놓기
            board[index].SetPiece(GameManager.Instance.SetTemporaryPiece(index, currentPlayer));

            // 3. 공격적 기회
            score += EvaluateOffensivePotential(x, y, currentPlayer);

            // 4. 위치적 이점 (중앙 제어)
            score += EvaluatePositionalAdvantage(x, y, BOARD_SIZE);
        }
        finally
        {
            if (board[index].Piece != null)
            {
                Destroy(board[index].Piece.gameObject);
            }
            board[index].SetPiece(null);
        }

        return score;
    }

    // 상대의 위협(threat)이 3개 이상 연속될 경우 차단 점수를 부여하도록 수정
    private double EvaluateBlockingPotential(int x, int y, Piece.Owner currentPlayer)
    {
        Piece.Owner opponent = (currentPlayer == Piece.Owner.PLAYER_A) ? Piece.Owner.PLAYER_B : Piece.Owner.PLAYER_A;

        double blockScore = 0;
        int index = y * BOARD_SIZE + x;
        board[index].SetPiece(GameManager.Instance.SetTemporaryPiece(index, opponent));
        try
        {
            int maxConsecutive = 0;
            for (int dir = 0; dir < 4; dir++)
            {
                int consecutiveStones = 1;
                consecutiveStones += CountStonesInDirection(x, y,  dx[dir], dy[dir], opponent);
                consecutiveStones += CountStonesInDirection(x, y,  -dx[dir], -dy[dir], opponent);
                if (consecutiveStones > maxConsecutive)
                    maxConsecutive = consecutiveStones;
            }
            // 만약 4개 이상의 연속 돌이 있다면 블록 필요 (점수는 위협 정도에 따라 조정)
            if (maxConsecutive > 3)
            {
                if (maxConsecutive == 4)
                    blockScore += 1410;
                else if (maxConsecutive >= 5)
                    blockScore += 4200;
            }
        }
        finally
        {
            Destroy(board[index].Piece.gameObject);
            board[index].SetPiece(null);
        }
        return blockScore;
    }

    private double EvaluateOffensivePotential(int x, int y, Piece.Owner currentPlayer)
    {
        double offenseScore = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            int consecutiveStones = 1;
            int limitcount = 0;
            consecutiveStones += CountStonesInDirection(x, y, dx[dir], dy[dir], currentPlayer);
            consecutiveStones += CountStonesInDirection(x, y, -dx[dir], -dy[dir], currentPlayer);
            if (limitcount < 2) {

                switch (consecutiveStones)
                {
                    case 2: offenseScore += 200; { limitcount++; break; }
                    case 3: offenseScore += 700; { limitcount++; break; }
                    case 4: offenseScore += 2000; { limitcount++; break; }
                }
            }
        }
        return offenseScore;
    }

    private double EvaluatePositionalAdvantage(int x, int y, int boardSize)
    {
        int centerX = boardSize / 2;
        int centerY = boardSize / 2;
        double distanceFromCenter = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
        return 1 / (distanceFromCenter + 1);
    }
    #endregion


    #region AttackAI
    public List<int> FindPiecesWithAttackRange(Piece.Owner currentPlayer)
    {
        List<(int index, int cost, int attackableOpponentCount)> attackablePieces = new List<(int, int, int)>();

        // 모든 보드를 순회
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                int index = y * BOARD_SIZE + x;
                Tile currentTile = board[index];

                // 현재 플레이어의 Piece인지 확인
                if (currentTile.Piece?.pieceOwner == currentPlayer)
                {
                    // 공격 가능 범위 계산
                    List<int> attackRange = GameManager.Instance.CanAttackRangeCalculate(index, currentTile.Piece.GetAttackRange());

                    // 공격 범위 내 적 Piece 수 계산
                    int opponentCount = CountOpponentPiecesInRange(attackRange, currentPlayer);

                    // 적 Piece가 있으면 리스트에 추가
                    if (opponentCount > 0)
                    {
                        attackablePieces.Add((index, currentTile.Piece.cost, opponentCount));
                    }
                }
            }
        }

        // 정렬 조건: 
        // 1. 기본적으로 Piece의 cost가 높은 순
        // 2. cost가 같다면 공격 가능한 적 Piece 수가 많은 순
        attackablePieces.Sort((a, b) =>
        {
            int costComparison = b.cost.CompareTo(a.cost);
            if (costComparison != 0) return costComparison;
            return b.attackableOpponentCount.CompareTo(a.attackableOpponentCount);
        });

        // 인덱스만 반환
        return attackablePieces.Select(x => x.index).ToList();
    }

    private int CountOpponentPiecesInRange(List<int> attackRange, Piece.Owner currentPlayer)
    {
        Piece.Owner opponentPlayer = (currentPlayer == Piece.Owner.PLAYER_A) ? Piece.Owner.PLAYER_B : Piece.Owner.PLAYER_A;
        int opponentCount = 0;

        foreach (int rangeIndex in attackRange)
        {
            if (board[rangeIndex].Piece?.pieceOwner == opponentPlayer)
            {
                opponentCount++;
            }
        }

        return opponentCount;
    }

    /// <summary>
    /// 아군 Piece 인덱스 가져오기 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="currentPlayer"></param>
    /// <returns></returns>
    public int FindWeakestConsecutiveAllyInRange(int index, Piece.Owner currentPlayer)
    {
        // 공격 가능 범위 계산 (현재 말의 공격 범위)
        Piece piece = board[index].Piece;
        List<int> attackRange = GameManager.Instance.CanAttackRangeCalculate(index, piece.GetAttackRange());


        for (int dir = 0; dir < 4; dir++)
        {
            List<int> consecutiveAllyIndices = FindConsecutiveAlliesInDirection(index, currentPlayer, dx[dir], dy[dir], attackRange);

            if (consecutiveAllyIndices?.Count > 0)
            {
                // 가장 낮은 cost의 아군 돌 찾기
                int weakestAllyIndex = consecutiveAllyIndices.OrderBy(idx => board[idx].Piece.cost).First();
                return weakestAllyIndex;
            }
        }

        // 연속된 아군 돌이 없으면 범위 내 아군 중 가장 낮은 cost의 돌 찾기
        List<int> allAlliesInRange = attackRange
            .Where(idx => board[idx].Piece?.pieceOwner == currentPlayer)
            .ToList();

        if (allAlliesInRange?.Count > 0)
        {
            // 범위 내 아군 중 가장 낮은 cost의 돌 반환
            return allAlliesInRange.OrderBy(idx => board[idx].Piece.cost).First();
        }

        // 아군 돌이 아예 없으면 -1 반환
        return -1;
    }

    private List<int> FindConsecutiveAlliesInDirection(int startIndex, Piece.Owner currentPlayer, int dx, int dy, List<int> attackRange)
    {
        List<int> consecutiveAllyIndices = new List<int>();
        int width = 8; // 보드 크기에 맞게 조정 필요
        int startY = startIndex / width;
        int startX = startIndex % width;

        // 시작 방향으로 연속된 아군 돌 찾기
        for (int step = 1; step <= 2; step++) // 최대 2칸 연속 확인
        {
            int nx = startX + dx * step;
            int ny = startY + dy * step;
            int nextIndex = ny * width + nx;

            // 공격 가능 범위 내이고 아군 돌인지 확인
            if (attackRange.Contains(nextIndex) &&
                IsInBoard(nx, ny) &&
                board[nextIndex].Piece?.pieceOwner == currentPlayer)
            {
                consecutiveAllyIndices.Add(nextIndex);
            }
            else
            {
                break;
            }
        }

        return consecutiveAllyIndices;
    }


    public int FindBestAttackTarget(int index, Piece.Owner currentPlayer)
    {
        // 공격 가능 범위 계산 (현재 말의 공격 범위)
        Piece piece = board[index].Piece;
        List<int> attackRange = GameManager.Instance.CanAttackRangeCalculate(index, piece.GetAttackRange());
        Piece.Owner opponentPlayer = (currentPlayer == Piece.Owner.PLAYER_A) ? Piece.Owner.PLAYER_B : Piece.Owner.PLAYER_A;

        // 연속된 적 말 찾기
        for (int dir = 0; dir < 4; dir++)
        {
            List<int> consecutiveEnemyIndices = FindConsecutiveEnemiesInDirection(index, opponentPlayer, dx[dir], dy[dir], attackRange);

            if (consecutiveEnemyIndices.Count > 0)
            {
                // 연속된 적 말 중 체력이 가장 적고 cost가 높은 순으로 정렬
                return consecutiveEnemyIndices
                .OrderBy(idx => board[idx].Piece.hp)
                .ThenBy(idx => board[idx].Piece.cost)
                .ThenByDescending(idx => board[idx].Piece.cost)
                .First();
            }
        }

        // 연속된 적 말이 없는 경우, 공격 가능 범위 내 적 말 중 체력이 가장 적고 cost가 높은 순으로 선택
        List<int> allEnemiesInRange = attackRange
            .Where(idx => board[idx].Piece?.pieceOwner == opponentPlayer)
            .ToList();

        if (allEnemiesInRange.Count > 0)
        {
            return allEnemiesInRange
                .OrderBy(idx => board[idx].Piece.cost)
                .ThenByDescending(idx => board[idx].Piece.cost)
                .First();
        }

        // 적 말이 없으면 -1 반환
        return -1;
    }

    private List<int> FindConsecutiveEnemiesInDirection(int startIndex, Piece.Owner opponentPlayer, int dx, int dy, List<int> attackRange)
    {
        List<int> consecutiveEnemyIndices = new List<int>();
        int startY = startIndex / BOARD_SIZE;
        int startX = startIndex % BOARD_SIZE;

        // 시작 방향으로 연속된 적 말 찾기
        for (int step = 1; step <= 2; step++) // 최대 2칸 연속 확인
        {
            int nx = startX + dx * step;
            int ny = startY + dy * step;
            int nextIndex = ny * BOARD_SIZE + nx;

            // 공격 가능 범위 내이고 적 말인지 확인
            if (attackRange.Contains(nextIndex) &&
                IsInBoard(nx, ny) &&
                board[nextIndex].Piece?.pieceOwner == opponentPlayer)
            {
                consecutiveEnemyIndices.Add(nextIndex);
            }
            else
            {
                break;
            }
        }
        return consecutiveEnemyIndices;
    }
    #endregion
}
