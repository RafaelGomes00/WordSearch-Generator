using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InfoHolder : ScriptableObject
{
    [SerializeField] private int size;
    [SerializeField] private bool canHaveReverseWords;
    [SerializeField] private string[] words;
    [SerializeField] private List<string> board;

    public void InitializeData(BoardData boardData)
    {
        string[,] matrixBoard = boardData.GetBoardAsMatrix();
        size = matrixBoard.GetLength(0);
        ResetBoardData(matrixBoard);

        board = boardData.GetBoardAsList();
        words = boardData.GetWords().ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ResetBoardData(string[,] refBoard)
    {
        board = new List<string>();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                board.Add(refBoard[x, y]);
            }
        }
    }

    public string[,] GetBoard()
    {
        return TransformBoard(board);
    }

    public BoardData GetBoardData()
    {
        return new BoardData(TransformBoard(board), words);
    }

    public int GetSize()
    {
        return size;
    }

    public bool GetReverseWords()
    {
        return canHaveReverseWords;
    }

    public string[] GetWords()
    {
        return words;
    }

    private string[,] TransformBoard(List<string> board)
    {
        string[,] newShowBoard = new string[size, size];
        int index = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                newShowBoard[x, y] = board[index];
                index++;
            }
        }

        return newShowBoard;
    }
}