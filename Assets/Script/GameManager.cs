using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Caro[] caros;
    public Transform caroContainer;
    public GameObject endPopup;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        caros = caroContainer.GetComponentsInChildren<Caro>();
        ResetBoard();
    }

    public void ResetBoard()
    {
        for (int i = 0; i < caros.Length; i++)
        {
            caros[i].Fill(0);
        }
    }    

    public void HandleBoard(int player)
    {
        int[,] board = GetCurrentBoard();
        int winner = CheckWinner(board);

        switch(winner)
        {
            case -1:
                Debug.Log("Hòa");
                break;
            case 0:
                if(player == 1) OpponentPlayRandom();
                break;
            case 1:
                Debug.Log("X thắng");
                break;
            case 2:
                Debug.Log("O thắng");
                break;
        };    

        if(winner != 0)
        {
            endPopup.SetActive(true);
        }    
    }    

    public void OpponentPlayRandom()
    {
        int move = OpponentPlayMinimax();
        if (move >= 0)
        {
            caros[move].Fill(2);
        }
        HandleBoard(2);
    } 

    // Tro choi O cua may, tim nuoc di tot nhat voi Minimax sau toi da 6 nuoc.
    public int OpponentPlayMinimax()
    {
        int[,] board = GetCurrentBoard();
        int bestMove = -1;
        int bestScore = int.MinValue;

        for (int i = 0; i < 9; i++)
        {
            int x = i % 3;
            int y = i / 3;
            if (board[x, y] != 0) continue;

            board[x, y] = 2;
            int score = Minimax(board, 6 - 1, false, int.MinValue, int.MaxValue);
            board[x, y] = 0;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = i;
            }
        }

        return bestMove;
    }

    private int Minimax(int[,] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        int winner = CheckWinner(board);
        if (winner == 2) return 100 + depth;
        if (winner == 1) return -100 - depth;
        if (winner == -1 || depth == 0) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            for (int i = 0; i < 9; i++)
            {
                int x = i % 3;
                int y = i / 3;
                if (board[x, y] != 0) continue;

                board[x, y] = 2;
                int score = Minimax(board, depth - 1, false, alpha, beta);
                board[x, y] = 0;

                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);
                if (beta <= alpha) break;
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int i = 0; i < 9; i++)
            {
                int x = i % 3;
                int y = i / 3;
                if (board[x, y] != 0) continue;

                board[x, y] = 1;
                int score = Minimax(board, depth - 1, true, alpha, beta);
                board[x, y] = 0;

                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);
                if (beta <= alpha) break;
            }

            return bestScore;
        }
    }

       

    public int[,] GetCurrentBoard()
    {
        int[,] board = new int[3, 3];

        for(int i = 0; i < caros.Length; i++)
        {
            int x = i % 3;
            int y = i / 3;
            board[x, y] = caros[i].currentValue;
        }    

        return board;
    }    

    public int CheckWinner(int[,] board)
    {
        // Kiểm tra hàng ngang và hàng dọc
        for (int i = 0; i < 3; i++)
        {
            // Hàng ngang
            if (board[i, 0] != 0 &&
                board[i, 0] == board[i, 1] &&
                board[i, 1] == board[i, 2])
                return board[i, 0];

            // Hàng dọc
            if (board[0, i] != 0 &&
                board[0, i] == board[1, i] &&
                board[1, i] == board[2, i])
                return board[0, i];
        }

        // Đường chéo chính
        if (board[0, 0] != 0 &&
            board[0, 0] == board[1, 1] &&
            board[1, 1] == board[2, 2])
            return board[0, 0];

        // Đường chéo phụ
        if (board[0, 2] != 0 &&
            board[0, 2] == board[1, 1] &&
            board[1, 1] == board[2, 0])
            return board[0, 2];

        // Kiểm tra còn ô trống không
        foreach (int cell in board)
        {
            if (cell == 0)
                return 0;
        }

        return -1; // Hòa
    }
}
