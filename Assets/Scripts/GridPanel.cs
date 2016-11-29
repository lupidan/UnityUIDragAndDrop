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

    public bool CanWindowBePlaced(WindowComponent windowComponent)
    {
        Bounds windowComponentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, windowComponent.ExpandedRectTransform);
        int numberOfChildren = _rectTransform.childCount;
        for (int i=0; i < numberOfChildren; i++)
        {
            WindowComponent otherWindow = _rectTransform.GetChild(i).GetComponent<WindowComponent>();
            if (otherWindow == null || otherWindow == windowComponent)
                continue;

            Bounds otherWindowBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_rectTransform, otherWindow.ExpandedRectTransform);
            otherWindowBounds.extents -= new Vector3(0.01f, 0.01f, 0.01f);
            Debug.Log(windowComponentBounds + " VS " + otherWindowBounds);
            if (otherWindowBounds.Intersects(windowComponentBounds))
                return false;
        }
        return true;
    }

}
