using System;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
   
    private const int BoardSize = 15;
    private const int MaxDepth = 3; // 탐색 깊이
    private int[,] board; // 0: 빈 칸, 1: 흑돌, 2: 백돌

    public AI(int[,] boardState)
    {
        board = boardState;
    }

    public (int, int) GetBestMove(int player)
    {
        int bestScore = int.MinValue;
        (int, int) bestMove = (-1, -1);

        foreach (var (x, y) in GetAvailableMoves())
        {
            board[x, y] = player;
            int score = MinMax(0, false, int.MinValue, int.MaxValue, player);
            board[x, y] = 0; // 원상 복구

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (x, y);
            }
        }
        return bestMove;
    }

    private int MinMax(int depth, bool isMaximizing, int alpha, int beta, int player)
    {
        if (depth == MaxDepth || IsGameOver())
        {
            return EvaluateBoard(player);
        }

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        int opponent = player == 1 ? 2 : 1;

        foreach (var (x, y) in GetAvailableMoves())
        {
            board[x, y] = isMaximizing ? player : opponent;
            int score = MinMax(depth + 1, !isMaximizing, alpha, beta, player);
            board[x, y] = 0; // 원상 복구

            if (isMaximizing)
            {
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);
            }
            else
            {
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, score);
            }

            if (beta <= alpha) break; // 가지치기
        }
        return bestScore;
    }

    private List<(int, int)> GetAvailableMoves()
    {
        List<(int, int)> moves = new List<(int, int)>();
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                if (board[i, j] == 0)
                {
                    moves.Add((i, j));
                }
            }
        }
        return moves;
    }

    private bool IsGameOver()
    {
        return EvaluateBoard(1) >= 100000 || EvaluateBoard(2) >= 100000;
    }

    private int EvaluateBoard(int player)
    {
        int score = 0;
        int opponent = player == 1 ? 2 : 1;

        score += CountScore(player) - CountScore(opponent) * 2; // 상대 점수 고려
        return score;
    }

    private int CountScore(int player)
    {
        int score = 0;
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                if (board[i, j] == player)
                {
                    score += GetLineScore(i, j, player, 1, 0);  // 가로
                    score += GetLineScore(i, j, player, 0, 1);  // 세로
                    score += GetLineScore(i, j, player, 1, 1);  // 대각선 ↘
                    score += GetLineScore(i, j, player, 1, -1); // 대각선 ↙
                }
            }
        }
        return score;
    }

    private int GetLineScore(int x, int y, int player, int dx, int dy)
    {
        int count = 0, openEnds = 0;
        int i = x, j = y;

        // 연속된 돌 개수 세기
        while (i >= 0 && j >= 0 && i < BoardSize && j < BoardSize && board[i, j] == player)
        {
            count++;
            i += dx;
            j += dy;
        }

        // 열린 끝 체크
        if (i >= 0 && j >= 0 && i < BoardSize && j < BoardSize && board[i, j] == 0)
            openEnds++;

        i = x - dx;
        j = y - dy;
        while (i >= 0 && j >= 0 && i < BoardSize && j < BoardSize && board[i, j] == player)
        {
            count++;
            i -= dx;
            j -= dy;
        }

        if (i >= 0 && j >= 0 && i < BoardSize && j < BoardSize && board[i, j] == 0)
            openEnds++;

        return CalculatePatternScore(count, openEnds);
    }

    private int CalculatePatternScore(int count, int openEnds)
    {
        if (count >= 5) return 100000; // 승리 조건
        if (count == 4 && openEnds == 2) return 10000; // 열린 4
        if (count == 4 && openEnds == 1) return 5000;  // 닫힌 4
        if (count == 3 && openEnds == 2) return 1000;  // 열린 3
        if (count == 3 && openEnds == 1) return 500;   // 닫힌 3
        if (count == 2 && openEnds == 2) return 100;   // 열린 2
        if (count == 2 && openEnds == 1) return 50;    // 닫힌 2

        return 0;
    }
}
