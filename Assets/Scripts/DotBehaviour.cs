using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class DotBehaviour : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public Action<DotBehaviour> OnDragEvent;
    public Action<DotBehaviour> LeftClickEvent;
    public Action<DotBehaviour> RightClickEvent;
    public Action<DotBehaviour> OnHoveredEvent;
    public GameObject renderDot;
    public int index;
    public int markingIndex;
    Vector3 renderDotScale;
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

    private void Start()
    {
        renderDotScale = renderDot.transform.localScale;
    }

    private void Update()
    {
        if (DotIsHovered())
        {
            renderDot.transform.localScale = renderDotScale * 1.4f;
        }
        else
        {
            renderDot.transform.localScale = renderDotScale;
        }
        
    }

    bool DotIsHovered()
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

    public virtual void Remove()
    {
        Destroy(renderDot);
        Destroy(gameObject);
    }
}
