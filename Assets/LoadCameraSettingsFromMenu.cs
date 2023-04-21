using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCameraSettingsFromMenu : MonoBehaviour
{
    public bool loadSettingsOnStart = true;
    public GameObject backgroundPlane;
    public List<Camera> cameras;

    float defaultFOV;
    // Start is called before the first frame update
    void Start()
    {
        defaultFOV = cameras[0].fieldOfView;
        if (loadSettingsOnStart)
        {
            TryLoadSettingsFromMenu();
        }
    }

    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null || settings.HasExported() == false) { return; }
        gameObject.transform.position = settings.cameraPosition;
        gameObject.transform.eulerAngles = settings.cameraRotation;
        foreach(Camera cam in cameras)
        {
            cam.fieldOfView = settings.cameraFOV;
            cam.usePhysicalProperties = false;
        }
        var planePos = backgroundPlane.transform.localPosition;
        planePos.z *= (64.5402998239f / settings.cameraFOV);
        backgroundPlane.transform.localPosition = planePos;
        
        if (backgroundPlane.TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.mainTexture = settings.background;
        }
    } 
}
