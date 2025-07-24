using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardData
{
    private string[,] board;
    private string[] words;

    private bool _initialized;

    public BoardData()
    {
        _initialized = false;
    }

    public BoardData(string[,] board, string[] words)
    {
        this.board = board;
        this.words = words;

        _initialized = true;
    }

    public string[,] GetBoardAsMatrix()
    {
        return board;
    }

    public List<string> GetBoardAsList()
    {
        List<string> arrayBoard = new List<string>();
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                arrayBoard.Add(board[x, y]);
            }
        }
        return arrayBoard;
    }

    public int GetSize()
    {
        return board.GetLength(0);
    }

    public string[] GetWords()
    {
        return words;
    }

    public void FillBoard()
    {
        try
        {
            for (int x = 0; x < GetSize(); x++)
            {
                for (int y = 0; y < GetSize(); y++)
                {
                    if (board[x, y] == " ") board[x, y] = ((char)UnityEngine.Random.Range(97, 123)).ToString().ToUpper();
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Board was not initialized");
        }
    }

    public bool CheckNull(int i, int j)
    {
        return string.IsNullOrEmpty(board[i, j]);
    }

    public string GetLetterAt(int i, int j)
    {
        return board[i, j];
    }

    public bool isInitialized()
    {
        return _initialized;
    }
}
