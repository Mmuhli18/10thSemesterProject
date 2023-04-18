using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMovement : MonoBehaviour
{
    Camera ViewCamera;
    GameObject BGImg;
    GameObject road; 
    Vector3 standardRotation = new Vector3 ();
    Vector3 standardPossition = new Vector3();
    Vector3 possition = new Vector3();
    Vector3 rotation = new Vector3();

    public void CameraRotation(Vector3 rot)                                              // Takes in a 3D vector: "rot", this is suppose to be how much is the object suppose to rotate.
    {
        if (ViewCamera != null )                                 // Checks if the camera is NOT eaqual to Null, but also of the road is in view
        {
            rotation = rotation + rot;                                                   // Adds the rot (rotation from the user) to the already rotated 
            Quaternion r = new Quaternion();                                             // Assigns a new Quaternion, called "r" 
            r = Quaternion.Euler(standardRotation + rotation);                           // Sets the Euler rotation of "r" to be the standard rotation, basically the pre detimined rotation of the camera, plus the roation the user has inputed. 
            ViewCamera.transform.rotation = r;                                           // Sets the rotation of the camera to be equal to "r"
        }   
    }

    public void CameraPosition(Vector3 pos)                                               // Takes in a 3D Vector: "pos", this is suppose to be how much the camera has beenmoved by the user
    {
        if (ViewCamera != null && CheckIfRoadIsInView())                                  // Checks if the camera is NOT null, and if the road is still in view of the camera 
        {
            possition = possition + pos;                                                  // Adds pos, the how much the user wants to change the possition, to a vector called possition
            ViewCamera.transform.position = standardPossition + possition;                // Sets the possition of the camera, to standarspossition plus the possition vector.
            Debug.Log(ViewCamera.transform.position);
        }
    }

    public void SetCameraPositionToStandardPosition() 
    {
        ViewCamera.transform.position = standardPossition;
    }

    public void SetCameraPositionToStandardRotation()
    {
        Quaternion r = new Quaternion();                                              
        r = Quaternion.Euler(standardRotation);                           
        ViewCamera.transform.rotation = r;
    }

    public void SetCamera(Camera c)                                                        // A set function for the camera. 
    {
        ViewCamera = c;
    }

    public void SetStandardPos(Vector3 pos)                                                // A set function for the standard possition (dont real know if it was nessasrry but not it is there)
    {
        standardPossition = pos; 
    }

    public Vector3 GetStandardPos()                                                        // A Get function for the standard posstion 
    {
        return standardPossition;
    }
    public void SetStandardRotation(Vector3 rot)                                           // A set function for the standard rotation  
    {
        standardRotation = rot; 
    }

    public Vector3 GetStandardRotation()                                                    // A get function for the standard rotation 
    {
        return standardRotation; 
    }

    public void SetBackgroundImage(GameObject BGImg)                                       // A Set function for the Background image  
    {
        this.BGImg = BGImg;
    }

    public void SetRoad(GameObject road)                                                    // A set function for the road object 
    {
        this.road = road; 
    }
    public bool CheckIfRoadIsInView ()                                                                              // Checks if the road is within the field-of-view of the camera, and if the road is in front of the camera.  
    {
        if (road != null && ViewCamera != null)                                                                     // Checks if the road is NOT null and that the Camera is NOT null
        {
            Vector3 cameraViewPos = ViewCamera.WorldToViewportPoint(road.transform.position);                       // Goes from world possition to view posstion (goes from world space to what the camera can see, possition is normalized to the camera) 
            if (cameraViewPos.x > 0 && cameraViewPos.x < 1 && cameraViewPos.y > 0 && cameraViewPos.y < 1)           // If the object is with in the cameras field of view. 
            {
                Debug.Log("Road is in camera view");
                //GameObject BGImg = GameObject.Find("CanvasBackgroundImage");
                if (BGImg != null)                                                                                  // Checks if the BGImg is not equal 
                {
                    Mesh roadMesh = road.GetComponent<MeshFilter>().mesh;                                           // Gets the mesh of the road object

                    Vector3[] vertices = roadMesh.vertices;                                                         // Gets the vertices of the road object.
                    Vector3[] normalizedVertices = new Vector3[vertices.Length];                                    // Initialze a second array of 3D vectors.  

                    for (int i = 0; i < vertices.Length; i++)                                                       // Loops through the verticies and calculates the normalized distances from the caerma to the Background image. 
                    {
                        normalizedVertices[i].x = ((road.transform.position.x + vertices[i].x) - Math.Min(ViewCamera.transform.position.x, BGImg.transform.position.x)) / (Math.Max(ViewCamera.transform.position.x, BGImg.transform.position.x) - Math.Min(ViewCamera.transform.position.x, BGImg.transform.position.x));
                        normalizedVertices[i].y = ((road.transform.position.y + vertices[i].y) - Math.Min(ViewCamera.transform.position.y, BGImg.transform.position.y)) / (Math.Max(ViewCamera.transform.position.y, BGImg.transform.position.y) - Math.Min(ViewCamera.transform.position.y, BGImg.transform.position.y));
                        normalizedVertices[i].z = ((road.transform.position.z + vertices[i].z) - Math.Min(ViewCamera.transform.position.z, BGImg.transform.position.z)) / (Math.Max(ViewCamera.transform.position.z, BGImg.transform.position.z) - Math.Min(ViewCamera.transform.position.z, BGImg.transform.position.z));
                    }


                    foreach (Vector3 votex in normalizedVertices)                                                   // loops through the normalized verticies and checks if any of the normlize verticies is between 0 and 1. 
                    {
                        if (votex.x > 0 && votex.x < 1
                            && votex.y > 0 && votex.y < 1
                            && votex.z > 0 && votex.z < 1)
                        {
                        }
                        else
                        {
                            return false;                                                                           // Returns false if any of the verticies is NOT betweem 0 and 1. 
                        }
                    }

                    return true;                                                                                    // Returns true, if all of verticies is between 0 and 1 
                }
                else 
                {
                    return false;                                                                                   // Returns false, if the background image is equal to null
                }
            }
            else
            {
                return false;                                                                                       // Returns false, if the road object is NOT in the field of view of the camera. 
            }
        }
        else 
        {
            return false; 
        }
    }
    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            SetStandardRotation(Vector3.zero);
            SetCamera( GameObject.Find("Main Camera").GetComponent<Camera>());
            CameraRotation(new Vector3(44.5446f, 4.38444f, -34.156f));
            Debug.Log("You Pressed Q");
        }
    }
}
