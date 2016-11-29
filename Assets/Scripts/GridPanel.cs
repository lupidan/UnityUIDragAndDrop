using UnityEngine;
using System.Collections;

public class GridPanel : MonoBehaviour
{

    public Vector2 GridSize;

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    public Vector2 SnapToGrid(Vector2 anchoredPosition)
    {
        Vector2 snappedAnchoredPosition = anchoredPosition;
        for (int i = 0; i < 2; i++)
        {
            if (GridSize[i] <= 0.0f)
                continue;

            int multiple = Mathf.RoundToInt(anchoredPosition[i] / GridSize[i]);
            snappedAnchoredPosition[i] = GridSize[i] * multiple;
        }
        return snappedAnchoredPosition;
    }

}
