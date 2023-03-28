using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    Camera ViewCamera; 
    public void CameraRotation(Vector3 rot) 
    {
        if (ViewCamera != null)
        {
            Quaternion rotation = new Quaternion();
            rotation = Quaternion.Euler(rot);
            ViewCamera.transform.rotation = rotation;
        }
    }

    public void CameraPosition(Vector3 pos) 
    {
        if (ViewCamera != null)
        {
            ViewCamera.transform.position = pos;
        }
    }

    public void SetCamera(Camera c) 
    {
        ViewCamera = c;
    }

    public void CheckIfRoadIsInView (Camera c, GameObject road) 
    {
        if (road != null && c != null)
        {
            Vector3 cameraViewPos = c.WorldToViewportPoint(road.transform.position);
            if (cameraViewPos.x > 0 && cameraViewPos.x < 1 && cameraViewPos.y > 0 && cameraViewPos.y < 1)
            {
                Debug.Log("Road is in camera view");
                GameObject BGImg = GameObject.Find("Image of road");

                Mesh roadMesh = road.GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = roadMesh.vertices;
                foreach (Vector3 vertex in vertices) 
                {
                    Debug.Log(vertex);
                }

                Vector3 normalized;
                normalized.x = (road.transform.position.x - Math.Min(c.transform.position.x, BGImg.transform.position.x)) / (Math.Max(c.transform.position.x, BGImg.transform.position.x) - Math.Min(c.transform.position.x, BGImg.transform.position.x));
                normalized.y = (road.transform.position.y - Math.Min(c.transform.position.y, BGImg.transform.position.y)) / (Math.Max(c.transform.position.y, BGImg.transform.position.y) - Math.Min(c.transform.position.y, BGImg.transform.position.y));
                normalized.z = (road.transform.position.z - Math.Min(c.transform.position.z, BGImg.transform.position.z)) / (Math.Max(c.transform.position.z, BGImg.transform.position.z) - Math.Min(c.transform.position.z, BGImg.transform.position.z));
                
                if (   normalized.x > 0 && normalized.x < 1
                    && normalized.y > 0 && normalized.y < 1
                    && normalized.z > 0 && normalized.z < 1 )
                {
                    Debug.Log("The road is between the camera and the image");
                }
                else 
                {
                    Debug.Log("The road is in the field of view, but it is out side the background image");
                } 

            }
            else
            {
                Debug.Log("Road NOT in view");
            }
        }
        else 
        {
            Debug.Log("Either 'Road' or 'camera' was null");
        }
    }

    public Camera c;
    public GameObject road;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        c = GameObject.Find("Main Camera").GetComponent<Camera>();
        road = GameObject.Find("RoadGO/Road");
        CheckIfRoadIsInView(c, road);
    }
}
