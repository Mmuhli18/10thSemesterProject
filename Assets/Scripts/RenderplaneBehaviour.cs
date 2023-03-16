using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderplaneBehaviour : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    //LineController lineController;
    PointController pointController;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            pointController.AddPointAtMouse();
    }
}
