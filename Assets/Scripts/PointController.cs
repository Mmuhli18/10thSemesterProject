using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PointController : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] 
    GameObject dotPrefab;
    [SerializeField]
    GameObject renderDotPrefab;
    [SerializeField]
    Material deSelectedMaterial;
    [SerializeField]
    Material selectedMaterial;
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

    public void AddMarking()
    {
        markings.Add(new ForegroundMarking());
        markings[markings.Count - 1].spriteShapeController = Instantiate(spriteShapeRenderer, viewportHandler.transform).GetComponent<SpriteShapeController>();
        SwitchMarking(markings.Count - 1);
    }

    public void SwitchMarking(int markingIndex)
    {
        if(markingIndex != activeMarking)
        {
            //deselecting current points
            if (activeMarking >= 0)
            {
                for (int i = 0; i < markings[activeMarking].dots.Count; i++)
                {
                    markings[activeMarking].dots[i].gameObject.GetComponent<DotBehaviour>().renderDot.GetComponent<Renderer>().material = deSelectedMaterial;
                }
            }

            if(markingIndex >= 0)
            {
                //selecting new points
                for (int i = 0; i < markings[markingIndex].dots.Count; i++)
                {
                    markings[markingIndex].dots[i].gameObject.GetComponent<DotBehaviour>().renderDot.GetComponent<Renderer>().material = selectedMaterial;
                }
            }
            activeMarking = markingIndex;
        }
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
        if(renderPlane.IsPlaneHovered())
        {
            GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
            GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, markings[activeMarking].spriteShapeController.transform);
            DotBehaviour db = dot.GetComponent<DotBehaviour>();
            db.OnDragEvent += MoveDot;
            db.RightClickEvent += RemoveDot;
            db.LeftClickEvent += SelectDot;
            db.renderDot = renderDot;
            var dots = markings[activeMarking].dots;
            db.index = dots.Count;
            db.markingIndex = activeMarking;
            dots.Add(dot.transform);
            markings[activeMarking].dots = dots;
        }
    }

    void MoveDot(DotBehaviour dot)
    {
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

    public void RemoveDot(DotBehaviour dot)
    {
        SwitchMarking(dot.markingIndex);
        var dots = markings[dot.markingIndex].dots;
        for (int i = dot.index + 1; i < dots.Count; i++)
        {
            dots[i].GetComponent<DotBehaviour>().index--;
        }
        dots.RemoveAt(dot.index);
        dot.Remove();
        markings[dot.markingIndex].dots = dots;
        if (dots.Count < 1) markings[dot.markingIndex].Remove();
    }

    public void SelectDot(DotBehaviour dot)
    {
        SwitchMarking(dot.markingIndex);
    }

    private void LateUpdate()
    {
        if (markings.Count > 0)
        {
            var dots = markings[activeMarking].dots;
            if (dots.Count >= 1)
            {
                Spline spline = markings[activeMarking].spriteShapeController.spline;
                spline.Clear();
                for (int i = 0; i < dots.Count; i++)
                {
                    spline.InsertPointAt(i, dots[i].position);
                }
                markings[activeMarking].spriteShapeController.RefreshSpriteShape();
            }
            markings[activeMarking].dots = dots;
        }
    }

    public class ForegroundMarking
    {
        public List<Transform> dots = new List<Transform>();
        public SpriteShapeController spriteShapeController;

        public void Remove()
        {
            for (int i = 0; i < dots.Count; i++)
            {
                Destroy(dots[i].gameObject);
            }
            Destroy(spriteShapeController.gameObject);
        }

        public void GiveNewIndex(int index)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                dots[i].gameObject.GetComponent<DotBehaviour>().markingIndex = index;
            }
        }
    }
}
