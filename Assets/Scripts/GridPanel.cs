using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridPanel : MaskableGraphic
{

    public Vector2 GridSize;
    public Color GridColor;
    public Color AlternateGridColor;

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    protected override void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        vertexHelper.Clear();
        Rect rect = new Rect(0.0f, 0.0f, width, height);
        DrawRectangle(rect, GridColor, vertexHelper);

        int numberOfRows = 0;
        if (GridSize.y > 0)
            numberOfRows = Mathf.CeilToInt(height / GridSize.y);

        int numberOfColumns = 0;
        if (GridSize.x > 0)
            numberOfColumns = Mathf.CeilToInt(width / GridSize.x);

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int column = row % 2; column < numberOfColumns; column += 2) 
            {
                Rect alternatingRect = new Rect(column * GridSize.x, row * GridSize.y, GridSize.x, GridSize.y);
                DrawRectangle(alternatingRect, AlternateGridColor, vertexHelper);
            }
        }
    }

    private void DrawRectangle(Rect rectangle, Color color, VertexHelper vertexHelper)
    {
        UIVertex[] verts = new UIVertex[4];
        UIVertex vert = UIVertex.simpleVert;

        vert.color = color;
        vert.position = new Vector2(rectangle.position.x, -rectangle.position.y);
        verts[0] = vert;
        vert.position = new Vector2(rectangle.position.x, -(rectangle.position.y + rectangle.size.y));
        verts[1] = vert;
        vert.position = new Vector2(rectangle.position.x + rectangle.size.x, -(rectangle.position.y + rectangle.size.y));
        verts[2] = vert;
        vert.position = new Vector2(rectangle.position.x + rectangle.size.x, -rectangle.position.y);
        verts[3] = vert;
        vertexHelper.AddUIVertexQuad(verts);
    }

    public void SnapToGrid(WindowComponent windowComponent)
    {
        Vector2 snappedAnchoredPosition = windowComponent.RectTransform.anchoredPosition;
        for (int i = 0; i < 2; i++)
        {
            if (GridSize[i] <= 0.0f)
                continue;

            int multiple = Mathf.RoundToInt(snappedAnchoredPosition[i] / GridSize[i]);
            snappedAnchoredPosition[i] = GridSize[i] * multiple;
        }
        windowComponent.RectTransform.anchoredPosition = snappedAnchoredPosition;
    }

    public void ClampInsideGridPanel(WindowComponent windowComponent)
    {
        Vector2 gridMin = new Vector2(0.0f, -_rectTransform.rect.height);
        Vector2 gridMax = new Vector2(_rectTransform.rect.width, 0.0f);
        Bounds windowComponentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, windowComponent.ExpandedRectTransform);

        Vector2 fixedAnchoredPosition = windowComponent.RectTransform.anchoredPosition;
        for (int i=0; i < 2; i++)
        {
            
            float minDifference = gridMin[i] - windowComponentBounds.min[i];
            if (minDifference > 0.0f)
            {
                fixedAnchoredPosition[i] += minDifference;
            }

            float maxDifference = gridMax[i] - windowComponentBounds.max[i];
            if (maxDifference < 0.0f)
            {
                fixedAnchoredPosition[i] += maxDifference;
            }
        }

        windowComponent.RectTransform.anchoredPosition = fixedAnchoredPosition;
    }

    public bool CanWindowBePlaced(WindowComponent windowComponent)
    {
        Bounds windowComponentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, windowComponent.ExpandedRectTransform);

        int numberOfChildren = _rectTransform.childCount;
        for (int i=0; i < numberOfChildren; i++)
        {
            WindowComponent otherWindow = _rectTransform.GetChild(i).GetComponent<WindowComponent>();
            if (otherWindow == windowComponent || otherWindow == null)
                continue;

            Bounds otherWindowBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, otherWindow.ExpandedRectTransform);
            if (otherWindowBounds.Intersects(windowComponentBounds))
                return false;
        }
        return true;
    }
    

}
