using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PointController : MonoBehaviour
{
    [SerializeField] 
    GameObject dotPrefab;
    [SerializeField]
    GameObject renderDotPrefab;
    [SerializeField]
    Camera sceneCamera;
    private List<Transform> points;
    [SerializeField]
    SpriteShapeController spriteShapeController;

    private void Start()
    {
        points = new List<Transform>();
    }

    public void AddPointAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 15 && mousePos.x < 1215)
        {
            if (mousePos.y > 440 && mousePos.y < 1010)
            {
                GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
                GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, spriteShapeController.transform);
                DotBehaviour db = dot.GetComponent<DotBehaviour>();
                db.OnDragEvent += MovePoint;
                db.RightClickEvent += RemovePoint;
                db.renderDot = renderDot;
                db.index = points.Count;
                points.Add(dot.transform);
            }
        }
    }

    void MovePoint(DotBehaviour point)
    {
        point.transform.position = GetMouseInWorldSpace();
        point.renderDot.transform.position = point.transform.position;
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    public void RemovePoint(DotBehaviour dot)
    {
        for(int i = dot.index + 1; i < points.Count; i++)
        {
            points[i].GetComponent<DotBehaviour>().index--;
        }
        points.RemoveAt(dot.index);
        dot.Remove();
        
    }

    private void LateUpdate()
    {
        if (points.Count >= 1)
        {
            Spline spline = spriteShapeController.spline;
            spline.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                spline.InsertPointAt(i, points[i].position);
            }
            spriteShapeController.RefreshSpriteShape();
        }
    }
}
