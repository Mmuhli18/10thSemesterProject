using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class RoadPiece : MonoBehaviour
{
    public float lengthRatio = 1f;
    public RoadDotBehaviour startDot;
    public RoadDotBehaviour endDot;
    public Vector3 tempEndPos;
    public RoadSetting settings;
    [SerializeField]
    GameObject roadMeshPrefab;
    [HideInInspector] 
    public LineRenderer lineRenderer;
    [HideInInspector]
    public MeshFilter roadMesh;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Remove()
    {
        startDot.connectedRoads.Remove(this);
        if(endDot != null) endDot.connectedRoads.Remove(this);
        Destroy(roadMesh.gameObject);
        Destroy(gameObject);
    }

    public void DrawLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startDot.transform.position);
        lineRenderer.SetPosition(1, endDot.transform.position);
        if (roadMesh == null)
        {
            roadMesh = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            roadMesh.transform.position -= new Vector3(0f, 0f, -1f);
        }
        MeshFace face = MeshDrawer.combineFaces(MeshDrawer.extrudeFace(FaceFromRoadPiece(), 1f, new Vector3(0f, 0f, -1f)));
        roadMesh.mesh = MeshDrawer.makeMesh(face);
    }

    public void DrawLine(Vector3 endPos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startDot.transform.position);
        lineRenderer.SetPosition(1, endPos);
        tempEndPos = endPos;
        if (roadMesh == null)
        {
            roadMesh = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            roadMesh.transform.position -= new Vector3(0f, 0f, -1f);
        }
        roadMesh.mesh = MeshDrawer.makeMesh(MeshDrawer.combineFaces(MeshDrawer.extrudeFace(FaceFromRoadPiece(), 1f, new Vector3(0f, 1f, 0f))));
    }

    MeshFace FaceFromRoadPiece()
    {
        Vector3 startpoint = startDot.transform.position;
        Vector3 endpoint;
        if (endDot == null) endpoint = tempEndPos;
        else endpoint = endDot.transform.position;
        Vector3 direction = (startpoint - endpoint).normalized;
        direction = Quaternion.AngleAxis(90f, Vector3.back) * direction;

        List<Vector3> corners = new List<Vector3>();
        Vector3 offset1 = direction.normalized * settings.width;
        Vector3 offset2 = -direction.normalized * settings.width;
        Vector3 movement = new Vector3(20f, 20f, 0f);
        corners.Add(new Vector3(startpoint.x, startpoint.y, 0f) + offset1 + movement);
        corners.Add(new Vector3(startpoint.x, startpoint.y, 0f) + offset2 + movement);
        corners.Add(new Vector3(endpoint.x, endpoint.y, 0f) + offset2 + movement);
        corners.Add(new Vector3(endpoint.x, endpoint.y, 0f) + offset1 + movement);

        return MeshDrawer.drawFace(corners, true);
    }
}

[Serializable]
public class RoadSetting
{
    public float width = 30f;
    public bool isOneWay = false;
    public bool hasBikeLanes = false;
    public bool hasSideWalks = false;
}
