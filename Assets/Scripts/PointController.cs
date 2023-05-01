using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;

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

    private void Update()
    {
        if (inDrawingMode) previewDot.transform.position = GetMouseInWorldSpace();

        if (renderPlane.IsOnlyPlaneHovered() && inDrawingMode) previewDot.SetActive(true);
        else previewDot.SetActive(false);
    }

    public void SwitchDrawingMode()
    {
        if(viewportHandler.isFootageLoaded) SetDrawingMode(!inDrawingMode);
    }

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

    public void AddMarking()
    {
        markings.Add(new ForegroundMarking());
        markings[markings.Count - 1].spriteShapeController = Instantiate(spriteShapeRenderer, viewportHandler.transform).GetComponent<SpriteShapeController>();
        markings[markings.Count - 1].spriteShapeRenderer = markings[markings.Count - 1].spriteShapeController.gameObject.GetComponent<SpriteShapeRenderer>();
        SwitchMarking(markings.Count - 1);
    }

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

    public void AddDotAtMouse()
    {
        if (markings.Count < 1) return;
        if (activeMarking < 0) return;
        if (!inDrawingMode) return;
        if(renderPlane.IsPlaneHovered())
        {
            GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
            GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, markings[activeMarking].spriteShapeController.transform);
            AnnotationDotBehaviour db = dot.GetComponent<AnnotationDotBehaviour>();
            db.OnDragEvent += MoveDot;
            db.RightClickEvent += RemoveDot;
            db.LeftClickEvent += SelectDot;
            db.renderDot = renderDot;
            var dots = markings[activeMarking].dots;
            db.index = dots.Count;
            db.markingIndex = activeMarking;
            dots.Add(dot.transform);
            markings[activeMarking].dots = dots;
            if (dots.Count > 1) markings[activeMarking].spriteShapeRenderer.enabled = true;
        }
    }

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

    public void RemoveDot(DotBehaviour d)
    {
        AnnotationDotBehaviour dot = d as AnnotationDotBehaviour;
        SwitchMarking(dot.markingIndex);
        var dots = markings[dot.markingIndex].dots;
        for (int i = dot.index + 1; i < dots.Count; i++)
        {
            dots[i].GetComponent<AnnotationDotBehaviour>().index--;
        }
        dots.RemoveAt(dot.index);
        dot.Remove();
        markings[dot.markingIndex].dots = dots;
        if (dots.Count < 2) markings[dot.markingIndex].spriteShapeRenderer.enabled = false;
    }

    public void SelectDot(DotBehaviour d)
    {
        AnnotationDotBehaviour dot = d as AnnotationDotBehaviour;
        SwitchMarking(dot.markingIndex);
    }

    private void LateUpdate()
    {
        if (markings.Count > 0)
        {
            markings[activeMarking].DrawShape();
        }
    }

    public class ForegroundMarking
    {
        public List<Transform> dots = new List<Transform>();
        public SpriteShapeController spriteShapeController;
        public SpriteShapeRenderer spriteShapeRenderer;

        public void Remove()
        {
            for (int i = 0; i < dots.Count; i++)
            {
                Destroy(dots[i].gameObject);
            }
            Destroy(spriteShapeController.gameObject);
        }

        public void Select()
        {
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.GetComponent<Renderer>().material = selectedMaterial;
            }
        }

        public void DeSelect() 
        {
            for(int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().renderDot.GetComponent<Renderer>().material = deSelectedMaterial;
            }
        }

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

        public void GiveNewIndex(int index)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<AnnotationDotBehaviour>().markingIndex = index;
            }
        }
    }
}
