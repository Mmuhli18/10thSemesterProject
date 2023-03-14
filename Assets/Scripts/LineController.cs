using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private List<Transform> points;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Camera renderCam;
    public List<Transform> testpoints;

    private void Start()
    {
        points = new List<Transform>();
        lr.positionCount = 0;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void AddPointAtMouse()
    {
        GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
        lr.positionCount++;
        points.Add(dot.transform);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(Input.mousePosition);
            AddPointAtMouse();
        }
        if (Input.GetMouseButtonDown(1))
        { 
            
        }
    }

    Vector3 GetMouseInWorldSpace()
    {
        return renderCam.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    private void LateUpdate()
    {
        if(points.Count >= 2)
        {
            for(int i = 0; i < points.Count; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
    }
}
