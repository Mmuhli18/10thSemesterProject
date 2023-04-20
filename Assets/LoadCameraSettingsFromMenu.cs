using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCameraSettingsFromMenu : MonoBehaviour
{
    public bool loadSettingsOnStart = true;
    public GameObject backgroundPlane;
    // Start is called before the first frame update
    void Start()
    {
        if (loadSettingsOnStart)
        {
            TryLoadSettingsFromMenu();
        }
    }

    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null) { return; }
        gameObject.transform.position = settings.cameraPosition;
        gameObject.transform.localEulerAngles = settings.cameraRotation;
        
        if (backgroundPlane.TryGetComponent(out Renderer renderer))
        {
            renderer.sharedMaterial.mainTexture = settings.background;
        }
    } 
}
