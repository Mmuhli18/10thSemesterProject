using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConfig : MonoBehaviour
{
    public Transform lightTransform;
    public bool nightmode = false;

    float defaultValue;


    void Start()
    {
        defaultValue = lightTransform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (nightmode)
        {
            lightTransform.localEulerAngles = new Vector3(14f, lightTransform.localEulerAngles.y, lightTransform.localEulerAngles.z);
        }
        else
        {
            lightTransform.localEulerAngles = new Vector3(defaultValue, lightTransform.localEulerAngles.y, lightTransform.localEulerAngles.y);
        }
    }
}
