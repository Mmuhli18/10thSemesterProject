using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadDotBehaviour : DotBehaviour
{
    [HideInInspector]
    public List<RoadPiece> connectedRoads = new List<RoadPiece>();

    public void RedrawRoads()
    {
        
        for (int i = 0; i < connectedRoads.Count; i++)
        {
            connectedRoads[i].DrawLine();
        }
    }

    public override void Remove()
    {
        int count = connectedRoads.Count;
        for (int i = 0; i < count; i++)
        {
            connectedRoads[0].Remove();
        }
        Destroy(renderDot);
        Destroy(gameObject);
    }
}
