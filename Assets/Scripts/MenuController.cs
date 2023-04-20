using System.Collections;
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
    public Action onExportClickedEvent;
    public Action onRoadLengthUpdateEvent;
    

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

    private OpenFSpyFromUnity fSpy;
    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();
    private List<TabElement> tabElements = new List<TabElement>();
    private List<AnomalyController> anomalyControllers;
    private List<TrafficSettingController> trafficSettingControllers;
    private List<RoadSettingController> roadSettingControllers;

    void Start()
    {
        NumberField.instance = this;
        UIDoc = GetComponent<UIDocument>();
        fSpy = GetComponent<OpenFSpyFromUnity>();
        LoadFSpy._camera = viewportHandler.viewportCam;
        tabMenuElement = UIDoc.rootVisualElement.Q<VisualElement>("settings-window");
        VisualElement tabs = UIDoc.rootVisualElement.Q<VisualElement>("tabs");

        tabButtons.Add(new SettingTabButton(tabs, "tab-anomalies", SettingTabButton.TabType.Anomalies));
        tabButtons.Add(new SettingTabButton(tabs, "tab-traffic", SettingTabButton.TabType.Traffic));
        tabButtons.Add(new SettingTabButton(tabs, "tab-lighting", SettingTabButton.TabType.Light));
        tabButtons.Add(new SettingTabButton(tabs, "tab-road", SettingTabButton.TabType.Road));
        for(int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].onPressEvent += SwitchSettingTab;
        }

        CreateTabs();

        SwitchSettingTab(SettingTabButton.TabType.Anomalies);

        UIDoc.rootVisualElement.Q<Button>("bt-add-footage").RegisterCallback<MouseUpEvent>(x => viewportHandler.AddFootage(x.currentTarget as Button));
        UIDoc.rootVisualElement.Q<Button>("bt-draw-foreground").RegisterCallback<MouseUpEvent>(x => pointController.AddMarking());
        UIDoc.rootVisualElement.Q<Button>("bt-export").RegisterCallback<MouseUpEvent>(x => TryEvent(onExportClickedEvent));
        UIDoc.rootVisualElement.Q<Button>("bt-load-data").RegisterCallback<MouseUpEvent>(x => viewportHandler.LoadFSpy(fSpy));
        UIDoc.rootVisualElement.Q<Button>("bt-open-fspy").RegisterCallback<MouseUpEvent>(x => fSpy.OpenFSpy());

        SetupExportUI();
        SetupTransformMenu();
        onChangeEvent += viewportHandler.RenderPreviewSprite;
    }

    private void Update()
    {
        if (RenderTesting)
        {
            GetMask();
            RenderTesting = false;
            viewportHandler.RenderPreviewSprite();
        }
    }

    void TryEvent(Action eventAction)
    {
        if(eventAction != null && eventAction.Method != null)
        {
            eventAction.Invoke();
        }
        else
        {
            if(actionWarnings && eventAction != onChangeEvent) Debug.LogWarning("No action tied to event");
            else if(actionWarnings) Debug.LogWarning("No action tied to onChangeEvent");
        }
        if (eventAction != onChangeEvent)
        {
            TryEvent(onChangeEvent);
        }
    }

    //
    //Transform menu functions
    //

    void SetupTransformMenu()
    {
        NumberField scaleField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-scale"), false);
        Button resetButton = UIDoc.rootVisualElement.Q<Button>("bt-reset");

        VectorFieldController vectorFieldPosition = new VectorFieldController(UIDoc.rootVisualElement, "tf-pos-x", "tf-pos-y", "tf-pos-z", true, "Position");
        VectorFieldController vectorFieldRotation = new VectorFieldController(UIDoc.rootVisualElement, "tf-rot-x", "tf-rot-y", "tf-rot-z", true, "Rotation");

        scaleField.SetValue(80);

        vectorFieldPosition.onVectorUpdateEvent += UpdateRoadTransform;
        vectorFieldRotation.onVectorUpdateEvent += UpdateRoadTransform;
        scaleField.onValueUpdateEvent += UpdateRoadScale;
        resetButton.clicked += MenuElementCollection.TransformElements.ResetValues;

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
        
        NumberField lengthField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-length"), false);
        NumberField videoAmountField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-amount"), false);
        Toggle mixAnomalyToggle = UIDoc.rootVisualElement.Q<Toggle>("tg-mix-anomalies");
        RadioButtonGroup rbgOutType = UIDoc.rootVisualElement.Q<RadioButtonGroup>("rbg-output-type");
        Button exportButton = UIDoc.rootVisualElement.Q<Button>("bt-export");

        lengthField.SetValue(exportSettings.videoLength);

        lengthField.onValueUpdateEvent += UpdateLengthValue;
        videoAmountField.onValueUpdateEvent += UpdateAmountValue;
        mixAnomalyToggle.RegisterValueChangedCallback(x => UpdateAnomalyMix(x.currentTarget as Toggle));
        rbgOutType.RegisterValueChangedCallback(x => UpdateOutputType(x.currentTarget as RadioButtonGroup));

        MenuElementCollection.ExportElements.videoAmountField = videoAmountField;
        MenuElementCollection.ExportElements.videoLengthField = lengthField;
        MenuElementCollection.ExportElements.mixAnomalyToggle = mixAnomalyToggle;
        MenuElementCollection.ExportElements.outputTypeButtons = rbgOutType;
    }

    void UpdateOutputType(RadioButtonGroup rbg)
    {
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

    void CreateTabs()
    {
        //
        //Creating anomaly tab
        VisualElement anomalyList = new VisualElement();
        anomalyControllers = new List<AnomalyController>();
        for (int i = 0; i < anomalyOptions.Count; i++)
        {
            //if you are marco, good luck lmao this is a lost cause to understand lmao
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
        //creating lighting tab, this one is simple hihi, I updateded this, it is no longer simple haha
        VisualElement lightingElement = lightingTab.Instantiate();
        Slider ambientLight = lightingElement.Q<Slider>("slider-ambient");
        ambientLight.value = lightingSettings.ambient;
        ambientLight.RegisterValueChangedCallback(x => UpdateAmbient(x.newValue));
        Slider intensityLight = lightingElement.Q<Slider>("slider-intensity");
        intensityLight.value = lightingSettings.intensity;
        intensityLight.RegisterValueChangedCallback(x => UpdateIntensity(x.newValue));

        VectorFieldController directionController = new VectorFieldController(lightingElement, "tf-x", "tf-y", "tf-z");
        directionController.onVectorUpdateEvent += UpdateLightDirection;
        
        //These field were implemented, but are now not used, so we hide them
        lightingElement.Q<TextField>("tf-z").style.display = DisplayStyle.None;
        ambientLight.style.display = DisplayStyle.None;
        intensityLight.style.display = DisplayStyle.None;

        VectorFieldController shadowVectorController = new VectorFieldController(lightingElement, "tf-hue", "tf-saturation", "tf-velocity", false, "ColorVector");
        NumberField alphaField = new NumberField(lightingElement.Q<TextField>("tf-alpha"));

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
        return anomalyOptions;
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
        return viewportHandler.RenderMask();
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
        return roadSettings;
    }

    public float GetRoadLength()
    {
        return roadLength;
    }
}

//
//    Serializable classes
//

[Serializable]
public class AnomalyOption
{
    public float value;
    public string name;
    public bool active;
}

[Serializable]
public class TrafficSetting
{
    public float value;
    public string name;
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
public  class RoadSetting
{
    public float leftValue;
    public float rightValue;
    public string name;
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

