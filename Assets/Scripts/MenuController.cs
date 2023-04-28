using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[RequireComponent(typeof(OpenFSpyFromUnity))]
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
    ViewportHandler viewportHandler;

    [SerializeField]
    PointController pointController;

    [SerializeField]
    VisualTreeAsset anomolyController;

    [SerializeField]
    VisualTreeAsset trafficSettingController;

    [SerializeField]
    VisualTreeAsset lightingTab;

    [SerializeField]
    VisualTreeAsset roadSettingController;

    [SerializeField]
    VisualTreeAsset roadSettingTabLayout;

    [SerializeField]
    List<GameObject> fancyLightObjects;

    private OpenFSpyFromUnity fSpy;
    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();
    private List<TabElement> tabElements = new List<TabElement>();
    private List<AnomalyController> anomalyControllers;
    private List<TrafficSettingController> trafficSettingControllers;
    private List<RoadSettingController> roadSettingControllers;
    private Button playPauseButton;

    void Start()
    {
        //NumberField is one of our custom UIClasses
        //The click and drag of numberfield requires the use of tracking mouse. To do this numberfield uses a static instance of monobehaviour
        //We give that instance to it here
        NumberField.instance = this;
        UIDoc = GetComponent<UIDocument>();
        fSpy = GetComponent<OpenFSpyFromUnity>();
        //LoadFSpy uses a static camera for the viewport, we define this here
        LoadFSpy._camera = viewportHandler.viewportCam;
        //The layout file for the menu has a visual element specifically for the tabs and tab headers, this is found through a  query here
        tabMenuElement = UIDoc.rootVisualElement.Q<VisualElement>("settings-window");
        VisualElement tabs = UIDoc.rootVisualElement.Q<VisualElement>("tabs");

        //The tabs all have a button to switch for the tabs, this is controlled by the SettingTabButton class
        //We define these tab buttons here. The SettingTabButton is part of our custom UI elements
        tabButtons.Add(new SettingTabButton(tabs, "tab-anomalies", SettingTabButton.TabType.Anomalies));
        tabButtons.Add(new SettingTabButton(tabs, "tab-traffic", SettingTabButton.TabType.Traffic));
        tabButtons.Add(new SettingTabButton(tabs, "tab-lighting", SettingTabButton.TabType.Light));
        tabButtons.Add(new SettingTabButton(tabs, "tab-road", SettingTabButton.TabType.Road));
        //The tab buttons tie their onPressEvent tied to the function for switching tabs, where the tab type is passed along
        for(int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].onPressEvent += SwitchSettingTab;
        }

        CreateTabs();
        //Default tab of anomaly options is switched to
        SwitchSettingTab(SettingTabButton.TabType.Anomalies);

        //Functionallity for different buttons in the layout is defined
        UIDoc.rootVisualElement.Q<Button>("bt-add-footage").RegisterCallback<MouseUpEvent>(x => viewportHandler.AddFootage(x.currentTarget as Button));
        UIDoc.rootVisualElement.Q<Button>("bt-draw-foreground").RegisterCallback<MouseUpEvent>(x => pointController.AddMarking());
        UIDoc.rootVisualElement.Q<Button>("bt-export").RegisterCallback<MouseUpEvent>(x => DoExport());
        UIDoc.rootVisualElement.Q<Button>("bt-load-data").RegisterCallback<MouseUpEvent>(x => viewportHandler.LoadFSpy(fSpy));
        UIDoc.rootVisualElement.Q<Button>("bt-open-fspy").RegisterCallback<MouseUpEvent>(x => fSpy.OpenFSpy());
        playPauseButton = UIDoc.rootVisualElement.Q<Button>("bt-play-pause");
        playPauseButton.RegisterCallback<MouseUpEvent>(x => PlayPauseSimulation());

        SetupExportUI();
        SetupTransformMenu();
        //Whenever events are executed the onChangeEvent is run, this event is tied to render a preview for the user
        //This will render a new fresh preview for the user whenever they make changes to the simulation
        onChangeEvent += viewportHandler.RenderPreviewSprite;
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

    void PlayPauseSimulation()
    {
        bool result = viewportHandler.PlayPausePreview();
        if (result) playPauseButton.text = "Play";
        else playPauseButton.text = "Pause";
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

    void SetupTransformMenu()
    {
        NumberField scaleField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-scale"), false);
        Button resetButton = UIDoc.rootVisualElement.Q<Button>("bt-reset");

        //VectorFieldController is one of our custom UI classes. Two are defined here
        //These are the vector controllers that the user uses for moving the road/camera manually
        VectorFieldController vectorFieldPosition = new VectorFieldController(UIDoc.rootVisualElement, "tf-pos-x", "tf-pos-y", "tf-pos-z", true, "Position");
        VectorFieldController vectorFieldRotation = new VectorFieldController(UIDoc.rootVisualElement, "tf-rot-x", "tf-rot-y", "tf-rot-z", true, "Rotation");

        scaleField.SetValue(80);

        vectorFieldPosition.onVectorUpdateEvent += UpdateRoadTransform;
        vectorFieldRotation.onVectorUpdateEvent += UpdateRoadTransform;
        scaleField.onValueUpdateEvent += UpdateRoadScale;
        resetButton.clicked += MenuElementCollection.TransformElements.ResetValues;

        //MenuElementCollection is a static class used to defined the Visual elements in the menu. This is used for loading values into the menu
        MenuElementCollection.TransformElements.positionController = vectorFieldPosition;
        MenuElementCollection.TransformElements.rotationController = vectorFieldRotation;
        MenuElementCollection.TransformElements.scaleField = scaleField;
    }

    void UpdateRoadScale(NumberField field)
    {
        roadTransformSettings.distance = field.value;
        UpdateRoadTransform();
    }

    void UpdateRoadTransform(Vector3 value = new Vector3(), string vectorName = "")
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
    //     Export UI functions
    //

    void SetupExportUI()
    {
        //Our custom Numberfields
        NumberField lengthField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-length"), false);
        NumberField videoAmountField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-amount"), false);
        //Unity UI.Elements
        Toggle mixAnomalyToggle = UIDoc.rootVisualElement.Q<Toggle>("tg-mix-anomalies");
        RadioButtonGroup rbgOutType = UIDoc.rootVisualElement.Q<RadioButtonGroup>("rbg-output-type");
        Button exportButton = UIDoc.rootVisualElement.Q<Button>("bt-export");

        //Default value is loaded for lengthField
        lengthField.SetValue(exportSettings.videoLength);

        //Events are tied to the controls to update values
        lengthField.onValueUpdateEvent += UpdateLengthValue;
        videoAmountField.onValueUpdateEvent += UpdateAmountValue;
        mixAnomalyToggle.RegisterValueChangedCallback(x => UpdateAnomalyMix(x.currentTarget as Toggle));
        rbgOutType.RegisterValueChangedCallback(x => UpdateOutputType(x.currentTarget as RadioButtonGroup));

        //The elements are added to our static collection of elements
        MenuElementCollection.ExportElements.videoAmountField = videoAmountField;
        MenuElementCollection.ExportElements.videoLengthField = lengthField;
        MenuElementCollection.ExportElements.mixAnomalyToggle = mixAnomalyToggle;
        MenuElementCollection.ExportElements.outputTypeButtons = rbgOutType;
    }

    void DoExport()
    {
        /*Idea was to pass a string along with the output folder, however this is not implemented in this version
         * instead this version passes an empty string and uses a default folder*/
        TryEvent(onExportClickedEvent, "");
    }

    /* All our export information is stored in a holder class called ExportSetting, the values for this are updated in
     * the following functions */
    void UpdateOutputType(RadioButtonGroup rbg)
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

    void UpdateAnomalyMix(Toggle toggle)
    {
        exportSettings.mixAnomalies = toggle.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    void UpdateLengthValue(NumberField numberField)
    {
        exportSettings.videoLength = (int)numberField.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    void UpdateAmountValue(NumberField numberField)
    {
        exportSettings.videoAmount = (int)numberField.value;
        TryEvent(onExportSettingUpdateEvent);
    }

    //
    //    Tab functions
    //

    /* this function is hooked up to the tab buttons in our settings panel, each button passes along a TabType when pressed
     * based on the type passed we then change to the coresponding tab here*/
    void SwitchSettingTab(SettingTabButton.TabType tab)
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].DisplayIfType(tab);
        }

        for (int i = 0; i < tabElements.Count; i++)
        {
            tabElements[i].DisplayIfType(tab);
        }
    }

    //Our function for creating and tieing up functionallity for all elements in the tabs of our settings panel
    void CreateTabs() 
    {
        //
        //Creating anomaly tab
        VisualElement anomalyList = new VisualElement();
        anomalyControllers = new List<AnomalyController>();
        /* Anomaly controllers are custom UI Elements used for controlling the options for our anomalies, they are created
         * with the use of a custom layout we instantiate, this is then filled with information that has been set in the inspector,
         * for anomalyOptions. All these elements are then added to the VisualElement anomalyList that represents the tab*/
        for (int i = 0; i < anomalyOptions.Count; i++)
        {
            VisualElement anomaly = anomolyController.Instantiate();
            AnomalyController controller = new AnomalyController(anomaly, anomalyOptions[i]);
            controller.onControllerChangedEvent += UpdateAnomalyValue;
            anomalyControllers.Add(controller);
            anomalyList.hierarchy.Add(anomaly);
        }
        tabElements.Add(new TabElement(anomalyList, SettingTabButton.TabType.Anomalies));
        tabMenuElement.Add(anomalyList);

        MenuElementCollection.AnomalyOptionElements.anomalyControllers = anomalyControllers;

        //
        //Creating traffic tab
        /* Traffic setting controllers use the same concept as anomaly controllers, we have a custom UI element, 
         * we instanciate versions of this based on information filled out in the inspector. We then add these to a VisualElement
         * that represents the tab.*/
        VisualElement trafficSettingList = new VisualElement();
        trafficSettingControllers = new List<TrafficSettingController>();
        for (int i = 0; i < trafficSettings.Count; i++)
        {
            VisualElement setting = trafficSettingController.Instantiate();
            TrafficSettingController controller = new TrafficSettingController(setting, trafficSettings[i]);
            controller.onControllerChangedEvent += UpdateTrafficValue;
            trafficSettingControllers.Add(controller);
            trafficSettingList.hierarchy.Add(setting);
        }
        tabElements.Add(new TabElement(trafficSettingList, SettingTabButton.TabType.Traffic));
        tabMenuElement.Add(trafficSettingList);

        MenuElementCollection.TrafficSettingElements.trafficSettingControllers = trafficSettingControllers;

        //
        //Creating lighting tab
        /* The lighting tab does not have modular elements and as such it is simply instanciated from a custom layout.
         * From here elements are simply tied to functionallity*/
        VisualElement lightingElement = lightingTab.Instantiate();
        Slider ambientLight = lightingElement.Q<Slider>("slider-ambient");
        ambientLight.value = lightingSettings.ambient;
        ambientLight.RegisterValueChangedCallback(x => UpdateAmbient(x.newValue));
        Slider intensityLight = lightingElement.Q<Slider>("slider-intensity");
        intensityLight.value = lightingSettings.intensity;
        intensityLight.RegisterValueChangedCallback(x => UpdateIntensity(x.newValue));

        //Out custem UI Element VectorFieldController, here used for controlling direction of light
        VectorFieldController directionController = new VectorFieldController(lightingElement, "tf-x", "tf-y", "tf-z", false);
        directionController.onVectorUpdateEvent += UpdateLightDirection;
        
        //These fields were implemented, but are now not used, so we hide them
        lightingElement.Q<TextField>("tf-z").style.display = DisplayStyle.None;
        ambientLight.style.display = DisplayStyle.None;
        intensityLight.style.display = DisplayStyle.None;

        //Out shadow color controls make use of a VectorController and a seperate NumberField as it needed four fields
        VectorFieldController shadowVectorController = new VectorFieldController(lightingElement, "tf-hue", "tf-saturation", "tf-velocity", false, "ColorVector");
        NumberField alphaField = new NumberField(lightingElement.Q<TextField>("tf-alpha"), false);

        shadowVectorController.onVectorUpdateEvent += UpdateShadowColor;
        alphaField.onValueUpdateEvent += UpdateShadowAlpha;

        tabElements.Add(new TabElement(lightingElement, SettingTabButton.TabType.Light));
        tabMenuElement.Add(lightingElement);

        MenuElementCollection.LightingElements.intensitySlider = intensityLight;
        MenuElementCollection.LightingElements.ambientSlider = ambientLight;
        MenuElementCollection.LightingElements.directionController = directionController;
        MenuElementCollection.LightingElements.shadowController = shadowVectorController;
        MenuElementCollection.LightingElements.alphaField = alphaField;

        //
        // Creating road setting tab
        /* Use settings use same concept as AnomalyOptions and TrafficSettings tab. A custom UI element for a controller
         * is instanciated a number of times based on values set in the inspector*/
        VisualElement roadSettingElement = roadSettingTabLayout.Instantiate();
        NumberField lengthField = new NumberField(roadSettingElement.Q<TextField>("nf-road-length"), false);
        lengthField.onValueUpdateEvent += UpdateRoadLength;
        roadSettingControllers = new List<RoadSettingController>();
        for(int i = 0; i < roadSettings.Count; i++)
        {
            VisualElement setting = roadSettingController.Instantiate();
            RoadSettingController controller;
            if (roadSettings[i].useSlider) controller = new RoadSettingSliderController(setting, roadSettings[i]);
            else controller = new RoadSettingController(setting, roadSettings[i]);
            controller.onControllerChangedEvent += UpdateRoadValue;
            roadSettingControllers.Add(controller);
            roadSettingElement.hierarchy.Add(setting);
        }
        tabElements.Add(new TabElement(roadSettingElement, SettingTabButton.TabType.Road));
        tabMenuElement.Add(roadSettingElement);
    }
    
    void UpdateShadowAlpha(NumberField field)
    {
        UpdateShadowColor(new Vector3(field.value, 0f, 0f), "Shadow");
        TryEvent(onLightingSettingUpdateEvent);
    }
    void UpdateShadowColor(Vector3 vector, string name)
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
    void UpdateLightDirection(Vector3 vector, string name)
    {
        lightingSettings.direction = vector;
        TryEvent(onLightingSettingUpdateEvent);
    }
    void UpdateAmbient(float value)
    {
        lightingSettings.ambient = value;
        TryEvent(onLightingSettingUpdateEvent);
    }
    void UpdateIntensity(float value)
    {
        lightingSettings.intensity = value;
        TryEvent(onLightingSettingUpdateEvent);
    }

    void UpdateAnomalyValue(AnomalyController controller)
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

    void UpdateTrafficValue(TrafficSettingController controller)
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

    void UpdateRoadValue(RoadSettingController controller)
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

    void UpdateRoadLength(NumberField field)
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

