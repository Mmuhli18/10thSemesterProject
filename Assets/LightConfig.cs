using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConfig : MonoBehaviour
{
    public Transform lightTransform;
    public Material shadowReceiver;


    void Start()
    {
        TryLoadSettingsFromMenu();
    }

    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null || settings.HasExported() == false) { return; }
        lightTransform.localEulerAngles = settings.lightingDirection;
        var col = settings.lightingShadow;
        Debug.Log(col);
        shadowReceiver.SetColor("_Shadow_Color", new Color(col.x / 255, col.y / 255, col.z / 255, col.w / 255));
        Debug.Log(shadowReceiver.GetColor("_Shadow_Color"));
    }
}
