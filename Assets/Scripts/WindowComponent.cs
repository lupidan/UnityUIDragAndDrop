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

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    private RectTransform _compactRectTransform;
    public RectTransform CompactRectTransform { get { return _compactRectTransform; } }

    private RectTransform _expandedRectTransform;
    public RectTransform ExpandedRectTransform { get { return _expandedRectTransform; } }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        bool isOnComponentPanel = RectTransformUtility.RectangleContainsScreenPoint(ComponentPanel.RectTransform, eventData.position, Camera.main);
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GridPanel.RectTransform, eventData.position, Camera.main, out localAnchoredPosition);
        localAnchoredPosition = GridPanel.SnapToGrid(localAnchoredPosition);
        _rectTransform.anchoredPosition = localAnchoredPosition;
        Debug.Log(GridPanel.CanWindowBePlaced(this));
    }

    private void OnDragInComponentPanel(PointerEventData eventData)
    {
        if (Expanded)
            Expanded = false;

        if (_rectTransform.parent != ComponentPanel.RectTransform)
            _rectTransform.SetParent(ComponentPanel.RectTransform);

        Vector2 localAnchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(ComponentPanel.RectTransform, eventData.position, Camera.main, out localAnchoredPosition);
        _rectTransform.anchoredPosition = localAnchoredPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

}
