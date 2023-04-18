using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlenderRotationToUnityRotation : MonoBehaviour
{
    public void rotate() 
    {
        Camera cam = GameObject.Find("BlenderToUnityrotationTest").GetComponent<Camera>();
        cam.transform.position = new Vector3(cam.transform.position.x *-1, cam.transform.position.z*-1, cam.transform.position.y);

        //Vector3 originalRotation = cam.transform.rotation.eulerAngles;
        Vector3 originalRotation = new Vector3(307.6f, 83f, 56.7f);
        /*
        Vector3 z_axis_euler = new Vector3(0.0f, originalRotation.z * -1, 0.0f);
        Vector3 y_axis_euler = new Vector3(0.0f, 0.0f, originalRotation.y * -1);
        Vector3 x_axis_euler = new Vector3(originalRotation.x * -1, 0.0f, 0.0f);
        */
        //Vector3 originalRotation = new Vector3(90f, 45f, 0);
        Debug.Log(originalRotation);

        //Transform camTranform = cam.GetComponent<Transform>();
        Vector3 zero = Vector3.zero;
        Quaternion z = Quaternion.Euler(zero);
        cam.transform.rotation = z;
        /*
        Quaternion z_axis = Quaternion.Euler(z_axis_euler);
        cam.transform.rotation = z_axis;
        Quaternion y_axis = Quaternion.Euler(y_axis_euler);
        cam.transform.rotation = y_axis;
        //x_axis_euler.x += 90f;
        Quaternion x_axis = Quaternion.Euler(x_axis_euler);
        cam.transform.rotation = x_axis;
        */


        //camTranform.rotation.eulerAngles.Set(zero.x, zero.y, zero.z);
        //camTranform.rotation = Quaternion.Euler(zero);

        //Debug.Log(camTranform.rotation.x);
        
        cam.transform.Rotate(0.0f, originalRotation.z * -1, 0.0f);
        Debug.Log(cam.transform.rotation.eulerAngles);
        cam.transform.Rotate(0.0f, 0.0f, originalRotation.y * -1);
        Debug.Log(cam.transform.rotation.eulerAngles);
        cam.transform.Rotate((originalRotation.x *-1), 0.0f, 0.0f);
        Debug.Log(cam.transform.rotation.eulerAngles);
        
        //camTranform.Rotate(90f, 0.0f, 0.0f);

    }

    public void Start()
    {
        rotate();
    }
    public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            rotate();
            Debug.Log("You pressed Q ");
        }
        */
    }
}
