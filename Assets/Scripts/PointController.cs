using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;

/* Our main controller class for our annotation system. The system uses markings that are defined by dots. 
 * Dots are manipulated using mouse inputs from the user, based on the dots, markings are drawn using sprite renderers
 * The PointControllers function is to keep track of markings and manipulate the dots.
 */
public class PointController : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] 
    GameObject dotPrefab;
    [SerializeField]
    GameObject renderDotPrefab;
    [SerializeField]
    Material selected;
    [SerializeField]
    Material deSelected;
    [SerializeField]
    GameObject previewDot;
    [Header("Other")]
    [SerializeField]
    Camera sceneCamera;
    [SerializeField]
    ViewportHandler viewportHandler;
    [SerializeField]
    RenderplaneBehaviour renderPlane;
    [Header("Markings")]
    [SerializeField]
    GameObject spriteShapeRenderer;
    public List<ForegroundMarking> markings = new List<ForegroundMarking>();
    public int activeMarking = -1;
    public bool inDrawingMode = false;
    public Action<bool> drawingSwitchEvent;
    //statics
    static Material deSelectedMaterial;
    static Material selectedMaterial;
    private void Start()
    {
        renderPlane.OnClickEvent += AddDotAtMouse;
        deSelectedMaterial = deSelected;
        selectedMaterial = selected;
    }

    //Used for controlling the preview dot, this should only display when in drawing mode and not already hovering a dot
    private void Update()
    {
        if (inDrawingMode) previewDot.transform.position = GetMouseInWorldSpace();

        if (renderPlane.IsOnlyPlaneHovered() && inDrawingMode) previewDot.SetActive(true);
        else previewDot.SetActive(false);
    }

    //Used to switch whatever the current drawing mode is
    public void SwitchDrawingMode()
    {
        if(viewportHandler.isFootageLoaded) SetDrawingMode(!inDrawingMode);
    }

    //Used to enable or disable the drawing mode
    public void SetDrawingMode(bool mode)
    {
        inDrawingMode = mode;
        if (drawingSwitchEvent.Method != null) drawingSwitchEvent.Invoke(inDrawingMode);
        if (mode && markings.Count < 1) AddMarking();
        else markings[activeMarking].Select();
        if (!mode)
        {
            markings[activeMarking].DeSelect();
            previewDot.SetActive(false);
        }
        else previewDot.SetActive(true);
    }

    //Adds a new marking
    public void AddMarking()
    {
        markings.Add(new ForegroundMarking());
        markings[markings.Count - 1].spriteShapeController = Instantiate(spriteShapeRenderer, viewportHandler.transform).GetComponent<SpriteShapeController>();
        markings[markings.Count - 1].spriteShapeRenderer = markings[markings.Count - 1].spriteShapeController.gameObject.GetComponent<SpriteShapeRenderer>();
        SwitchMarking(markings.Count - 1);
    }

    //Used for switching the active marking
    public void SwitchMarking(int markingIndex)
    {
        //deselecting current points
        if (activeMarking >= 0)
        {
            markings[activeMarking].DeSelect();
        }

        if(markingIndex >= 0)
        {
            //selecting new points
            markings[markingIndex].Select();
        }
        activeMarking = markingIndex;
        SetDrawingMode(true);
    }

    //Unused function to clean up in the markings made by the user, these days we just let the chaos insue if the user spams markings
    public void RemoveMarking(int markingIndex)
    {
        for(int i = markingIndex + 1; i < markings.Count; i++)
        {
            markings[i].GiveNewIndex(i - 1);
        }
        markings[markingIndex].Remove();
        markings.RemoveAt(markingIndex);
        SwitchMarking(-1);
    }

    /* When the user clicks the renderplane and drawing is activated a dot is instanciated, assigned functions to 
     * its event actions, and added to the active marking.
     */
    public void AddDotAtMouse()
    {
        if (markings.Count < 1) return;
        if (activeMarking < 0) return;
        if (!inDrawingMode) return;
        if(renderPlane.IsPlaneHovered())
        {
            GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
            // Renderdot is the red part of the dot, this is placed on a different hiercy, this is the result of a previous implementation
            // As we now only use one camera it is less important, but does give nice feedback for the user
            GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, markings[activeMarking].spriteShapeController.transform);
            AnnotationDotBehaviour db = dot.GetComponent<AnnotationDotBehaviour>();
            db.OnDragEvent += MoveDot;
            db.RightClickEvent += RemoveDot;
            db.LeftClickEvent += SelectDot;
            db.renderDot = renderDot;
            db.markingIndex = activeMarking;
            markings[activeMarking].dots.Add(dot.transform);
            //We want to enable the sprite renderer if there are two or more dots
            if (markings[activeMarking].dots.Count > 1) markings[activeMarking].spriteShapeRenderer.enabled = true;
        }
    }

    /* When a user starts dragging a dot, will get the position of the mouse for as long as it is over the renderplane
     * and then place the dot at this position
     */
    void MoveDot(DotBehaviour d)
    {
        AnnotationDotBehaviour dot = d as AnnotationDotBehaviour;
        SwitchMarking(dot.markingIndex);
        if (renderPlane.IsPlaneHovered())
        {
            dot.transform.position = GetMouseInWorldSpace();
            dot.renderDot.transform.position = dot.transform.position;
        }
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    //When a user right-clicks a dot or the corss this is activated. 
    public void RemoveDot(DotBehaviour d)
    {
        AnnotationDotBehaviour dot = d as AnnotationDotBehaviour;
        SwitchMarking(dot.markingIndex);
        markings[dot.markingIndex].dots.Remove(dot.transform);
        dot.Remove();
        //We want to disable the sprite renderer if there are less that two dots, as this gives interesting results otherwise
        if (markings[dot.markingIndex].dots.Count < 2) markings[dot.markingIndex].spriteShapeRenderer.enabled = false;
    }

    //When a user clicks a dot, this will make the dot and its marking the selected marking
    public void SelectDot(DotBehaviour d)
    {
        AnnotationDotBehaviour dot = d as AnnotationDotBehaviour;
        SwitchMarking(dot.markingIndex);
    }

    //Shapes are drawn in late update to make use all splines where updated
    private void LateUpdate()
    {
        if (markings.Count > 0)
        {
            markings[activeMarking].DrawShape();
        }
    }

    /* Class for storing and controlling the dots and information for a marking
     * This includes drawing it shape and highlighting dots
     */
    public class ForegroundMarking
    {
        public List<Transform> dots = new List<Transform>();
        public SpriteShapeController spriteShapeController;
        public SpriteShapeRenderer spriteShapeRenderer;

        //Deletes any assosiated dots and then itself
        public void Remove()
        {
            for (int i = 0; i < dots.Count; i++)
            {
                Destroy(dots[i].gameObject);
            }
            Destroy(spriteShapeController.gameObject);
        }

        //Highlights the dots for the marking
        public void Select()
        {
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.GetComponent<Renderer>().material = selectedMaterial;
            }
        }

        //Stop highlighting the dots for the marking
        public void DeSelect() 
        {
            for(int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.GetComponent<Renderer>().material = deSelectedMaterial;
            }
        }

        //Updates the spline for the spriteShapeController and refreshes the shape
        public void DrawShape()
        {
            if (dots.Count >= 1)
            {
                Spline spline = spriteShapeController.spline;
                spline.Clear();
                for (int i = 0; i < dots.Count; i++)
                {
                    spline.InsertPointAt(i, dots[i].position);
                }
                spriteShapeController.RefreshSpriteShape();
            }
        }

        //To identify what marking a dot is connected to they have a marking index, this is updated here
        public void GiveNewIndex(int index)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().markingIndex = index;
            }
        }
    }
}
