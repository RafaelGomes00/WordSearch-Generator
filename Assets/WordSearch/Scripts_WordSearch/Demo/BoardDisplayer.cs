using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardDisplayer : MonoBehaviour
{
    [SerializeField] private WordSearchSorter wordSearchSorter;
    [SerializeField] private GameObject textObj;
    [SerializeField] private Transform textObjParent;

    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private WordChecker wordChecker;

    [Header("Static board")]
    [SerializeField] bool useStaticBoard;
    [SerializeField] private BoardHolder board;

    [Header("Dynamic board")]
    [SerializeField] private List<string> words;
    [SerializeField] private bool canHaveReverseWords;
    [SerializeField] private int size;

    private Dictionary<string, TextMeshProUGUI> wordToTextRelation = new Dictionary<string, TextMeshProUGUI>();

    private void Start()
    {
        ResetBoard();
    }

    public void ResetBoard()
    {
        wordSearchSorter.Reset();
        wordChecker.Reset();

        foreach (TextMeshProUGUI text in wordToTextRelation.Values)
        {
            Destroy(text.gameObject);
        }
        wordToTextRelation = new Dictionary<string, TextMeshProUGUI>();

        CreateBoard();
    }

    public void CreateBoard()
    {
        BoardData boardData = BoardGenerator.CreateWordSearch(words.ToArray(), size, canHaveReverseWords);

        if (useStaticBoard)
        {
            boardData = board.GetBoardData();
        }

        wordSearchSorter.InitializeLetters(boardData.GetBoardAsList());
        gridLayoutGroup.constraintCount = boardData.GetSize();

        foreach (string word in boardData.GetWords())
        {
            GameObject obj = Instantiate(textObj, textObjParent);
            TextMeshProUGUI txt = obj.GetComponent<TextMeshProUGUI>();
            txt.text = word;
            wordToTextRelation.Add(word.ToUpper(), txt);
        }

        wordChecker.SetWordToTextRelation(wordToTextRelation);
    }
}
