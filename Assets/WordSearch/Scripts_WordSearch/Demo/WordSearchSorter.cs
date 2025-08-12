using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordSearchSorter : MonoBehaviour
{
    [SerializeField] Letter letterGO;
    [SerializeField] RectTransform letterParent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private List<Letter> letters = new List<Letter>();

    public void InitializeLetters(List<string> letters)
    {
        Vector2 size = ResolveGridSize((int)Mathf.Sqrt(letters.Count));

        for (int i = 0; i < letters.Count(); i++)
        {
            Letter letter = Instantiate(letterGO, letterParent);
            letter.Initialize(letters[i].ToString(), i);
            this.letters.Add(letter);
            letter.SetColliderSize(size);
        }
        gridLayoutGroup.cellSize = size;
    }

    public void Reset()
    {
        foreach (Letter letter in letters)
        {
            Destroy(letter.gameObject);
        }
        letters.Clear();
    }

    private Vector2 ResolveGridSize(int size)
    {
        float xSize = Mathf.Clamp((letterParent.rect.width - (size * gridLayoutGroup.spacing.x) - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / size, 20, 200);
        float ySize = Mathf.Clamp((letterParent.rect.height - (size * gridLayoutGroup.spacing.y) - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom) / size, 20, 200);

        return new Vector2(xSize, ySize);
    }
}
