using UnityEngine;

public class ComponentPanel : MonoBehaviour
{

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } } 

    void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    public void ClampInsideGridPanel(WindowComponent windowComponent)
    {
        Vector2 gridMin = new Vector2(0.0f, -_rectTransform.rect.height);
        Vector2 gridMax = new Vector2(_rectTransform.rect.width, 0.0f);
        Bounds windowComponentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, windowComponent.CompactRectTransform);

        Vector2 fixedAnchoredPosition = windowComponent.RectTransform.anchoredPosition;
        for (int i = 0; i < 2; i++)
        {
            float minDifference = gridMin[i] - windowComponentBounds.min[i];
            if (minDifference > 0.0f)
                fixedAnchoredPosition[i] += minDifference;

            float maxDifference = gridMax[i] - windowComponentBounds.max[i];
            if (maxDifference < 0.0f)
                fixedAnchoredPosition[i] += maxDifference;
        }

        windowComponent.RectTransform.anchoredPosition = fixedAnchoredPosition;
    }
}
