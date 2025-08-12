using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

public class WindowEditor : EditorWindow
{
    GUIStyle tableStyle;
    GUIStyle headerColumStyle;
    GUIStyle columStyle;
    GUIStyle rowStyle;
    GUIStyle textFieldStyle;

    SerializedObject so;
    ScriptableObject target;

    [SerializeField] private string[] words;
    private bool canHaveReverseWords;
    private int size;

    private BoardData board;
    private bool fillBoardAutomatically;
    Vector2 scrollPos;

    //Words library
    [SerializeField] private TextAsset wordsLibrary;
    private int wordsQuantity_Library;
    private int maxWordSize_Library;
    private int minWordSize_Library;
    bool showWordsLibrary;


    [MenuItem("Window/Word search creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WindowEditor), false, "Board creator");
    }

    public void OnEnable()
    {
        target = this;
        so = new SerializedObject(target);
        board = new BoardData();
        fillBoardAutomatically = true;
        wordsQuantity_Library = 10;
        maxWordSize_Library = 20;
        minWordSize_Library = 1;
        size = 10;

        SetGUIStyle();
        UpdatePreview();
    }

    public void OnGUI()
    {
        tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);

        EditorGUILayout.Space();
        size = EditorGUILayout.IntField("Size", size);
        canHaveReverseWords = EditorGUILayout.Toggle("Can have reverse words", canHaveReverseWords);
        EditorGUILayout.Space();

        RenderWordsProperty();
        EditorGUILayout.Space(10);

        RenderWordsLibrary();
        EditorGUILayout.Space(10);

        fillBoardAutomatically = EditorGUILayout.Toggle("Automatically fill board", fillBoardAutomatically);
        RenderLayoutButtons();

        UpdatePreview();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void UpdatePreview()
    {
        if (board == null || !board.isInitialized()) return;
        AddGridToLayout(board);
    }

    private void RenderWordsProperty()
    {
        SerializedProperty wordsSP = so.FindProperty("words");
        float height = wordsSP.isExpanded ? 200 : 25;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(height));
        EditorGUILayout.PropertyField(wordsSP, true);
        EditorGUILayout.EndScrollView();
    }

    private void CopyInfo()
    {
        string absPath = EditorUtility.OpenFilePanel("Load asset", "", "asset");
        string relPath = absPath.Substring(absPath.IndexOf("Assets/"));

        BoardHolder copiedInfo = (BoardHolder)AssetDatabase.LoadAssetAtPath(relPath, typeof(BoardHolder));

        this.size = copiedInfo.GetSize();
        this.words = copiedInfo.GetWords();
        this.board = copiedInfo.GetBoardData();
        this.canHaveReverseWords = copiedInfo.GetReverseWords();

        so.Update();
    }

    private void RenderWordsLibrary()
    {
        showWordsLibrary = EditorGUILayout.BeginFoldoutHeaderGroup(showWordsLibrary, "Words library");
        EditorGUILayout.Space();
        if (showWordsLibrary)
        {
            wordsLibrary = (TextAsset)EditorGUILayout.ObjectField(wordsLibrary, typeof(TextAsset), false);
            wordsQuantity_Library = EditorGUILayout.IntField("Words quantity", wordsQuantity_Library);
            maxWordSize_Library = EditorGUILayout.IntField("Maximum word size", maxWordSize_Library);
            minWordSize_Library = EditorGUILayout.IntField("Minimum word size", minWordSize_Library);

            EditorGUILayout.Space();
            if (GUILayout.Button("Get random words"))
            {
                if (wordsLibrary == null)
                {
                    Debug.LogError("Words library not set.");
                }
                else
                {
                    List<string> words_Library = GetWords(wordsLibrary, wordsQuantity_Library, maxWordSize_Library, minWordSize_Library);
                    if (words == null) words = words_Library.ToArray();
                    else
                    {
                        words_Library.AddRange(words);
                        words = words_Library.ToArray();
                    }
                }
                so.Update();
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private List<string> GetWords(TextAsset textFile, int wordsQuantity_Library, int maxWordSize_Library, int minWordSize_Library)
    {
        if (words == null) words = new string[0];
        List<string> possibleWords = GetPossibleWords(textFile, words, maxWordSize_Library, minWordSize_Library);
        if (possibleWords.Count < wordsQuantity_Library) return possibleWords;

        List<string> sortedWords = new List<string>();

        for (int i = 0; i < wordsQuantity_Library; i++)
        {
            string sorted;
            sorted = possibleWords[UnityEngine.Random.Range(0, possibleWords.Count)];

            sortedWords.Add(sorted);
            possibleWords.Remove(sorted);
        }

        return sortedWords;
    }

    private List<string> GetPossibleWords(TextAsset textFile, string[] currentWords, int maxWordSize_Library, int minWordSize_Library)
    {
        List<string> fileWords = textFile.text.Replace(" ", "").Split(',').ToList();
        return fileWords.FindAll(x => x.Length >= minWordSize_Library && x.Length <= maxWordSize_Library && !currentWords.Contains(x));
    }

    private void SaveBoard()
    {
        string absPath = System.IO.Path.Combine(EditorUtility.SaveFilePanel("Select Directory", "Assets", "New board", "asset"));

        if (absPath.Length != 0)
        {
            string relPath = absPath.Substring(absPath.IndexOf("Assets/"));

            BoardHolder infoHolder = ScriptableObject.CreateInstance<BoardHolder>();

            infoHolder.InitializeData(board);

            AssetDatabase.CreateAsset(infoHolder, relPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void RenderLayoutButtons()
    {
        if (GUILayout.Button("Generate"))
        {
            board = BoardGenerator.CreateWordSearch(words, size, canHaveReverseWords, fillBoardAutomatically);
            if (fillBoardAutomatically)
                board.FillBoard();
        }

        if (GUILayout.Button("Fill with random letters"))
        {
            board.FillBoard();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Copy info from board"))
        {
            CopyInfo();
        }

        GUI.enabled = board.isInitialized();
        if (GUILayout.Button("Save board"))
        {
            SaveBoard();
        }
        GUI.enabled = true;
    }

    private void AddGridToLayout(BoardData board)
    {
        if (board.GetSize() >= size)
        {
            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int i = 0; i < size; i++)
            {
                EditorGUILayout.BeginVertical(i == -1 ? headerColumStyle : columStyle);
                for (int j = 0; j < size; j++)
                {
                    if (i >= 0 && j >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        if (board.CheckNull(i, j))
                            EditorGUILayout.Space(30);
                        else
                            EditorGUILayout.LabelField(board.GetLetterAt(i, j).ToUpper(), textFieldStyle, GUILayout.Width(25));

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void SetGUIStyle()
    {
        headerColumStyle = new GUIStyle();
        headerColumStyle.fixedWidth = 50;

        columStyle = new GUIStyle();
        columStyle.fixedWidth = 30; //TODO: Calculate the number of objects and it's size to determine a proper spacing

        rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.fixedWidth = 10;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        textFieldStyle = new GUIStyle();
        textFieldStyle.normal.background = Texture2D.grayTexture;
        textFieldStyle.normal.textColor = Color.white;
        textFieldStyle.fontStyle = FontStyle.Normal;
        textFieldStyle.alignment = TextAnchor.MiddleCenter;
    }
}
