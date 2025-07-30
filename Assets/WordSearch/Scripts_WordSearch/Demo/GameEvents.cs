using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public delegate void EnableSquareSelection();
    public static event EnableSquareSelection OnEnableSquareSelection;
    public static void EnableSquareSelectionMethod()
    {
        OnEnableSquareSelection.Invoke();
    }

    public delegate void DisableSquareSelection();
    public static event DisableSquareSelection OnDisableSquareSelection;
    public static void DisableSquareSelectionMethod()
    {
        OnDisableSquareSelection.Invoke();
    }

    public delegate void SelectSquare(Vector3 position);
    public static event SelectSquare OnSelectSquare;
    public static void SelectSquareMethod(Vector3 position)
    {
        OnSelectSquare.Invoke(position);
    }

    public delegate bool CheckSquare(string letter, Vector3 position, Letter letterRef);
    public static event CheckSquare OnCheckSquare;
    public static bool CheckSquareMethod(string letter, Vector3 position, Letter letterRef)
    {
        return OnCheckSquare.Invoke(letter, position, letterRef);
    }

    public delegate void ClearSelection();
    public static event ClearSelection OnClearSelection;
    public static void ClearSelectionMethod()
    {
        OnClearSelection.Invoke();
    }

    public delegate void MouseOverLetter(Letter letterRef);
    public static event MouseOverLetter OnMouseOverLetter;
    public static void MouseOverMethod(Letter letterRef)
    {
        OnMouseOverLetter?.Invoke(letterRef);
    }
}
