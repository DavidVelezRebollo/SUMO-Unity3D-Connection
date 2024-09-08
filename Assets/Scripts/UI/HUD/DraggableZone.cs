using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableZone : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform TransformToMove;

    private RectTransform _draggableRect;
    private Camera _mainCamera;
    private bool _clicking;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _draggableRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!_clicking) return;

        if (!Input.GetMouseButton(0))
        {
            _clicking = false;
            return;
        }
        
        Vector3 pos = Input.mousePosition;
        // pos.x -= _draggableRect.rect.width / 2;
        // pos.y += _draggableRect.rect.height / 2;
        TransformToMove.position = pos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _clicking = true;
    }
}
