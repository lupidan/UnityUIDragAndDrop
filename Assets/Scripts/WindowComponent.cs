using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform CompactWindow;
    public RectTransform ExpandedWindow;
    public GridPanel GridPanel;
    public ComponentPanel ComponentPanel;
    
    public bool _expanded = false;
    public bool Expanded {
        get {
            return _expanded;
        }
        set
        {
            _expanded = value;
            UpdateActiveObjects();
        }
    }

    private CanvasGroup _canvasGroup;
    private Canvas _parentCanvas;
    private RectTransform _parentCanvasRectTransform;

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    private RectTransform _compactRectTransform;
    public RectTransform CompactRectTransform { get { return _compactRectTransform; } }

    private RectTransform _expandedRectTransform;
    public RectTransform ExpandedRectTransform { get { return _expandedRectTransform; } }

    private Vector2 _compactDragOffset;
    private Vector2 _expandedDragOffset;
    private RectTransform _beginDragParent;
    private Vector2 _beginDragAnchoredPosition;


    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _compactRectTransform = CompactWindow.GetComponent<RectTransform>();
        _expandedRectTransform = ExpandedWindow.GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        _parentCanvasRectTransform = _parentCanvas.transform as RectTransform;
        UpdateActiveObjects();
    }

    private void UpdateActiveObjects()
    {
        CompactWindow.gameObject.SetActive(!_expanded);
        ExpandedWindow.gameObject.SetActive(_expanded);
        _canvasGroup.alpha = 1.0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.SetAsLastSibling();
        _beginDragParent = _rectTransform.parent as RectTransform;
        _beginDragAnchoredPosition = _rectTransform.anchoredPosition;

        Vector2 dragOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out dragOffset);
        _compactDragOffset =  Expanded ? CompactRectTransform.rect.center : dragOffset;
        _expandedDragOffset = Expanded ? dragOffset : ExpandedRectTransform.rect.center;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 clampedPosition = eventData.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0.0f, _parentCanvasRectTransform.rect.width);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0.0f, _parentCanvasRectTransform.rect.height);

        bool isOnComponentPanel = RectTransformUtility.RectangleContainsScreenPoint(ComponentPanel.RectTransform, clampedPosition, eventData.pressEventCamera);
        
        bool shouldBeExpanded = !isOnComponentPanel;
        if (Expanded != shouldBeExpanded)
            Expanded = shouldBeExpanded;
        
        RectTransform windowParent = isOnComponentPanel ? ComponentPanel.RectTransform : GridPanel.RectTransform;
        if (_rectTransform.parent != windowParent)
            _rectTransform.SetParent(windowParent);

        Vector2 localAnchoredPosition;
        Vector2 dragOffset = isOnComponentPanel ? _compactDragOffset : _expandedDragOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(windowParent, clampedPosition, eventData.pressEventCamera, out localAnchoredPosition);
        _rectTransform.anchoredPosition = localAnchoredPosition - dragOffset;

        if (isOnComponentPanel)
        {
            ComponentPanel.ClampInsideGridPanel(this);
        }
        else
        {
            GridPanel.SnapToGrid(this);
            GridPanel.ClampInsideGridPanel(this);
            bool windowCanBePlaced = GridPanel.CanWindowBePlaced(this);
            _canvasGroup.alpha = windowCanBePlaced ? 1.0f : 0.5f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       _compactDragOffset = Vector2.zero;
       _expandedDragOffset = Vector2.zero;
        if (Expanded && !GridPanel.CanWindowBePlaced(this))
        {
            _rectTransform.SetParent(_beginDragParent);
            _rectTransform.anchoredPosition = _beginDragAnchoredPosition;
            Expanded = _beginDragParent == GridPanel.RectTransform;
        }
    }

}
