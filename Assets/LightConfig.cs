using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConfig : MonoBehaviour
{
    public Transform light;
    public bool nightmode = false;

    float defaultValue;


    void Start()
    {
        defaultValue = light.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (nightmode)
        {
            light.localEulerAngles = new Vector3(14f, light.localEulerAngles.y, light.localEulerAngles.z);
        }
        else
        {
            light.localEulerAngles = new Vector3(defaultValue, light.localEulerAngles.y, light.localEulerAngles.y);
        }
    }
}
