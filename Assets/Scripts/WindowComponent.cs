using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2 CompactSize;
    public Vector2 ExpandedSize;
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
            _rectTransform.sizeDelta = _expanded ? ExpandedSize : CompactSize;
            _rectTransform.pivot = _expanded ? new Vector2(0.0f, 1.0f) : new Vector2(0.5f, 0.5f);
        }
    }

    private RectTransform _rectTransform;
    private DragAndDropMenuController _controller;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        _controller = FindObjectOfType<DragAndDropMenuController>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_controller.DraggedWindowComponent != null)
            return;

        _controller.DraggedWindowComponent = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_controller.DraggedWindowComponent != this)
            return;

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
        if (_controller.DraggedWindowComponent != this)
            return;

        _controller.DraggedWindowComponent = null;
    }

}
