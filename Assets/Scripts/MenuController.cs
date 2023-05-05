using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using CustomUIClasses;

[RequireComponent(typeof(MenuUI))]
public class MenuController : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField]
    bool actionWarnings = false;
    public Action onChangeEvent;
    public Action onAnomalyOptionUpdateEvent;
    public Action onTrafficSettingUpdateEvent;
    public Action onLightingSettingUpdateEvent;
    public Action onRoadSettingUpdateEvent;
    public Action onExportSettingUpdateEvent;
    public Action onRoadTransformUpdateEvent;
    public Action<string> onExportClickedEvent;
    public Action onRoadLengthUpdateEvent;
    
    [Header("Settings")]
    public bool enableFancyLighting = true;
    public bool enableStableRendering = true;
    public TooltipTextCollection tipCollection;

    [Header("Values")]
    [SerializeField]
    List<AnomalyOption> anomalyOptions = new List<AnomalyOption>();

    [SerializeField]
    List<TrafficSetting> trafficSettings = new List<TrafficSetting>();

    [SerializeField]
    LightingSetting lightingSettings = new LightingSetting();

    [SerializeField]
    List<RoadSetting> roadSettings = new List<RoadSetting>();

    [SerializeField]
    ExportSetting exportSettings = new ExportSetting();

    [SerializeField]
    float roadLength = 1f;

    [SerializeField]
    RoadTransformSetting roadTransformSettings = new RoadTransformSetting();

    [Header("Stuff to make it work & debug")]

    public bool RenderTesting;

    [SerializeField]
    Texture2D outputTexture;

    [SerializeField]
    public ViewportHandler viewportHandler;

    [SerializeField]
    public PointController pointController;

    [SerializeField]
    List<GameObject> fancyLightObjects;

    public static UIDocument UIDoc;
    private static MenuUI menuUI;


    void Start()
    {
        //NumberField is one of our custom UIClasses
        //The click and drag of numberfield requires the use of tracking mouse. To do this numberfield uses a static instance of monobehaviour
        //We give that instance to it here
        NumberField.instance = this;
        UIDoc = GetComponent<UIDocument>();
        menuUI = GetComponent<MenuUI>();
        
        //LoadFSpy uses a static camera for the viewport, we define this here
        LoadFSpy._camera = viewportHandler.viewportCam;

        //Loading our frontend UI
        menuUI.SetupMenu();

        //Whenever events are executed the onChangeEvent is run, this event is tied to render a preview for the user
        //This will render a new fresh preview for the user whenever they make changes to the simulation
        onChangeEvent += viewportHandler.RenderPreviewSprite;
        //PointController event for what drawing mode the foreground tool is in
        pointController.drawingSwitchEvent += ForegroundButtonSwitch;
    }

    private void Update()
    {
        //RenderTesting can be flicked on in the inspector to force a render
        if (RenderTesting)
        {
            GetMask();
            RenderTesting = false;
            viewportHandler.RenderPreviewSprite();
        }
        //FancyLighting makes the simulation look better in the preview, but decreased performance.
        //Fancy lighting is always on in renders.
        if(enableFancyLighting != fancyLightObjects[0].activeSelf)
        {
            for(int i = 0; i < fancyLightObjects.Count; i++)
            {
                fancyLightObjects[i].SetActive(enableFancyLighting);
            }
        }
    }

    public void PlayPauseSimulation()
    {
        bool result = viewportHandler.PlayPausePreview();
        if (result) menuUI.playPauseButton.text = "Play";
        else menuUI.playPauseButton.text = "Pause";
    }

    void ForegroundButtonSwitch(bool isDrawing)
    {
        if (isDrawing)
        {
            menuUI.drawForegroundButton.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            menuUI.addMarkingButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            menuUI.drawForegroundButton.style.backgroundColor = new StyleColor(new Color(0.67f, 0.67f, 0.67f));
            menuUI.addMarkingButton.style.display = DisplayStyle.None;
        }
    }

    //This function is used when running the differnet events of the menu
    void TryEvent(Action eventAction)
    {
        //If and event is defined and tied to a method
        if(eventAction != null && eventAction.Method != null)
        {
            eventAction.Invoke();
        }
        else
        {
            if(actionWarnings && eventAction != onChangeEvent) Debug.LogWarning("No action tied to event");
            else if(actionWarnings) Debug.LogWarning("No action tied to onChangeEvent");
        }
        //onChangeEvent will run if an event is not onChangeEvent
        if (eventAction != onChangeEvent)
        {
            TryEvent(onChangeEvent);
        }
    }
    //If an event needs to pass an argument along
    void TryEvent<T>(Action<T> eventAction, T arguement)
    {
        if (eventAction != null && eventAction.Method != null)
        {
            eventAction.Invoke(arguement);
        }
        else
        {
            if (actionWarnings) Debug.LogWarning("No action tied to event");
        }
        TryEvent(onChangeEvent);
    }

    //
    //Transform menu functions
    //

    public void UpdateRoadScale(NumberField field)
    {
        roadTransformSettings.distance = field.value;
        UpdateRoadTransform();
    }

    public void UpdateRoadTransform(Vector3 value = new Vector3(), string vectorName = "")
    {
        //The VectorFieldControllers will pass along a name for the vector, that's how we know which vector to update
        switch (vectorName)
        {
            case "Position":
                roadTransformSettings.position = value;
                break;
            case "Rotation":
                roadTransformSettings.rotation = value;
                break;
        }
        TryEvent(onRoadTransformUpdateEvent);
    }

    //
    //     Export functions
    //
    public void DoExport()
    {
        /*Idea was to pass a string along with the output folder, however this is not implemented in this version
         * instead this version passes an empty string and uses a default folder*/
        TryEvent(onExportClickedEvent, "");
    }

    /* All our export information is stored in a holder class called ExportSetting, the values for this are updated in
     * the following functions */
    public void UpdateOutputType(RadioButtonGroup rbg)
    {
        /*This will change the output type, however we can only output image sequences in this iteration, therefore this is mostly
        * useless, but this setting is still changed here*/
        switch (rbg.value)
        {
            case 0:
                exportSettings.outputType = ExportOutputType.ImageSequence;
                break;
            case 1:
                exportSettings.outputType = ExportOutputType.VideoFile;
                break;
        }
        TryEvent(onExportSettingUpdateEvent);
    }

    public void UpdateAnomalyMix(Toggle toggle)
    {
        exportSettings.mixAnomalies = toggle.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    public void UpdateLengthValue(NumberField numberField)
    {
        exportSettings.videoLength = (int)numberField.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    public void UpdateAmountValue(NumberField numberField)
    {
        exportSettings.videoAmount = (int)numberField.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    //
    //    Tab functions
    //
    public void UpdateShadowAlpha(NumberField field)
    {
        UpdateShadowColor(new Vector3(field.value, 0f, 0f), "Shadow");
        TryEvent(onLightingSettingUpdateEvent);
    }
    public void UpdateShadowColor(Vector3 vector, string name)
    {
        switch (name)
        {
            case "ColorVector":
                lightingSettings.shadowColor = new Vector4(vector.x, vector.y, vector.z, lightingSettings.shadowColor.w);
                break;
            case "Shadow":
                lightingSettings.shadowColor = new Vector4(lightingSettings.shadowColor.x, lightingSettings.shadowColor.y, lightingSettings.shadowColor.z, vector.x);
                break;
        }
        TryEvent(onLightingSettingUpdateEvent);
    }
    public void UpdateLightDirection(Vector3 vector, string name)
    {
        lightingSettings.direction = vector;
        TryEvent(onLightingSettingUpdateEvent);
    }
    public void UpdateAmbient(float value)
    {
        lightingSettings.ambient = value;
        TryEvent(onLightingSettingUpdateEvent);
    }
    public void UpdateIntensity(float value)
    {
        lightingSettings.intensity = value;
        TryEvent(onLightingSettingUpdateEvent);
    }
    public void UpdateAnomalyValue(AnomalyController controller)
    {
        for (int i = 0; i < anomalyOptions.Count; i++)
        {
            if (anomalyOptions[i].name == controller.name)
            {
                anomalyOptions[i].value = controller.value;
                anomalyOptions[i].active = controller.isActive;
            }
        }
        TryEvent(onAnomalyOptionUpdateEvent);
    }
    public void UpdateTrafficValue(TrafficSettingController controller)
    {
        for(int i = 0; i < trafficSettings.Count; i++)
        {
            if(trafficSettings[i].name == controller.name)
            {
                trafficSettings[i].value = controller.value;
                trafficSettings[i].offsetLeft = controller.offsetLeft / 10f;
                trafficSettings[i].offsetRight = controller.offsetRight / 10f;
            }
        }
        TryEvent(onTrafficSettingUpdateEvent);
    }
    public void UpdateRoadValue(RoadSettingController controller)
    {
        for(int i = 0; i < roadSettings.Count; i++)
        {
            if(roadSettings[i].name == controller.name)
            {
                roadSettings[i].leftValue = controller.leftValue / 10f;
                roadSettings[i].rightValue = controller.rightValue / 10f;
                roadSettings[i].isActive = controller.isActive;
                if(roadSettings[i].useSlider) roadSettings[i].sliderValue = (controller as RoadSettingSliderController).sliderValue;
            }
        }
        TryEvent(onRoadSettingUpdateEvent);
    }
    public void UpdateRoadLength(NumberField field)
    {
        roadLength = field.value;
        TryEvent(onRoadLengthUpdateEvent);
    }

    //
    //     GET functions
    //

    public List<AnomalyOption> GetAnomalies()
    {
        List<AnomalyOption> options = new List<AnomalyOption>();
        for(int i = 0; i < anomalyOptions.Count; i++)
        {
            AnomalyOption option = anomalyOptions[i];
            if (!option.active) option.value = 0;
            options.Add(option);
        }
        return options;
    }

    public List<TrafficSetting> GetTrafficSettings()
    {
        return trafficSettings;
    }

    public LightingSetting GetLightingSettings()
    {
        return lightingSettings;
    }

    public Texture2D GetMask()
    {
        bool lastState = enableFancyLighting;
        enableFancyLighting = false;
        Texture2D outputTestTexture = viewportHandler.RenderMask();
        outputTestTexture.Apply();
        /* Our output texture is rendered at a different resolution that what we use for the simulation, we therefore
         * change the resolution here */
        ChangeOutputTexture(outputTestTexture);

        enableFancyLighting = lastState;
        return outputTexture;
    }

    //Used for changing resolution of a texture
    void ChangeOutputTexture(Texture2D texture)
    {
        float scalerX = ((float)outputTexture.width / (float)texture.width);
        float scalerY = ((float)outputTexture.height / (float)texture.height);
        for(float x = 0; x < texture.width; x++)
        {
            for(float y = 0; y < texture.height; y++)
            {
                outputTexture.SetPixel((int)(x * scalerX), (int)(y * scalerY), texture.GetPixel((int)x, (int)y));
            }
        }
        outputTexture.Apply();
    }

    public ExportSetting GetExportSettings()
    {
        return exportSettings;
    }

    public RoadTransformSetting GetRoadTransform()
    {
        return roadTransformSettings;
    }

    public List<RoadSetting> GetRoadSettings()
    {
        List<RoadSetting> settings = new List<RoadSetting>();
        for (int i = 0; i < roadSettings.Count; i++)
        {
            RoadSetting setting = roadSettings[i];
            //If a part of the road has been set to not be active we disable it by making the width 0
            if(!setting.isActive)
            {
                setting.leftValue = 0;
                setting.rightValue = 0;
            }
            settings.Add(setting);
        }
        return settings;
    }

    public float GetRoadLength()
    {
        return roadLength;
    }
}

//
//    Serializable classes
//
/*These are all our serialized classes, these hold differnet informations and values for settings throughout the menu */

//A base class used for all setting that are in list and use names for identification, such as AnomalyOptions or TrafficSettings
public class BaseNamedSetting
{
    public string name;
    public string labelName;
    public string tooltip;
    public ToolTip.Alignment tooltipAlignment = ToolTip.Alignment.Top;
}


[Serializable]
public class AnomalyOption : BaseNamedSetting
{
    public float value;
    public bool active;
}

[Serializable]
public class TrafficSetting : BaseNamedSetting
{
    public float value;
    public bool useOffsets;
    public float offsetRight;
    public float offsetLeft;
    public Color color;
    [HideInInspector]
    public string positionToolTipText;
}


[Serializable]
public class LightingSetting
{
    public float intensity;
    public float ambient;
    public Vector3 direction;
    public Vector4 shadowColor;
}

[Serializable]
public class RoadTransformSetting
{
    public Vector3 position;
    public Vector3 rotation;
    public float distance;
}

[Serializable]
public  class RoadSetting :BaseNamedSetting
{
    public float leftValue;
    public float rightValue;
    public bool isActive;
    public bool useSlider;
    public float sliderValue;
    public Color color;
}

[Serializable]
public class ExportSetting
{
    public int videoLength;
    public int videoAmount;
    public bool mixAnomalies;
    public ExportOutputType outputType;
}

public enum ExportOutputType
{
    ImageSequence,
    VideoFile,
    Gif,
    SingleImage
}

