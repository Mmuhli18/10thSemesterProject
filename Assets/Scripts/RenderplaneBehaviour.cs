using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderplaneBehaviour : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    PointController pointController;
    [SerializeField]
    ViewportHandler viewportHandler;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (viewportHandler.isFootageLoaded && eventData.button == PointerEventData.InputButton.Left)
            pointController.AddDotAtMouse();
    }

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
}
