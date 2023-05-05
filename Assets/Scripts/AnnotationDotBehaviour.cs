using UnityEngine;

/* A sub-class of the DotBehaviour that is used by the main dot in annotation.
 */
public class AnnotationDotBehaviour : DotBehaviour
{

    public GameObject renderDot;
    public int markingIndex;
    Vector3 renderDotScale;
    public DotBehaviour crossDot;

    private void Start()
    {
        renderDotScale = renderDot.transform.localScale;
        crossDot.RightClickEvent += CrossPressed;
        crossDot.LeftClickEvent += CrossPressed;
    }

    private void Update()
    {
        //If main dot is hovered highlight the dot and enable the cross dot
        if (DotIsHovered())
        {
            renderDot.transform.localScale = renderDotScale * 1.4f;
            crossDot.gameObject.SetActive(true);
        }
        //If the cross dot is hovered keep the cross dot enabled, but don't highlight the main dot
        else if (crossDot.DotIsHovered())
        {
            renderDot.transform.localScale = renderDotScale;
            crossDot.gameObject.SetActive(true);
        }
        //If nothing is hovered, do not highlight dot and disable cross dot
        else
        {
            renderDot.transform.localScale = renderDotScale;
            crossDot.gameObject.SetActive(false);
        }
        
    }
    //If the cross is pressed, simply use RightClick behaviour which deletes the dot
    void CrossPressed(DotBehaviour dot)
    {
        RightClickEvent.Invoke(this);
    }

    //Used for removing this and assosiated gameobjects
    public virtual void Remove()
    {
        Destroy(renderDot);
        Destroy(gameObject);
    }
}
