using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class RenderDotPlacer : MonoBehaviour
{
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Camera sceneCamera;


    public void AddPointAtMouse()
    {
        GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
    }

    public void RemovePoint()
    {
        
    }

    public void MovePoint(DotBehaviour point)
    {
        point.transform.position = GetMouseInWorldSpace();
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }
}
