using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPointController : MonoBehaviour
{
    [SerializeField]
    RenderplaneBehaviour renderPlane;
    [SerializeField]
    GameObject dotPrefab;
    [SerializeField]
    GameObject renderDotPrefab;
    List<Transform> dots;
    [SerializeField]
    Camera sceneCamera;

    // Start is called before the first frame update
    void Start()
    {
        renderPlane.OnClickEvent += AddDot;
        dots = new List<Transform>();
    }

    void AddDot()
    {
        Debug.Log("Clicked!");
        GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
        GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform.parent.transform);
        DotBehaviour db = dot.GetComponent<DotBehaviour>();
        db.OnDragEvent += MoveDot;
        db.RightClickEvent += RemoveDot;
        db.LeftClickEvent += SelectDot;
        db.renderDot = renderDot;
        dots.Add(dot.transform);
    }

    void MoveDot(DotBehaviour dot)
    {
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
        for (int i = dot.index + 1; i < dots.Count; i++)
        {
            dots[i].GetComponent<DotBehaviour>().index--;
        }
        dots.RemoveAt(dot.index);
        dot.Remove();
    }

    public void SelectDot(DotBehaviour dot)
    {

    }
}
