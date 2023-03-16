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
            pointController.AddPointAtMouse();
    }
}
