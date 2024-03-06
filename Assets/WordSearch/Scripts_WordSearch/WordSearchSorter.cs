using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class WordSearchSorter : MonoBehaviour
{
    [SerializeField] Letter letterGO;
    [SerializeField] Transform letterParent;

    public void InitializeLetters(List<string> letters)
    {
        for(int i = 0; i < letters.Count(); i++)
        {
            Instantiate(letterGO, letterParent).Initiate(letters[i].ToString(), i);
        }       
    }
}
