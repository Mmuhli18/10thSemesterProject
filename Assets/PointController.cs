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
    [SerializeField]
    ViewportHandler viewportHandler;

    public void AddPointAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        if (viewportHandler.markings.Count < 1) return;
        if (viewportHandler.activeMarking < 0) return;
        if (mousePos.x > 15 && mousePos.x < 1215)
        {
            if (mousePos.y > 440 && mousePos.y < 1010)
            {
                GameObject dot = Instantiate(dotPrefab, GetMouseInWorldSpace(), Quaternion.identity, transform);
                GameObject renderDot = Instantiate(renderDotPrefab, GetMouseInWorldSpace(), Quaternion.identity, viewportHandler.markings[viewportHandler.activeMarking].spriteShapeController.transform);
                DotBehaviour db = dot.GetComponent<DotBehaviour>();
                db.OnDragEvent += MovePoint;
                db.RightClickEvent += RemovePoint;
                db.renderDot = renderDot;
                var points = viewportHandler.markings[viewportHandler.activeMarking].points;
                db.index = points.Count;
                db.markingIndex = viewportHandler.activeMarking;
                points.Add(dot.transform);
                viewportHandler.markings[viewportHandler.activeMarking].points = points;
            }
        }
    }

    void MovePoint(DotBehaviour point)
    {
        viewportHandler.activeMarking = point.markingIndex;
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 15 && mousePos.x < 1215)
        {
            if (mousePos.y > 440 && mousePos.y < 1010)
            {
                point.transform.position = GetMouseInWorldSpace();
                point.renderDot.transform.position = point.transform.position;
            }
        }
    }

    Vector3 GetMouseInWorldSpace()
    {
        return sceneCamera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 9));
    }

    public void RemovePoint(DotBehaviour dot)
    {
        viewportHandler.activeMarking = dot.markingIndex;
        var points = viewportHandler.markings[dot.markingIndex].points;
        for (int i = dot.index + 1; i < points.Count; i++)
        {
            points[i].GetComponent<DotBehaviour>().index--;
        }
        points.RemoveAt(dot.index);
        dot.Remove();
        viewportHandler.markings[dot.markingIndex].points = points;
    }

    private void LateUpdate()
    {
        if (viewportHandler.markings.Count > 0)
        {
            var points = viewportHandler.markings[viewportHandler.activeMarking].points;
            if (points.Count >= 1)
            {
                Spline spline = viewportHandler.markings[viewportHandler.activeMarking].spriteShapeController.spline;
                spline.Clear();
                for (int i = 0; i < points.Count; i++)
                {
                    spline.InsertPointAt(i, points[i].position);
                }
                viewportHandler.markings[viewportHandler.activeMarking].spriteShapeController.RefreshSpriteShape();
            }
            viewportHandler.markings[viewportHandler.activeMarking].points = points;
        }
    }
}
