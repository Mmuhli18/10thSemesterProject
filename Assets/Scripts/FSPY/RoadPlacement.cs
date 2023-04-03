using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPlacement : MonoBehaviour
{
    

    public Vector3 GettingNewOregoPoint() 
    {
        Vector3 pos = new Vector3();
        GameObject BGImg = GameObject.Find("Image of road/CanvasBackgroundImage");
        if (BGImg != null)
        {
            pos = BGImg.transform.position;
            return pos;
        }
        else 
        {
            Debug.Log("Could not find game object");
            return pos ;
        }

    }

    public Quaternion GettingOrientationOfOrego() 
    {
        Quaternion rotation = new Quaternion();
        GameObject BGImg = GameObject.Find("Image of road/CanvasBackgroundImage");
        if (BGImg != null)
        {
            rotation = BGImg.transform.rotation;
            return rotation;
        }
        else
        {
            Debug.Log("Could not find game object");
            return rotation;
        }
    }
    public void SetPosOfRoad(string nameOfGameObject) 
    {
        GameObject road = GameObject.Find(nameOfGameObject);
        if (road != null)
        {
            Vector3 oregoPoint = GettingNewOregoPoint();
            oregoPoint = new Vector3 (oregoPoint.x , oregoPoint.y, oregoPoint.z);
            Quaternion rotationFromOregoPoint = GettingOrientationOfOrego();
            road.transform.position = oregoPoint;
            road.transform.rotation = rotationFromOregoPoint;
        }
        else 
        {
            Debug.Log("Could not find game object");
        }

    }
    /*
    // Start is called before the first frame update
    void Start()
    {
        gettingNewOregoPoint();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }*/
}
