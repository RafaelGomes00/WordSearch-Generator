using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private string[] words;

    private BoardData board;
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
        board = new BoardData();
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

        UpdatePreview();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void CopyInfo()
    {
        string absPath = EditorUtility.OpenFilePanel("Load asset", "", "asset");
        string relPath = absPath.Substring(absPath.IndexOf("Assets/"));

        InfoHolder copiedInfo = (InfoHolder)AssetDatabase.LoadAssetAtPath(relPath, typeof(InfoHolder));

        this.size = copiedInfo.GetSize();
        this.words = copiedInfo.GetWords();
        this.board = copiedInfo.GetBoardData();
        this.canHaveReverseWords = copiedInfo.GetReverseWords();

        so.Update();
    }

    private void SaveBoard()
    {
        string absPath = System.IO.Path.Combine(EditorUtility.SaveFilePanel("Select Directory", "Assets", "New board", "asset"));

        if (absPath.Length != 0)
        {
            string relPath = absPath.Substring(absPath.IndexOf("Assets/"));

            InfoHolder infoHolder = ScriptableObject.CreateInstance<InfoHolder>();

            infoHolder.InitializeData(board);

            AssetDatabase.CreateAsset(infoHolder, relPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void UpdatePreview()
    {
        if (board == null || !board.isInitialized()) return;
        AddGridToLayout(board);
    }

    private void AddGridToLayout(BoardData board)
    {
        if (board.GetSize() >= size && board.GetSize() >= size)
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
