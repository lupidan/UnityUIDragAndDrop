using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Vector2 _beginDragOffset;
    private GridComponent _gridComponent;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _gridComponent = FindObjectOfType<GridComponent>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        _beginDragOffset = _rectTransform.anchoredPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newAnchoredPosition = eventData.position + _beginDragOffset;
        for (int i = 0; i < 2; i++)
        {
            if (_gridComponent.GridSize[i] <= 0.0f)
                continue;

            int multiple = Mathf.RoundToInt(newAnchoredPosition[i] / _gridComponent.GridSize[i]);
            newAnchoredPosition[i] = _gridComponent.GridSize[i] * multiple;
        }
        _rectTransform.anchoredPosition = newAnchoredPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _beginDragOffset = Vector2.zero;
    }

}
