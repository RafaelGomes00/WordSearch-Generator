using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WindowEditor : EditorWindow
{
    GUIStyle tableStyle;
    GUIStyle headerColumStyle;
    GUIStyle columStyle;
    GUIStyle rowStyle;
    GUIStyle textFieldStyle;

    SerializedObject so;
    SerializedProperty wordsSP;
    ScriptableObject target;

    private int size;
    private bool canHaveReverseWords;
    private List<string> addedWords;
    [SerializeField] private string[] words;

    private string[,] board;
    private bool fillBoardAutomatically;


    [MenuItem("Window/Word search creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WindowEditor), false, "Board creator");
    }

    public void OnEnable()
    {
        target = this;
        so = new SerializedObject(target);
        wordsSP = so.FindProperty("words");
        board = new string[1, 1];
        fillBoardAutomatically = true;
        size = 10;

        SetGUIStyle();
        UpdatePreview();
    }

    public void OnGUI()
    {
        tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);

        EditorGUILayout.Space();
        canHaveReverseWords = EditorGUILayout.Toggle("Can have reverse words", canHaveReverseWords);
        size = EditorGUILayout.IntField("Size", size);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(wordsSP, true);

        fillBoardAutomatically = EditorGUILayout.Toggle("Automatically fill board", fillBoardAutomatically);
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            addedWords = new List<string>();
            board = BoardGenerator.CreateWordSearch(words, size, canHaveReverseWords, out addedWords);
            if (fillBoardAutomatically)
                board = BoardGenerator.FillBoard(board, size);
        }

        if (GUILayout.Button("Fill with random letters"))
        {
            board = BoardGenerator.FillBoard(board, size);
        }

        if (GUILayout.Button("Save board"))
        {

            SaveBoard();
        }

        UpdatePreview();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void SaveBoard()
    {
        string absPath = System.IO.Path.Combine(EditorUtility.SaveFilePanel("Select Directory", "Assets", "New board", "asset"));
        string relPath = absPath.Substring(absPath.IndexOf("Assets/"));

        InfoHolder infoHolder = ScriptableObject.CreateInstance<InfoHolder>();

        infoHolder.InitializeData(board, size, canHaveReverseWords, addedWords.ToArray());

        AssetDatabase.CreateAsset(infoHolder, relPath);
        AssetDatabase.SaveAssets();
    }

    private void UpdatePreview()
    {
        if (board == null || board.GetLength(0) <= 0) return;
        AddGridToLayout(board);
    }

    private void AddGridToLayout(string[,] board)
    {
        if (board.GetLength(0) >= size && board.GetLength(1) >= size)
        {
            try
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
                            if (string.IsNullOrEmpty(board[i, j]))
                                EditorGUILayout.Space(30);
                            else
                                EditorGUILayout.LabelField(board[i, j].ToUpper(), textFieldStyle, GUILayout.Width(25));

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            catch (System.Exception) { }
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
