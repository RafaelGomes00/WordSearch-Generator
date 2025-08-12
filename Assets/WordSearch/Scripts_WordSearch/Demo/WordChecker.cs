using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    [SerializeField] private Transform lineParent;
    [SerializeField] private LineRenderer lineRenderer;

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
        GameEvents.OnMouseOverLetter += OnMouseOverLetter;
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

        if (assignedPoints == 0)
        {
            firstSelectedLetter = letterRef;
            rayStartPosition = position;
            AddToSelected(letterRef);

            selected = true;
        }
        else
        {
            Ray newRay = SelectRay(rayStartPosition, position);
            if (newRay.direction != currentRay.direction)
            {
                ResetSelectedLetters();
                currentRay = newRay;
            }

            if (IsPointOnTheRay(currentRay, position))
            {
                AddToSelected(letterRef);
                GameEvents.SelectSquareMethod(position);

                selected = true;
            }

            CheckEmptySpace(currentRay, letterRef);
        }

        Debug.DrawRay(firstSelectedLetter.transform.position, currentRay.direction, Color.black, 2f);

        return selected;
    }

    private void ResetSelectedLetters()
    {
        UnselectAllLeters();
        AddToSelected(firstSelectedLetter);
        firstSelectedLetter.ForceSelectSquare();
    }

    private void AddToSelected(Letter letterRef)
    {
        if (!selectedLettersList.Contains(letterRef))
        {
            selectedLettersList.Add(letterRef);
            assignedPoints++;
        }
    }

    private void UnselectAllLeters()
    {
        foreach (Letter letter in selectedLettersList)
        {
            letter.Unselect();
        }
        selectedLettersList.Clear();
    }

    private void CheckEmptySpace(Ray currentRay, Letter letterRef)
    {
        Letter firstLetter = selectedLettersList[0];

        Vector3 lettersVector = letterRef.transform.position - firstLetter.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(firstLetter.transform.position, lettersVector, lettersVector.magnitude);

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

    private void OnMouseOverLetter(Letter letterRef)
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
                    letter.Unselect();
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
        firstSelectedLetter = null;
        rayStartPosition = new Vector3();
        currentRay = new Ray();
    }

    private void CheckWord()
    {
        string word = "";

        selectedLettersList.Sort((objA, objB) =>
        {
            float distA = Vector3.SqrMagnitude(objA.transform.position - firstSelectedLetter.transform.position);
            float distB = Vector3.SqrMagnitude(objB.transform.position - firstSelectedLetter.transform.position);
            return distA.CompareTo(distB);
        });

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

        if (Mathf.Abs(direction.x + 1) < tolerance && Mathf.Abs(direction.y) < tolerance) return new Ray(firstPosition, new Vector3(-1, 0)); //Left
        if (Mathf.Abs(direction.x - 1) < tolerance && Mathf.Abs(direction.y) < tolerance) return new Ray(firstPosition, new Vector3(1, 0)); //Right

        if (direction.x < 0f && direction.y > 0f) return new Ray(firstPosition, new Vector3(-1, 1)); ; //Diagonal Left Up
        if (direction.x < 0f && direction.y < 0f) return new Ray(firstPosition, new Vector3(-1, -1)); //Diagonal Left Down

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
