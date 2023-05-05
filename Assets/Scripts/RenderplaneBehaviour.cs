using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

/* The behaviour we assign the plane registering the mouse in the viewport. 
 * Its behaviour is to register when the user clicks it, or hovers their mouse over it
 */
public class RenderplaneBehaviour : MonoBehaviour, IPointerClickHandler
{
    public Action OnClickEvent;
    public Action OnRightClickEvent;
    [SerializeField]
    ViewportHandler viewportHandler;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (viewportHandler.isFootageLoaded && eventData.button == PointerEventData.InputButton.Left)
            OnClickEvent?.Invoke();
        if (viewportHandler.isFootageLoaded && eventData.button == PointerEventData.InputButton.Right)
            OnRightClickEvent?.Invoke();
    }

    //Checking if the mouse is hovering aboce the plane
    public bool IsPlaneHovered()
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

    //Checking specificly if only the plane is hovered, will return false if there is a dot between the mouse and plane
    public bool IsOnlyPlaneHovered()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        if(raysastResults.Count > 0)
            if (raysastResults[0].gameObject == gameObject) return true;
        return false;
    }
}
