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

        if (Expanded)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _expandedDragOffset);
            _compactDragOffset = CompactRectTransform.rect.center;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _compactDragOffset);
            _expandedDragOffset = ExpandedRectTransform.rect.center;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        bool isOnComponentPanel = RectTransformUtility.RectangleContainsScreenPoint(ComponentPanel.RectTransform, eventData.position, eventData.pressEventCamera);
        if (isOnComponentPanel)
            OnDragInComponentPanel(eventData);
        else
            OnDragInGridPanel(eventData);
    }

    private void OnDragInGridPanel(PointerEventData eventData)
    {
        if (!Expanded)
            Expanded = true;

        if (_rectTransform.parent != GridPanel.RectTransform)
            _rectTransform.SetParent(GridPanel.RectTransform);

        Vector2 localAnchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GridPanel.RectTransform, eventData.position, eventData.pressEventCamera, out localAnchoredPosition);
        _rectTransform.anchoredPosition = localAnchoredPosition - _expandedDragOffset;

        GridPanel.SnapToGrid(this);
        GridPanel.ClampInsideGridPanel(this);
        bool windowCanBePlaced = GridPanel.CanWindowBePlaced(this);
        _canvasGroup.alpha = windowCanBePlaced ? 1.0f : 0.5f;

    }

    private void OnDragInComponentPanel(PointerEventData eventData)
    {
        if (Expanded)
            Expanded = false;

        if (_rectTransform.parent != ComponentPanel.RectTransform)
            _rectTransform.SetParent(ComponentPanel.RectTransform);

        Vector2 localAnchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ComponentPanel.RectTransform, eventData.position, eventData.pressEventCamera, out localAnchoredPosition);
        _rectTransform.anchoredPosition = localAnchoredPosition - _compactDragOffset;
        ComponentPanel.ClampInsideGridPanel(this);
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
