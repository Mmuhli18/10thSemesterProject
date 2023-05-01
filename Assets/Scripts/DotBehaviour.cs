using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DotBehaviour : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public Action<DotBehaviour> OnDragEvent;
    public Action<DotBehaviour> LeftClickEvent;
    public Action<DotBehaviour> RightClickEvent;
    

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftClickEvent?.Invoke(this);
        else if (eventData.button == PointerEventData.InputButton.Right)
            RightClickEvent?.Invoke(this);
    }

    public bool DotIsHovered()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        for (int i = 0; i < raysastResults.Count; i++)
        {
            if (raysastResults[i].gameObject == gameObject) return true;
        }
        return false;
    }
}
