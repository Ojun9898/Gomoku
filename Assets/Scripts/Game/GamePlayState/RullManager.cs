using System.Collections.Generic;
using UnityEngine;
public class RullManager : MonoBehaviour
{
    private int BOARD_SIZE;
    private List<Tile> board;
    private Piece.Owner EMPTY = Piece.Owner.NONE;
    private Piece.Owner BLACK ;
    private Piece.Owner WHITE ;
    private Piece.Owner currentPlayer;
    private bool gameOver;
    private Piece.Owner winner;
    private List<(int, int)> forbiddenMoves;
    private List<GameObject> forbiddenMovesOnMap = new List<GameObject>();

    readonly int[] dx = { 0, 1, 1, 1 };
    readonly int[] dy = { 1, 1, 0, -1 };


    public void Init(List<Tile> mapTiles,  Piece.Owner firstTurnPlayer)
    {
        BOARD_SIZE = 8;
        board = mapTiles;
        BLACK = firstTurnPlayer;
        if (firstTurnPlayer == Piece.Owner.PLAYER_A) { 
           WHITE = Piece.Owner.PLAYER_B;
        }else if(firstTurnPlayer == Piece.Owner.PLAYER_B)
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
        if (currentPlayer == BLACK)
        {
            forbiddenMoves.Clear();
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    int index = y * BOARD_SIZE + x; // 1차원 인덱스 계산
                    if (board[index].Piece == null && IsForbiddenMove(x, y))
                    {
                        forbiddenMoves.Add((x, y));
                    }
                }
            }
            if(forbiddenMoves != null)
            {
                ForviddensOnMap();
            }
        }
    }


    private void ForviddensOnMap()
    {
        foreach (var move in forbiddenMoves)
        {
            int x = move.Item1;
            int y = move.Item2;

            // 1D 인덱스 계산 (여기서 BOARD_SIZE는 예시로 8, 8x8 보드인 경우)
            int index = y * BOARD_SIZE + x;

            board[index].isForbiddenMove = true;
            forbiddenMovesOnMap.Add(Instantiate(GameManager.Instance.forbiddenMoveObject, board[index].transform));
        }
    }

    public void DeleteForviddensOnMap() {
        foreach (var move in forbiddenMoves)
        {
            int x = move.Item1;
            int y = move.Item2;
            // 1D 인덱스 계산 (여기서 BOARD_SIZE는 예시로 8, 8x8 보드인 경우)
            int index = y * BOARD_SIZE + x;
            board[index].isForbiddenMove = false;
        }
        if (forbiddenMovesOnMap != null) {
            foreach (var forbidden in forbiddenMovesOnMap)
            {
                Destroy(forbidden);
            }
        }
    }





    /// <summary>
    /// 금수 체크
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>해당 위치의 금수 유무</returns>
    private bool IsForbiddenMove(int x, int y)
    {
        int index = y * BOARD_SIZE + x; // 1차원 인덱스 계산
        // 이미 돌이 있는 경우
        if (board[index].Piece != null)
            return false;
        else { 
            // 임시로 흑돌 놓기
            board[index].SetPiece(GameManager.Instance.SetTemporaryPiece(index, BLACK));

            // 장목(6목 이상), 4-4, 3-3 체크
            var a = IsOverline(x, y);
            var b = IsDoubleFour(x, y);
            var c = IsDoubleThree(x, y);
            var d =  CheckWin(x, y);
            bool isForbidden = (a||b||c)&&(!d);


            // 임시 돌 제거
            Destroy(board[index].Piece.gameObject);
            board[index].SetPiece(null);

            return isForbidden;
        }
    }




    /// <summary>
    /// 게임판 경계를 넘지 않았나 확인
    /// </summary>
    /// <param name="x">확인하려는 오브젝트의 x좌표</param>
    /// <param name="y">확인하려는 오브젝트의 y좌표</param>
    /// <returns></returns>
    private bool IsInBoard(int x, int y)
    {
        return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
    }





    /// <summary>
    /// 연속된 돌의 개수 세기
    /// </summary>
    /// <param name="x">시작 X 좌표</param>
    /// <param name="y">시작 y 좌표</param>
    /// <param name="dx">x좌표 진행 방향</param>
    /// <param name="dy">y좌표 진행 방향</param>
    /// <returns>해당 방향으로 연속돈 돌의 개수</returns>
    private int CountStonesInDirection(int x, int y, int dx, int dy)
    {
        int count = 0;
        int nx = x + dx;
        int ny = y + dy;

        while (IsInBoard(nx, ny))
        {
            int index = ny * BOARD_SIZE + nx; // 1차원 인덱스 계산
            if (board[index].Piece?.pieceOwner == BLACK)
            {
                count++;
                nx += dx;
                ny += dy;
            }
            else {
                break;
            }
        }
        
        return count;
    }




    /// <summary>
    /// 장목 체크 (6목 이상)
    /// </summary>
    /// <param name="x">체크 x좌표</param>
    /// <param name="y">체크 y좌표</param>
    /// <returns>해당  좌표 장목 금수의 유무</returns>
    private bool IsOverline(int x, int y)
    {

        for (int dir = 0; dir < 4; dir++)
        {
            int _count = 1;

            // 양방향으로 같은 색 돌 세기
            _count += CountStonesInDirection(x, y, dx[dir], dy[dir]);
            _count += CountStonesInDirection(x, y, -dx[dir], -dy[dir]);

            if (_count >= 6)
                return true;
        }

        return false;
    }




    /// <summary>
    /// 44 금수 체크 
    /// </summary>
    /// <param name="x">체크 x좌표</param>
    /// <param name="y">체크 y좌표</param>
    /// <returns>해당  좌표 44 금수의 유무</returns>
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



    /// <summary>
    /// 연속된 4개의 돌 체크
    /// 체크하는 곳 기준  -4 만큼의 크기를 잡아 그 안에서 4가  만들어 질 수 있는 경우를 모두 확인함
    /// </summary>
    /// <param name="x">체크할x좌표</param>
    /// <param name="y">체크할y좌표</param>
    /// <param name="dx">진행 방향</param>
    /// <param name="dy">진행 방향</param>
    /// <returns>해당 방향으로 빈칸 1개와  흑돌 4개가 있는지 유무</returns>
    private bool HasFour(int x, int y, int dx, int dy)
    {
        int index = y * BOARD_SIZE + x;
        // 돌을 놓기 전 상태로 되돌림
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
                int index2 = ny * BOARD_SIZE + nx;

                if (!IsInBoard(nx, ny))
                {
                    valid = false;
                    break;
                }

                if (nx == x && ny == y)
                {
                    blackCount++;
                }
                else if (board[index2].Piece != null)
                {
                    if (board[index2].Piece.pieceOwner == BLACK) { 
                    
                        blackCount++;
                    }
                }
                else if (board[index2].Piece == null)
                {
                    emptyCount++;
                }
                else
                {
                    valid = false;
                    break;
                }
            }

            // 돌 복원
            board[index].SetPiece(originalValue);

            // 4는 흑돌 4개와 빈칸 1개로 구성
            if (valid && blackCount == 4 && emptyCount == 1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 33 금수 체크
    /// </summary>
    /// <param name="x">체크 x좌표</param>
    /// <param name="y">체크 y좌표</param>
    /// <returns>해당  좌표 33 금수의 유무</returns>
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


    /// <summary>
    /// 열린 3 체크
    /// </summary>
    /// <param name="x">체크할x좌표</param>
    /// <param name="y">체크할y좌표</param>
    /// <param name="dx">진행 방향</param>
    /// <param name="dy">진행 방향</param>
    /// <returns>해당 방향으로 열린 3이 만들어지는지 유무</returns>
    private bool HasOpenThree(int x, int y, int dx, int dy)
    {
        int index = y * BOARD_SIZE + x;
        // 돌을 놓기 전 상태로 되돌림
        Piece originalValue = board[index].Piece;
        board[index].SetPiece(null);

        // 열린 3 패턴 예시: _OOO_, _OO_O_, _O_OO_
        bool isOpenThree = CheckOpenThreePattern(x, y, dx, dy, "_OOO_") ||
                          CheckOpenThreePattern(x, y, dx, dy, "_OO_O_") ||
                          CheckOpenThreePattern(x, y, dx, dy, "_O_OO_");

        // 돌 복원
        board[index].SetPiece(originalValue);

        return isOpenThree;
    }

    /// <summary>
    /// 열린 3 패턴 체크
    /// </summary>
    /// <param name="x">체크할x좌표</param>
    /// <param name="y">체크할y좌표</param>
    /// <param name="dx">진행 방향</param>
    /// <param name="dy">진행 방향</param>
    /// <param name="pattern">3 패턴</param>
    /// <returns>패턴에 맞는 경우의 유무</returns>
    private bool CheckOpenThreePattern(int x, int y, int dx, int dy, string pattern)
    {
        int patternLength = pattern.Length;
      

        // 패턴의 길이에 따라 시작 위치 조정
        for (int start = -(patternLength - 1); start <= 0; start++)
        {
            bool valid = true;

            for (int i = 0; i < patternLength; i++)
            {
                int nx = x + (start + i) * dx;
                int ny = y + (start + i) * dy;

                int index2 = ny * BOARD_SIZE + nx;
                if (!IsInBoard(nx, ny))
                {
                    valid = false;
                    break;
                }

                char expected = pattern[i];

                if (expected == '_')
                {
                    if (board[index2].Piece != null)
                    {
                        valid = false;
                        break;
                    }
                }
                else if (expected == 'O')
                {
                    if (nx == x && ny == y)
                    {
                        // 현재 위치는 흑돌로 간주
                    }
                    else if (board[index2].Piece?.pieceOwner != BLACK)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 게임판 위에 오목이 있는지 체크
    /// </summary>
    /// <returns>우승자의 타입을 반환,None이면 게속 진행</returns>
    public (bool,Piece.Owner) CheckGameOver()
    {
      
        // 승리 조건: 5개 연속
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
                        return (true,winner);
                    }
                }
            }
        }
        //결판이 나지 않았을 때
        return (false,EMPTY);
    }


    // 5개 연속 돌 확인 (새로운 함수)
    private bool CheckFiveInARow(int x, int y)
    {
        int index = y * BOARD_SIZE + x;
        Piece.Owner stone = Piece.Owner.NONE;
        if (board[index].Piece != null) { 
            stone = board[index].Piece.pieceOwner;
        }
        for (int dir = 0; dir < 4; dir++)
        {
            int count = 1; // 현재 위치의 돌 포함

            // 한쪽 방향으로 같은 색 돌 세기
            for (int i = 1; i < 5; i++)
            {
                int nx = x + dx[dir] * i;
                int ny = y + dy[dir] * i;

                int index2 = ny * BOARD_SIZE + nx;
                if (IsInBoard(nx, ny)) {
                    if (board[index2].Piece?.pieceOwner == stone) { 
                        count++;
                    }
                }
                else
                    break;
            }

            // 반대 방향으로 같은 색 돌 세기
            for (int i = 1; i < 5; i++)
            {
                int nx = x - dx[dir] * i;
                int ny = y - dy[dir] * i;

                int index2 = ny * BOARD_SIZE + nx;
                if (IsInBoard(nx, ny))
                {
                    if (board[index2].Piece?.pieceOwner == stone)
                    {
                        count++;
                    }
                }
                else
                    break;
            }

            // 흑돌은 정확히 5개, 백돌은 5개 이상일 때 승리
            if ((stone == BLACK && count == 5) || (stone == WHITE && count >= 5))
                return true;
        }
        return false;
    }



    /// <summary>
    /// 양측 모든 말을 다 낸 후 게임이 끝나지 않았다면  점수 합계 후 우승자 가리기
    /// </summary>
    /// <returns>우승자의 타입을 보냄 , 무승부시 None</returns>
    public Piece.Owner NotFinishedOnPlayingGame() {
      
            int APoint = 0;
            int BPoint = 0;
            foreach (var tile in board)
            {
                if (tile.Piece?.pieceOwner == Piece.Owner.PLAYER_A)
                {
                    APoint += tile.Piece.cost;
                }
                else if (tile.Piece?.pieceOwner == Piece.Owner.PLAYER_B)
                {
                    BPoint += tile.Piece.cost;
                }
            }

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
        else { 
            return Piece.Owner.NONE;
        }
        
    }


    /// <summary>
    /// 예측수를 뒀을 때 흑돌이 이길 수 있는지 확인하는 메소드
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CheckWin(int x, int y)
    {

        int index = y * BOARD_SIZE + x;
        Piece stone = board[index].Piece;
        int[] dx = { 0, 1, 1, 1 };
        int[] dy = { 1, 1, 0, -1 };

        for (int dir = 0; dir < 4; dir++)
        {
            int _count = 1; // 현재 위치의 돌 포함

            // 양방향으로 같은 색 돌 세기
            _count += CountStonesInDirection(x, y, dx[dir], dy[dir]);
            _count += CountStonesInDirection(x, y, -dx[dir], -dy[dir]);

            
            if (stone?.pieceOwner == BLACK && _count == 5)
                return true;
        }

        return false;
    }
}
