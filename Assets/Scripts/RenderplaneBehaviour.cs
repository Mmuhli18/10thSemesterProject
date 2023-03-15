using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderplaneBehaviour : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    LineController lineController;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click!");
        if (eventData.button == PointerEventData.InputButton.Left)
            lineController.AddPointAtMouse();
        else if (eventData.button == PointerEventData.InputButton.Right)
            lineController.RemoveLastPoint();
    }
}
