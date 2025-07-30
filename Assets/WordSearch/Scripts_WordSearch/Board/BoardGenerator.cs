using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoardGenerator
{
    public static BoardData CreateWordSearch(string[] words, int size, bool canHaveReverseWords, bool autoFill = true)
    {
        List<string> placedWords = new List<string>();

        if (words == null || words.Length < 1)
        {
            Debug.LogError("No words given");
            return new BoardData();
        }

        string[,] board = new string[size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                board[x, y] = " ";

        foreach (string word in words)
        {
            bool placed = PlaceWord(board, word.ToUpper(), size, canHaveReverseWords);
            if (placed) placedWords.Add(word);
        }

        BoardData boardData = new BoardData(board, placedWords.ToArray());
        if (autoFill) boardData.FillBoard();

        return boardData;
    }

    public static bool PlaceWord(string[,] board, string word, int size, bool canHaveReverseWords)
    {
        int diagonalRandomIndex = Random.Range(0, 2);

        List<Orientation> triedOrientations = new List<Orientation>();
        bool placed = false;

        if (word.Length > size)
        {
            Debug.LogWarning($"Word {word} is bigger than board size");
            return false;
        }

        while (!placed)
        {
            Orientation orientation = RandomizeOrientation(triedOrientations);
            word = word.Trim();

            if (orientation == Orientation.Vertical)
            {
                placed = TryVerticalPlacement(board, word, size, canHaveReverseWords);
            }
            else if (orientation == Orientation.Horizontal)
            {
                placed = TryHorizontalPlacement(board, word, size, canHaveReverseWords);
            }
            else if (orientation == Orientation.Diagonal && diagonalRandomIndex == 0)
            {
                placed = TryDiagonalTLBR(board, word, size, canHaveReverseWords);
            }
            else if (orientation == Orientation.Diagonal && diagonalRandomIndex == 1)
            {
                placed = TryDiagonalBLTR(board, word, size, canHaveReverseWords);
            }

            triedOrientations.Add(orientation);
            if (triedOrientations.Count >= 3 && !placed)
            {
                Debug.LogWarning($"Word {word} couldn't be placed");
                break;
            }
        }

        return placed;
    }

    //Bottom left to Top right
    private static bool TryDiagonalBLTR(string[,] board, string word, int size, bool canHaveReverseWords)
    {
        if (GenerateReverseBool(canHaveReverseWords))
            word = ReverseString(word);

        int randomCol = Random.Range(0, size - word.Length);
        int randomRow = Random.Range(word.Length - 1, size - 1);


        int indexCol = randomCol;
        int indexRow = randomRow;

        do
        {
            do
            {
                if (CheckAvailableSpaceDiagonalBLTR(board, word, indexRow, indexCol))
                {
                    int yIndex = indexCol;
                    int xIndex = indexRow;

                    for (int i = 0; i < word.Length; i++)
                    {
                        board[yIndex, xIndex] = word[i].ToString();
                        yIndex++;
                        xIndex--;
                    }
                    return true;
                }

                indexRow++;
                if (indexRow > size - 1)
                    indexRow = word.Length - 1;

            } while (indexRow != randomRow);

            indexCol++;
            if (indexCol + word.Length > size)
                indexCol = 0;

        } while (indexCol != randomCol);
        return false;
    }
    private static bool CheckAvailableSpaceDiagonalBLTR(string[,] board, string word, int row, int col)
    {
        int xIndex = row;
        int yIndex = col;

        for (int i = 0; i < word.Length; i++)
        {
            if (board[yIndex, xIndex] == " " || board[xIndex, yIndex].ToUpper() == word[i].ToString().ToUpper())
            {
                xIndex--;
                yIndex++;
            }
            else return false;
        }

        return true;
    }

    //Top left to Bottom right
    private static bool TryDiagonalTLBR(string[,] board, string word, int size, bool canHaveReverseWords)
    {
        if (GenerateReverseBool(canHaveReverseWords))
            word = ReverseString(word);

        int randomCol = Random.Range(0, size - word.Length);
        int randomRow = Random.Range(0, size - word.Length);

        int indexCol = randomCol;
        int indexRow = randomRow;

        do
        {
            do
            {
                if (CheckAvailableSpaceDiagonalTLBR(board, word, indexRow, indexCol))
                {
                    int yIndex = indexCol;
                    int xIndex = indexRow;

                    for (int i = 0; i < word.Length; i++)
                    {
                        board[xIndex, yIndex] = word[i].ToString();
                        yIndex++;
                        xIndex++;
                    }
                    return true;
                }

                indexRow++;
                if (indexRow + word.Length > size)
                    indexRow = 0;

            } while (indexRow != randomRow);

            indexCol++;
            if (indexCol + word.Length > size)
                indexCol = 0;

        } while (indexCol != randomCol);
        return false;
    }
    private static bool CheckAvailableSpaceDiagonalTLBR(string[,] board, string word, int row, int col)
    {
        int xIndex = row;
        int yIndex = col;

        for (int i = 0; i < word.Length; i++)
        {
            if (board[xIndex, yIndex] == " " || board[xIndex, yIndex].ToUpper() == word[i].ToString().ToUpper())
            {
                xIndex++;
                yIndex++;
            }
            else return false;
        }

        return true;
    }

    private static bool TryHorizontalPlacement(string[,] board, string word, int size, bool canHaveReverseWords)
    {
        if (GenerateReverseBool(canHaveReverseWords))
            word = ReverseString(word);

        int randomCol = Random.Range(0, size);
        int randomRow = Random.Range(0, size - word.Length);

        int indexCol = randomCol;
        int indexRow = randomRow;

        do
        {
            do
            {
                if (CheckAvailableSpaceHorizontal(board, word, indexCol, indexRow))
                {
                    int xIndex = indexRow;
                    for (int i = 0; i < word.Length; i++)
                    {
                        board[xIndex, indexCol] = word[i].ToString();
                        xIndex++;
                    }
                    return true;
                }

                indexRow++;
                if (indexRow + word.Length > size)
                    indexRow = 0;

            } while (indexRow != randomRow);

            indexCol++;
            if (indexCol >= size)
                indexCol = 0;

        } while (indexCol != randomCol);
        return false;
    }
    private static bool CheckAvailableSpaceHorizontal(string[,] board, string word, int col, int row)
    {
        int xIndex = row;
        for (int i = 0; i < word.Length; i++)
        {
            if (board[xIndex, col] == " " || board[xIndex, col].ToUpper() == word[i].ToString().ToUpper())
            {
                xIndex++;
            }
            else return false;
        }

        return true;
    }

    private static bool TryVerticalPlacement(string[,] board, string word, int size, bool canHaveReverseWords)
    {
        if (GenerateReverseBool(canHaveReverseWords))
            word = ReverseString(word);

        int randomCol = Random.Range(0, size - word.Length);
        int randomRow = Random.Range(0, size);

        int indexCol = randomCol;
        int indexRow = randomRow;

        do
        {
            do
            {
                if (CheckAvailableSpaceVertical(board, word, indexRow, indexCol))
                {
                    int yIndex = indexCol;
                    for (int i = 0; i < word.Length; i++)
                    {
                        board[indexRow, yIndex] = word[i].ToString();
                        yIndex++;
                    }
                    return true;
                }

                indexRow++;
                if (indexRow > size - 1)
                    indexRow = 0;

            } while (indexRow != randomRow);

            indexCol++;
            if (indexCol + word.Length > size - 1)
                indexCol = 0;

        } while (indexCol != randomCol);
        return false;
    }
    private static bool CheckAvailableSpaceVertical(string[,] board, string word, int row, int column)
    {
        int yIndex = column;
        for (int i = 0; i < word.Length; i++)
        {
            if (board[row, yIndex] == " " || board[row, yIndex].ToUpper() == word[i].ToString().ToUpper())
            {
                yIndex++;
            }
            else return false;
        }

        return true;
    }

    private static Orientation RandomizeOrientation(List<Orientation> triedOrientations)
    {
        System.Array values = System.Enum.GetValues(typeof(Orientation));
        Orientation orientation;
        do
        {
            orientation = (Orientation)values.GetValue(Random.Range(0, 3));
        } while (triedOrientations.Contains(orientation));

        return orientation;
    }

    private static bool GenerateReverseBool(bool canHaveReverseWords)
    {
        if (!canHaveReverseWords) return false;
        return GenerateRandomBool();
    }

    private static string ReverseString(string word)
    {
        return new string(word.Reverse().ToArray());
    }

    private static bool GenerateRandomBool()
    {
        return Random.Range(0, 100) < 50;
    }
}
