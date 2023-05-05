using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

/* Our class for controlling dot on a canvas. Main functionallity is in registering events and the 
 * proc'ing event actions corrosponding to the events. This is used in the annotation, both for the main dot
 * and for the cross dots which both use the behaviours. The main dots use a sub-class for their specific behaviour
 */
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

    // A ray is sent via the event system, it is then registered if the gameobject is hit
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
