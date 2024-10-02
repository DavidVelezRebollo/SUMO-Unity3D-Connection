using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableZone : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform TransformToMove;

    private Vector3 _initialMousePos;
    private Vector3 _initialWindowPos;
    private bool _clicking;

    private void Update()
    {
        if (!_clicking) return;

        if (!Input.GetMouseButton(0))
        {
            _clicking = false;
            return;
        }

        Vector3 currentPos = Input.mousePosition;
        Vector3 delta = currentPos - _initialMousePos;
        TransformToMove.position = _initialWindowPos + delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _clicking = true;
        _initialMousePos = Input.mousePosition;
        _initialWindowPos = TransformToMove.position;
    }
}
