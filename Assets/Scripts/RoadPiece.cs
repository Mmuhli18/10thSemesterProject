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
    public RoadSettingHolder settings;
    [SerializeField]
    GameObject roadMeshPrefab;
    [HideInInspector] 
    public LineRenderer lineRenderer;
    [HideInInspector]
    public MeshFilter roadMesh;
    MeshFilter sideMeshRight;
    MeshFilter sideMeshLeft;

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
        MeshFace roadFace = FaceFromRoadPiece();
        if (roadMesh == null)
        {
            roadMesh = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();

            sideMeshLeft = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            sideMeshLeft.transform.position += settings.roadDirection.normalized * settings.width;
            sideMeshLeft.transform.parent = roadMesh.transform;

            sideMeshRight = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            sideMeshRight.transform.position -= settings.roadDirection.normalized * settings.width;
            sideMeshRight.transform.parent = roadMesh.transform;

            roadMesh.transform.position -= new Vector3(0f, 0f, -1f);
        }
        MeshFace face = MeshDrawer.combineFaces(MeshDrawer.extrudeFace(roadFace, 1f, new Vector3(0f, 0f, -1f)));
        roadMesh.mesh = MeshDrawer.makeMesh(face);
    }

    public void DrawLine(Vector3 endPos)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startDot.transform.position);
        lineRenderer.SetPosition(1, endPos);
        tempEndPos = endPos;
        MeshFace roadFace = FaceFromRoadPiece();
        if (roadMesh == null)
        {
            roadMesh = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();

            sideMeshLeft = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            sideMeshLeft.transform.position += settings.roadDirection.normalized * settings.width;
            sideMeshLeft.transform.parent = roadMesh.transform;

            sideMeshRight = Instantiate(roadMeshPrefab).GetComponent<MeshFilter>();
            sideMeshRight.transform.position -= settings.roadDirection.normalized * settings.width;
            sideMeshRight.transform.parent = roadMesh.transform;

            roadMesh.transform.position -= new Vector3(0f, 0f, -1f);
        }
        roadMesh.mesh = MeshDrawer.makeMesh(MeshDrawer.combineFaces(MeshDrawer.extrudeFace(roadFace, 1f, new Vector3(0f, 1f, 0f))));
        sideMeshLeft.mesh = MeshDrawer.makeMesh(MeshDrawer.combineFaces(MeshDrawer.extrudeFace(SideWalkFaceFromRoadPiece(-1), 2f, new Vector3(0f, 1f, 0f))));
        sideMeshRight.mesh = MeshDrawer.makeMesh(MeshDrawer.combineFaces(MeshDrawer.extrudeFace(SideWalkFaceFromRoadPiece(1), 2f, new Vector3(0f, 1f, 0f))));
    }

    MeshFace FaceFromRoadPiece()
    {
        settings.startPoint = startDot.transform.position;
        if (endDot == null) settings.endPoint = tempEndPos;
        else settings.endPoint = endDot.transform.position;
        Vector3 direction = (settings.startPoint - settings.endPoint).normalized;
        settings.roadDirection = Quaternion.AngleAxis(90f, Vector3.back) * direction;

        List<Vector3> corners = new List<Vector3>();
        Vector3 offset1 = settings.roadDirection.normalized * settings.width;
        Vector3 offset2 = -settings.roadDirection.normalized * settings.width;
        Vector3 movement = new Vector3(20f, 20f, 0f);
        corners.Add(new Vector3(settings.startPoint.x, settings.startPoint.y, 0f) + offset1 + movement);
        corners.Add(new Vector3(settings.startPoint.x, settings.startPoint.y, 0f) + offset2 + movement);
        corners.Add(new Vector3(settings.endPoint.x, settings.endPoint.y, 0f) + offset2 + movement);
        corners.Add(new Vector3(settings.endPoint.x, settings.endPoint.y, 0f) + offset1 + movement);

        return MeshDrawer.drawFace(corners, true);
    }

    MeshFace SideWalkFaceFromRoadPiece(int direction)
    {
        Vector3 sideWalkOffset = settings.roadDirection.normalized * settings.width * direction;

        List<Vector3> corners = new List<Vector3>();
        Vector3 offset1 = settings.roadDirection.normalized * settings.SideWalkWidth;
        Vector3 offset2 = -settings.roadDirection.normalized * settings.SideWalkWidth;
        Vector3 movement = new Vector3(20f, 20f, 0f);
        corners.Add(new Vector3(settings.startPoint.x, settings.startPoint.y, 0f) + offset1 + movement + sideWalkOffset);
        corners.Add(new Vector3(settings.startPoint.x, settings.startPoint.y, 0f) + offset2 + movement + sideWalkOffset);
        corners.Add(new Vector3(settings.endPoint.x, settings.endPoint.y, 0f) + offset2 + movement + sideWalkOffset);
        corners.Add(new Vector3(settings.endPoint.x, settings.endPoint.y, 0f) + offset1 + movement + sideWalkOffset);

        return MeshDrawer.drawFace(corners, true);
    }
}

[Serializable]
public class RoadSettingHolder
{
    public float width = 30f;
    public bool isOneWay = false;
    public bool hasBikeLanes = false;
    public float BikeLaneWidth = 10f;
    public bool hasSideWalks = false;
    public float SideWalkWidth = 10f;
    public Vector3 roadDirection = new Vector3(1f, 0f, 0f);
    public Vector3 startPoint;
    public Vector3 endPoint;
}
