using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField]
    GameObject lineRendererPrefab;
    bool roadInProgress = false;
    RoadPiece activeRoadPiece;
    List<RoadPiece> roadPieces = new List<RoadPiece>();

    // Start is called before the first frame update
    void Start()
    {
        renderPlane.OnClickEvent += AddDot;
        renderPlane.OnRightClickEvent += DeSelectDot;
        dots = new List<Transform>();
    }

    private void Update()
    {
        if (roadInProgress)
        {
            activeRoadPiece.DrawLine(GetMouseInWorldSpace());
        }
    }

    void AddDot()
    {
        GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
        GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform.parent.transform);
        DotBehaviour db = dot.GetComponent<DotBehaviour>();
        db.OnDragEvent += MoveDot;
        db.RightClickEvent += RemoveDot;
        db.LeftClickEvent += SelectDot;
        db.renderDot = renderDot;
        db.index = dots.Count;
        dots.Add(dot.transform);
    }

    void MoveDot(DotBehaviour dot)
    {
        if (renderPlane.IsPlaneHovered())
        {
            dot.transform.position = GetMouseInWorldSpace();
            dot.renderDot.transform.position = dot.transform.position;
            RoadDotBehaviour rdb = dot as RoadDotBehaviour;
            rdb.RedrawRoads();
        }
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    void RemoveDot(DotBehaviour dot)
    {
        for (int i = dot.index + 1; i < dots.Count; i++)
        {
            dots[i].GetComponent<DotBehaviour>().index--;
        }
        RoadDotBehaviour rdb = dot as RoadDotBehaviour;
        for(int i = 0; i < rdb.connectedRoads.Count; i++)
        {
            roadPieces.Remove(rdb.connectedRoads[i]);
        }
        dots.RemoveAt(dot.index);
        dot.Remove();
    }

    void SelectDot(DotBehaviour dot)
    {
        if (roadInProgress != true)
        {
            roadInProgress = true;
            RoadPiece roadPiece = Instantiate(lineRendererPrefab).GetComponent<RoadPiece>();
            RoadDotBehaviour rdb = dot as RoadDotBehaviour;
            roadPiece.startDot = rdb;
            roadPieces.Add(roadPiece);
            activeRoadPiece = roadPiece;
        }
        else
        {
            roadInProgress = false;
            RoadDotBehaviour rdb = dot as RoadDotBehaviour;
            activeRoadPiece.endDot = rdb;
            rdb.connectedRoads.Add(activeRoadPiece);
            activeRoadPiece.startDot.connectedRoads.Add(activeRoadPiece);
            activeRoadPiece.DrawLine();
            activeRoadPiece = null;
        }

    }

    void DeSelectDot()
    {
        if (roadInProgress)
        {
            roadPieces.Remove(activeRoadPiece);
            activeRoadPiece.Remove();
            //Destroy(activeRoadPiece.gameObject);
            activeRoadPiece = null;
            roadInProgress = false;
        }
    }
}
