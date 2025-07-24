using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class WordSearchSorter : MonoBehaviour
{
    [SerializeField] Letter letterGO;
    [SerializeField] Transform letterParent;

    private List<Letter> letters = new List<Letter>();

    public void InitializeLetters(List<string> letters)
    {
        for (int i = 0; i < letters.Count(); i++)
        {
            Letter letter = Instantiate(letterGO, letterParent);
            letter.Initialize(letters[i].ToString(), i);
            this.letters.Add(letter);
        }
    }

    public void Reset()
    {
        foreach (Letter letter in letters)
        {
            Destroy(letter.gameObject);
        }
        letters.Clear();
    }
}
