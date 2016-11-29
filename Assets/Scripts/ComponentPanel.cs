using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComponentPanel : MonoBehaviour
{

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    void Awake()
    {
        _rectTransform = transform as RectTransform;
    }
}
