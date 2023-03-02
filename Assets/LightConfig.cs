using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConfig : MonoBehaviour
{
    public Transform light;
    public bool nightmode = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
            light.localEulerAngles = new Vector3(64f, light.localEulerAngles.y, light.localEulerAngles.y);
        }
    }
}
