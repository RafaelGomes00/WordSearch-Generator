using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new words library", fileName = "New words library")]
public class WordsLibrary : ScriptableObject
{
    [SerializeField] List<string> words;

    public List<string> GetWords(int wordsQuantity_Library, int maxWordSize_Library, int minWordSize_Library)
    {
        List<string> possibleWords = words.FindAll(x => x.Length >= minWordSize_Library && x.Length <= maxWordSize_Library);
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
}
