using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SafeFogDisable : MonoBehaviour
{
    float timeBeforeDisable = 1f;
    // This script disables the fog after one second has passed. This is because we don't want fog, but starting the program with fog disabled breaks everything. I love HDRP :)
    IEnumerator Start()
    {
        GetComponent<Volume>().profile.TryGet(out Fog fog);
        if (fog != null){ fog.active = true; }
        yield return new WaitForSecondsRealtime(timeBeforeDisable);
        if (fog != null){ fog.active = false;}
    }

}
