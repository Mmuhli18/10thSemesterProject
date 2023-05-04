using UnityEngine;


public class LightConfig : MonoBehaviour
{
    public Transform lightTransform;
    public Material shadowReceiver;


    void Start()
    {
        TryLoadSettingsFromMenu();
    }

    //Rotates our lighting in the scene and sets the showdow color
    void TryLoadSettingsFromMenu()
    {
        MenuSettingsForSimulation settings = FindObjectOfType<MenuSettingsForSimulation>();
        if (settings == null || settings.HasExported() == false) { return; }
        lightTransform.localEulerAngles = settings.lightingDirection;
        var col = Color.HSVToRGB(settings.lightingShadow.x / 255f, settings.lightingShadow.y / 255f, settings.lightingShadow.z / 255f);
        //Debug.Log(col);
        shadowReceiver.SetColor("_Shadow_Color", new Color(col.r, col.g, col.b, settings.lightingShadow.w / 255f));
        //Debug.Log(shadowReceiver.GetColor("_Shadow_Color"));
    }
}
