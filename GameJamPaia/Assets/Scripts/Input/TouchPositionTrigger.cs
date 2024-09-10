using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPositionTrigger :InputScript,IPointerDownHandler,IDragHandler,IPointerUpHandler,IHasActiveState
{
    [SerializeField] private bool active = true;

    public Action<bool> OnTouchScreen;

    private Vector2 startPoint;

    private Vector2 endPoint;

    public void OnPointerDown(PointerEventData eventData)
    {
      
        if (active == false)
            return;

        startPoint = eventData.position;

        OnTouchScreen?.Invoke(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if (active == false)
            return;

        endPoint = eventData.position;

        inputDirection = endPoint - startPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (active == false)
            return;

        inputDirection = Vector2.zero;

        OnTouchScreen?.Invoke(false);
    }

    public Vector2 GetStartPoint()
    {
        return startPoint;
    }

    public Vector2 GetEndPoint()
    {
        return endPoint;
    }

    public void Disable()
    {
        active = false;
    }

    public void Enable()
    {
        active = true;
    }
}
