using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RoadPiece : MonoBehaviour
{
    public RoadDotBehaviour startDot;
    public RoadDotBehaviour endDot;
    [HideInInspector] 
    public LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Remove()
    {
        startDot.connectedRoads.Remove(this);
        endDot.connectedRoads.Remove(this);
        Destroy(gameObject);
    }

    public void DrawLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startDot.transform.position);
        lineRenderer.SetPosition(1, endDot.transform.position);
    }

    public void DrawMouseLine(Vector3 mousePos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startDot.transform.position);
        lineRenderer.SetPosition(1, mousePos);
    }
}
