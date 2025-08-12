using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CustomEditor(typeof(BoardHolder), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class EditorWordSearch : Editor
{
    private BoardHolder comp => target as BoardHolder;

    SerializedObject so;
    SerializedProperty words;

    GUIStyle tableStyle;
    GUIStyle headerColumStyle;
    GUIStyle columStyle;
    GUIStyle rowStyle;
    GUIStyle textFieldStyle;

    string[,] showBoard;

    public void OnEnable()
    {
        showBoard = comp.GetBoard();

        so = new SerializedObject(comp);
        words = so.FindProperty("words");
        SetGUIStyle();
    }

    public override void OnInspectorGUI()
    {
        so.Update();

        tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);

        UpdatePreview();

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.Space();
        EditorGUILayout.Toggle("Can have reverse words", comp.GetReverseWords());
        EditorGUILayout.IntField("Size", comp.GetSize());
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(words, true);
        EditorGUILayout.Space();

        EditorGUI.EndDisabledGroup();

        so.ApplyModifiedProperties();
    }

    private void UpdatePreview()
    {
        if (showBoard == null || showBoard.GetLength(0) <= 0) return;
        AddGridToLayout(showBoard);
    }

    private void AddGridToLayout(string[,] showboardref)
    {
        if (showboardref.GetLength(0) >= comp.GetSize() && showboardref.GetLength(1) >= comp.GetSize())
        {
            EditorGUILayout.BeginHorizontal(tableStyle);
            for (int i = 0; i < comp.GetSize(); i++)
            {
                EditorGUILayout.BeginVertical(i == -1 ? headerColumStyle : columStyle);
                for (int j = 0; j < comp.GetSize(); j++)
                {
                    if (i >= 0 && j >= 0)
                    {
                        EditorGUILayout.BeginHorizontal(rowStyle);
                        if (string.IsNullOrEmpty(showboardref[i, j]))
                            EditorGUILayout.Space(30);
                        else
                            EditorGUILayout.LabelField(showboardref[i, j].ToUpper(), textFieldStyle, GUILayout.Width(25));

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

