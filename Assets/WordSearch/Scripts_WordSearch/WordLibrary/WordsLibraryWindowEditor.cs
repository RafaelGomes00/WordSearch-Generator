using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WordsLibraryWindowEditor : EditorWindow
{
    GUIStyle tableStyle;

    SerializedObject so;
    ScriptableObject target;

    string search;
    Vector2 scrollPos;
    string addWord;
    [SerializeField] private TextAsset textFile;

    private List<string> words;

    [MenuItem("Window/Word library editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WordsLibraryWindowEditor), false, "Words library");
    }

    public void OnEnable()
    {
        target = this;
        so = new SerializedObject(target);
        words = new List<string>();
    }

    public void OnGUI()
    {
        tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(5, 5, 5, 5);


        TextAsset originalTextFile = textFile;
        textFile = (TextAsset)EditorGUILayout.ObjectField(textFile, typeof(TextAsset), false);
        if (originalTextFile != textFile)
        {
            UpdateWords();
        }

        search = EditorGUILayout.TextField("Search", search);

        EditorGUILayout.BeginVertical(tableStyle);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width - 25), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(350));

        for (int i = 0; i < words.Count; i++)
        {
            RenderWordElement(i, search);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        RenderExtraWordElement();

        if (GUILayout.Button("Save"))
        {
            if (Save()) Debug.Log($"Words saved on path: {AssetDatabase.GetAssetPath(textFile)}");
        }

        if (GUILayout.Button("Save as"))
        {
            string path;
            if (SaveAs(out path)) Debug.Log($"Words saved on path: {path}");
        }
    }

    private void RenderWordElement(int index, string search)
    {
        if (words == null) return;
        if (!string.IsNullOrEmpty(search) && !words[index].ToUpper().StartsWith(search.ToUpper())) return;

        GUIStyle elementStyle = new GUIStyle("box");
        elementStyle.padding = new RectOffset(2, 2, 2, 2);

        EditorGUILayout.BeginHorizontal(elementStyle);
        words[index] = EditorGUILayout.TextField($"Element {index}: ", words[index]);
        if (GUILayout.Button("Delete", GUILayout.Width(50)))
        {
            words.RemoveAt(index);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void RenderExtraWordElement()
    {
        GUIStyle elementStyle = new GUIStyle("box");
        elementStyle.padding = new RectOffset(2, 2, 2, 2);

        EditorGUILayout.BeginHorizontal(elementStyle);
        addWord = EditorGUILayout.TextField($"Add: ", addWord);
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            if (addWord != "")
            {
                if (!words.Contains(addWord))
                    words.Add(addWord);
                else
                    Debug.LogWarning($"Word {addWord} already exists");

                addWord = "";
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateWords()
    {
        if (textFile == null) words.Clear();
        else words = textFile.text.Replace(" ", "").Split(',').ToList();
    }

    private bool Save()
    {
        try
        {
            string path = AssetDatabase.GetAssetPath(textFile);
            File.WriteAllText(path, BuildFileSave());
            AssetDatabase.Refresh();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Something went wrong saving the file: " + ex);
            return false;
        }
    }

    private bool SaveAs(out string path)
    {
        path = "";
        try
        {
            string absPath = System.IO.Path.Combine(EditorUtility.SaveFilePanel("Select Directory", "Assets", "New words library", "txt"));

            if (absPath.Length != 0)
            {
                string relPath = absPath.Substring(absPath.IndexOf("Assets/"));
                File.WriteAllText(relPath, BuildFileSave());

                path = relPath;

            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Something went wrong saving the file: " + ex);
            return false;
        }
    }

    private string BuildFileSave()
    {
        string fileText = "";
        for (int i = 0; i < words.Count; i++)
        {
            fileText += words[i];
            if (i < words.Count - 1)
                fileText += ',';
        }

        return fileText;
    }
}
