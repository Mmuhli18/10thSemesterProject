using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private List<Transform> points;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Camera sceneCamera;
    public List<Transform> testpoints;
    private SpriteShapeController spriteShapeController;
    [SerializeField] PointController pointController;

    private void Start()
    {
        points = new List<Transform>();
        lr.positionCount = 0;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spriteShapeController = GetComponent<SpriteShapeController>();
    }

    public void AddPointAtMouse()
    {
        /*Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 15 && mousePos.x < 1215)
        {
            if(mousePos.y > 440 && mousePos.y < 1010)
            {*/
                pointController.AddPointAtMouse();
                GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
                dot.GetComponent<DotBehaviour>().OnDragEvent += MovePoint;
                lr.positionCount++;
                points.Add(dot.transform);
       //     }
       // }
    }

    public void RemoveLastPoint()
    {
        Destroy(points[points.Count - 1].gameObject);
        points.RemoveAt(points.Count - 1);
        lr.positionCount--;
    }

    public void MovePoint(DotBehaviour point)
    {
        point.transform.position = GetMouseInWorldSpace();
    }



    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    private void LateUpdate()
    {
        if(points.Count >= 2)
        {
            Spline spline = spriteShapeController.spline;
            spline.Clear();
            for(int i = 0; i < points.Count; i++)
            {
                //lr.SetPosition(i, points[i].position);
                spline.InsertPointAt(i, points[i].position);
            }
            spriteShapeController.RefreshSpriteShape();
        }
    }

    
}
