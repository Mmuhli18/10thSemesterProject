using UnityEngine;
using System;

public class AnnotationDotBehaviour : DotBehaviour
{

    public GameObject renderDot;
    public int index;
    public int markingIndex;
    Vector3 renderDotScale;
    public DotBehaviour crossDot;
    public Action<DotBehaviour> OnHoveredEvent;

    private void Start()
    {
        renderDotScale = renderDot.transform.localScale;
        crossDot.RightClickEvent += CrossPressed;
        crossDot.LeftClickEvent += CrossPressed;
    }

    private void Update()
    {
        if (DotIsHovered())
        {
            renderDot.transform.localScale = renderDotScale * 1.4f;
            crossDot.gameObject.SetActive(true);
        }
        else if (crossDot.DotIsHovered())
        {
            renderDot.transform.localScale = renderDotScale;
            crossDot.gameObject.SetActive(true);
        }
        else
        {
            renderDot.transform.localScale = renderDotScale;
            crossDot.gameObject.SetActive(false);
        }
        
    }

    void CrossPressed(DotBehaviour dot)
    {
        RightClickEvent.Invoke(this);
    }

    public virtual void Remove()
    {
        Destroy(renderDot);
        Destroy(gameObject);
    }
}
