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

    public void InitializeData(string[,] board, int size, bool canHaveReverseWords, string[] words)
    {
        this.size = size;
        this.canHaveReverseWords = canHaveReverseWords;
        this.words = words;
        ResetBoardData(board);

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