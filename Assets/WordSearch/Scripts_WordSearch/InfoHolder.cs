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
    [SerializeField] private List<string> board;
    [SerializeField] private int size;
    [SerializeField] private bool canHaveReverseWords;
    [SerializeField] private string[] words;

    private BoardData boardData;
    

    public void InitializeData(BoardData boardData)
    {
        this.boardData = boardData;

        ResetBoardData(boardData.GetBoardAsMatrix());
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
                board.Add(refBoard[x,y]);
            }
        }
    }

    public List<string> GetBoard()
    {
        return board;
    }

    public BoardData GetBoardData()
    {
        return boardData;
    }

    public int GetSize()
    {
        return size;
    }

    public bool GetReverseWords()
    {
        return canHaveReverseWords;
    }

    public IEnumerable<string> GetWords()
    {
        return words;
    }
}