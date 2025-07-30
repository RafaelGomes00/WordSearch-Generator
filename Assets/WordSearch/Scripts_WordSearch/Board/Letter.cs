using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ColorType { Selected, Disabled, Correct }
public class Letter : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI letterText;
    [SerializeField] Image image;

    private ColorType t;

    private bool selected;
    private bool clicked;
    private string letter;
    private bool correct;

    private int _index = -1;
    public int index
    {
        get => _index;
        private set
        {
            _index = value;
        }
    }

    void OnEnable()
    {
        GameEvents.OnEnableSquareSelection += OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection += OnDisableSquareSelectionEvent;
        GameEvents.OnSelectSquare += SelectSquare;
    }

    private void Start()
    {
        selected = false;
        clicked = false;
        correct = false;
    }

    public void Initialize(string letter, int index)
    {
        this.index = index;
        this.letter = letter;
        letterText.text = letter.ToString();
    }

    public void SelectSquare(Vector3 position)
    {
        if (this.gameObject.transform.position == position)
        {
            ChangeSelectedColor(ColorType.Selected);
        }
    }

    private void ChangeSelectedColor(ColorType t)
    {
        switch (t)
        {
            case ColorType.Selected:
                image.color = Color.cyan;
                break;
            case ColorType.Disabled:
                image.color = Color.white;
                break;
            case ColorType.Correct:
                image.color = Color.green;
                break;
        }
    }

    public void OnDisableSquareSelectionEvent()
    {
        clicked = false;
        OnDisableSquareSelection();
    }

    public void OnDisableSquareSelection()
    {
        selected = false;

        if (correct)
        {
            ChangeSelectedColor(ColorType.Correct);
        }
        else
        {
            ChangeSelectedColor(ColorType.Disabled);
        }
    }

    public void OnEnableSquareSelection()
    {
        clicked = true;
        selected = false;
    }

    public void OnTouchDown()
    {
        OnEnableSquareSelection();
        GameEvents.EnableSquareSelectionMethod();
        CheckSquare();
        ChangeSelectedColor(ColorType.Selected);
    }

    public void OnTouchEnter()
    {
        GameEvents.MouseOverMethod(this);
        CheckSquare();
    }

    public void OnTouchReleased()
    {
        GameEvents.ClearSelectionMethod();
        GameEvents.DisableSquareSelectionMethod();
    }

    public void Unselect()
    {
        OnDisableSquareSelection();
    }

    public void CheckSquare()
    {
        if (!selected && clicked)
        {
            selected = GameEvents.CheckSquareMethod(letter, gameObject.transform.position, this);
        }
    }

    public void ForceSelectSquare()
    {
        selected = true;
        SelectSquare(transform.position);
    }

    void OnDisable()
    {
        GameEvents.OnEnableSquareSelection -= OnEnableSquareSelection;
        GameEvents.OnDisableSquareSelection -= OnDisableSquareSelectionEvent;
        GameEvents.OnSelectSquare -= SelectSquare;
    }

    public void SetCorrect()
    {
        correct = true;
        ChangeSelectedColor(ColorType.Correct);
    }

    internal string GetLetter()
    {
        return letter;
    }
}
