using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordChecker : MonoBehaviour
{
    [SerializeField] private Transform lineParent;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private int assignedPoints = 0;
    private Ray currentRay;
    private Vector3 rayStartPosition;
    Letter firstSelectedLetter;

    private List<Letter> selectedLettersList = new List<Letter>();
    private List<LineRenderer> lines = new List<LineRenderer>();
    private Dictionary<string, TextMeshProUGUI> wordToTextRelation = new Dictionary<string, TextMeshProUGUI>();
    private bool invalidRay;

    private void OnEnable()
    {
        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
        GameEvents.OnMouseOver += OnMouseOver;
    }

    private void Update()
    {
        if (assignedPoints > 0 && Application.isEditor)
        {
            Debug.DrawRay(currentRay.origin, currentRay.direction * 1000f, Color.black);
        }
    }

    public void SetWordToTextRelation(Dictionary<string, TextMeshProUGUI> wordToTextRelation)
    {
        this.wordToTextRelation = wordToTextRelation;
    }

    public void Reset()
    {
        assignedPoints = 0;
        currentRay = new Ray();
        rayStartPosition = new Vector3();
        firstSelectedLetter = null;
        selectedLettersList = new List<Letter>();
        wordToTextRelation = new Dictionary<string, TextMeshProUGUI>();

        foreach (LineRenderer line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }

    private bool SquareSelected(string letter, Vector3 position, Letter letterRef)
    {
        bool selected = false;

        if (assignedPoints == 1)
            currentRay = SelectRay(rayStartPosition, position);

        if (assignedPoints > 0)
        {
            CheckEmptySpace(currentRay, letterRef);
        }

        if (assignedPoints == 0)
        {
            firstSelectedLetter = letterRef;
            rayStartPosition = position;
            AddToSelected(letterRef);

            selected = true;
        }
        else if (assignedPoints == 1 && CheckNeighbourLetter(firstSelectedLetter, letterRef))
        {
            AddToSelected(letterRef);
            GameEvents.SelectSquareMethod(position);

            selected = true;
        }
        else
        {
            if (IsPointOnTheRay(currentRay, position))
            {
                AddToSelected(letterRef);
                GameEvents.SelectSquareMethod(position);

                selected = true;
            }
        }

        return selected;

    }

    private bool CheckNeighbourLetter(Letter firstSelectedLetter, Letter letterRef)
    {
        if (firstSelectedLetter.index + 1 == letterRef.index) return true; //Direita
        else if (firstSelectedLetter.index - gridLayoutGroup.constraintCount == letterRef.index) return true; //Cima
        else if (firstSelectedLetter.index - gridLayoutGroup.constraintCount + 1 == letterRef.index) return true; //Diagonal cima
        else if (firstSelectedLetter.index + gridLayoutGroup.constraintCount == letterRef.index) return true; //Baixo
        else if (firstSelectedLetter.index + gridLayoutGroup.constraintCount + 1 == letterRef.index) return true;  //Diagonal baixo
        return false;
    }

    private void AddToSelected(Letter letterRef)
    {
        if (!selectedLettersList.Contains(letterRef))
        {
            selectedLettersList.Add(letterRef);
            assignedPoints++;
        }
    }

    private void CheckEmptySpace(Ray currentRay, Letter letterRef)
    {
        Letter firstLetter = selectedLettersList[0];

        Vector3 lettersVector = letterRef.transform.position - firstLetter.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(firstLetter.transform.position, lettersVector, lettersVector.magnitude);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit hit in hits)
        {
            if (IsPointOnTheRay(currentRay, hit.transform.position))
            {
                Letter toSelectWord = hit.collider.GetComponent<Letter>();
                AddToSelected(toSelectWord);
                toSelectWord.ForceSelectSquare();
            }
        }
    }

    private void OnMouseOver(Letter letterRef)
    {
        if (selectedLettersList.Count > 1)
        {
            float distance = Vector3.Distance(firstSelectedLetter.transform.position, letterRef.transform.position);
            List<Letter> lettersToRemove = new List<Letter>();

            foreach (Letter letter in selectedLettersList)
            {
                if (!IsPointOnTheRay(currentRay, letter.transform.position, distance) && letter != firstSelectedLetter)
                {
                    lettersToRemove.Add(letter);
                    letter.UnselectWord();
                    assignedPoints--;
                }
            }

            foreach (Letter letter in lettersToRemove)
            {
                selectedLettersList.Remove(letter);
            }

            lettersToRemove.Clear();
        }
    }

    private void ClearSelection()
    {
        CheckWord();
        selectedLettersList.Clear();
        assignedPoints = 0;
    }

    private void CheckWord()
    {
        string word = "";
        foreach (Letter letter in selectedLettersList)
        {
            word += letter.GetLetter();
        }

        if (wordToTextRelation.ContainsKey(word))
        {
            Debug.Log($"Found word {word}");
            foreach (Letter letterRef in selectedLettersList)
            {
                letterRef.SetCorrect();
            }

            LineRenderer line = Instantiate(lineRenderer, lineParent);
            line.SetPositions(new Vector3[] {
                        selectedLettersList[0].transform.localPosition,
                        selectedLettersList[selectedLettersList.Count - 1].transform.localPosition
                        });
            lines.Add(line);

            wordToTextRelation[word].fontStyle = FontStyles.Strikethrough;
        }
    }

    private Ray SelectRay(Vector3 firstPosition, Vector3 secondPosition)
    {
        var direction = (secondPosition - firstPosition).normalized;
        float tolerance = 0.01f;

        invalidRay = false;

        if (Mathf.Abs(direction.x) < tolerance && Mathf.Abs(direction.y - 1) < tolerance) return new Ray(firstPosition, new Vector3(0, 1)); //Up
        if (Mathf.Abs(direction.x) < tolerance && Mathf.Abs(direction.y + 1) < tolerance) return new Ray(firstPosition, new Vector3(0, -1)); //Down
        if (Mathf.Abs(direction.x) + 1f < tolerance && Mathf.Abs(direction.y) < tolerance) invalidRay = true; //Left
        if (Mathf.Abs(direction.x) - 1f < tolerance && Mathf.Abs(direction.y) < tolerance) return new Ray(firstPosition, new Vector3(1, 0)); //Right
        if (direction.x < 0f && direction.y > 0f) invalidRay = true; //Diagonal Left Up
        if (direction.x < 0f && direction.y < 0f) invalidRay = true; //Diagonal Left Down
        if (direction.x > 0f && direction.y > 0f) return new Ray(firstPosition, new Vector3(1, 1)); //Diagonal Right Up
        if (direction.x > 0f && direction.y < 0f) return new Ray(firstPosition, new Vector3(1, -1)); //Diagonal Right Down

        Debug.Log("Invalid ray");
        return new Ray();
    }

    private bool IsPointOnTheRay(Ray ray, Vector3 point, float distance = 100f)
    {
        if (invalidRay) return false;

        var hits = Physics.RaycastAll(ray, distance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.position == point)
                return true;
        }
        return false;
    }

    private void OnDisable()
    {
        GameEvents.OnCheckSquare -= SquareSelected;
        GameEvents.OnClearSelection -= ClearSelection;
    }
}
