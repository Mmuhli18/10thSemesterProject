using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PointController : MonoBehaviour
{
    [SerializeField] 
    GameObject dotPrefab;
    [SerializeField]
    Camera sceneCamera;

    public void AddPointAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 15 && mousePos.x < 1215)
        {
            if (mousePos.y > 440 && mousePos.y < 1010)
            {
                GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
                dot.GetComponent<DotBehaviour>().OnDragEvent += MovePoint;
            }
        }
    }

    void MovePoint(DotBehaviour point)
    {
        point.transform.position = GetMouseInWorldSpace();
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }
}
