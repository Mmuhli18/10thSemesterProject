using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CustomUIClasses;
using System.Linq;

[RequireComponent(typeof(OpenFSpyFromUnity)), RequireComponent(typeof(MenuController))]
public class MenuUI : MonoBehaviour
{
    //Static stables
    public static UIDocument UIDoc;
    public static MenuController menuController;

    //Layouts
    [Header("Layouts")]
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

    //Dynamic stuff
    private VisualElement exportMenuElement;
    private Button gotoExportButton;
    private VisualElement tabMenuElement;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();
    private List<TabElement> tabElements = new List<TabElement>();

    public Button playPauseButton;

    public Button drawForegroundButton;
    public Button addMarkingButton;

    public void SetupMenu()
    {
        menuController = GetComponent<MenuController>();
        UIDoc = MenuController.UIDoc;

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
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].onPressEvent += SwitchSettingTab;
        }

        OpenFSpyFromUnity fSpy = GetComponent<OpenFSpyFromUnity>();

        //Functionallity for different buttons in the layout is defined
        UIDoc.rootVisualElement.Q<Button>("bt-add-footage").RegisterCallback<MouseUpEvent>(x => menuController.viewportHandler.AddFootage(x.currentTarget as Button));
        drawForegroundButton = UIDoc.rootVisualElement.Q<Button>("bt-draw-foreground");
        drawForegroundButton.RegisterCallback<MouseUpEvent>(x => menuController.pointController.SwitchDrawingMode());
        addMarkingButton = UIDoc.rootVisualElement.Q<Button>("bt-add-marking");
        addMarkingButton.RegisterCallback<MouseUpEvent>(x => menuController.pointController.AddMarking());
        addMarkingButton.style.display = DisplayStyle.None;
        UIDoc.rootVisualElement.Q<Button>("bt-export").RegisterCallback<MouseUpEvent>(x => menuController.DoExport());
        UIDoc.rootVisualElement.Q<Button>("bt-load-data").RegisterCallback<MouseUpEvent>(x => menuController.viewportHandler.LoadFSpy(fSpy));
        UIDoc.rootVisualElement.Q<Button>("bt-open-fspy").RegisterCallback<MouseUpEvent>(x => fSpy.OpenFSpy());
        playPauseButton = UIDoc.rootVisualElement.Q<Button>("bt-play-pause");
        playPauseButton.RegisterCallback<MouseUpEvent>(x => menuController.PlayPauseSimulation());

        CreateTabs();
        //Default tab of anomaly options is switched to
        SwitchSettingTab(SettingTabButton.TabType.Anomalies);

        SetupExportUI();
        SetupTransformMenu();

        //Loading up tooltips
        var tips = menuController.tipCollection.GetElementTiedTips();
        for (int i = 0; i < tips.Count; i++)
        {
            new ToolTip(UIDoc.rootVisualElement.Q(tips[i].element), tips[i].tip, tips[i].alignment);
        }
    }

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

        /* Anomaly controllers are custom UI Elements used for controlling the options for our anomalies, they are created
         * with the use of a custom layout we instantiate, this is then filled with information that has been set in the inspector,
         * for anomalyOptions. All these elements are then added to the VisualElement anomalyList that represents the tab*/
        var anomalyOptions = menuController.GetAnomalies().ConvertAll(x => x as BaseNamedSetting);
        SerializedDataController<AnomalyController> anomaliesController = new SerializedDataController<AnomalyController>(anomolyController, anomalyOptions, menuController.UpdateAnomalyValue);
        tabElements.Add(new TabElement(anomaliesController.holderElement, SettingTabButton.TabType.Anomalies));
        tabMenuElement.Add(anomaliesController.holderElement);

        MenuElementCollection.AnomalyOptionElements.anomalyControllers = anomaliesController.controllers;

        //
        //Creating traffic tab
        /* Traffic setting controllers use the same concept as anomaly controllers, we have a custom UI element, 
         * we instanciate versions of this based on information filled out in the inspector. We then add these to a VisualElement
         * that represents the tab.*/
        var trafficSettings = menuController.GetTrafficSettings().ConvertAll(x => x as BaseNamedSetting);
        SerializedDataController<TrafficSettingController> trafficSettingCon = new SerializedDataController<TrafficSettingController>(trafficSettingController, trafficSettings, menuController.UpdateTrafficValue);
        tabElements.Add(new TabElement(trafficSettingCon.holderElement, SettingTabButton.TabType.Traffic));
        tabMenuElement.Add(trafficSettingCon.holderElement);

        MenuElementCollection.TrafficSettingElements.trafficSettingControllers = trafficSettingCon.controllers;

        //
        //Creating lighting tab
        /* The lighting tab does not have modular elements and as such it is simply instanciated from a custom layout.
         * From here elements are simply tied to functionallity*/
        VisualElement lightingElement = lightingTab.Instantiate();
        LightingSetting lightingSettings = menuController.GetLightingSettings();
        Slider ambientLight = lightingElement.Q<Slider>("slider-ambient");
        ambientLight.value = lightingSettings.ambient;
        ambientLight.RegisterValueChangedCallback(x => menuController.UpdateAmbient(x.newValue));
        Slider intensityLight = lightingElement.Q<Slider>("slider-intensity");
        intensityLight.value = lightingSettings.intensity;
        intensityLight.RegisterValueChangedCallback(x => menuController.UpdateIntensity(x.newValue));

        //Our custem UI Element VectorFieldController, here used for controlling direction of light
        VectorFieldController directionController = new VectorFieldController(lightingElement, "tf-x", "tf-y", "tf-z", false);
        directionController.onVectorUpdateEvent += menuController.UpdateLightDirection;

        //These fields were implemented, but are now not used, so we hide them
        lightingElement.Q<TextField>("tf-z").style.display = DisplayStyle.None;
        ambientLight.style.display = DisplayStyle.None;
        intensityLight.style.display = DisplayStyle.None;

        //Out shadow color controls make use of a VectorController and a seperate NumberField as it needed four fields
        VectorFieldController shadowVectorController = new VectorFieldController(lightingElement, "tf-hue", "tf-saturation", "tf-velocity", false, "ColorVector");
        NumberField alphaField = new NumberField(lightingElement.Q<TextField>("tf-alpha"), false);

        shadowVectorController.onVectorUpdateEvent += menuController.UpdateShadowColor;
        alphaField.onValueUpdateEvent += menuController.UpdateShadowAlpha;

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
        lengthField.onValueUpdateEvent += menuController.UpdateRoadLength;

        var roadSettings = menuController.GetRoadSettings().ConvertAll(x => x as BaseNamedSetting);
        SerializedDataController<RoadSettingController> roadSettingCon = new SerializedDataController<RoadSettingController>(roadSettingController, roadSettings, menuController.UpdateRoadValue, roadSettingElement);
        tabElements.Add(new TabElement(roadSettingCon.holderElement, SettingTabButton.TabType.Road));
        tabMenuElement.Add(roadSettingCon.holderElement);

        MenuElementCollection.RoadSettingElements.roadSettingControllers = roadSettingCon.controllers;
    }

    void SetupTransformMenu()
    {
        NumberField scaleField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-scale"), false);
        Button resetButton = UIDoc.rootVisualElement.Q<Button>("bt-reset");

        //VectorFieldController is one of our custom UI classes. Two are defined here
        //These are the vector controllers that the user uses for moving the road/camera manually
        VectorFieldController vectorFieldPosition = new VectorFieldController(UIDoc.rootVisualElement, "tf-pos-x", "tf-pos-y", "tf-pos-z", true, "Position");
        VectorFieldController vectorFieldRotation = new VectorFieldController(UIDoc.rootVisualElement, "tf-rot-x", "tf-rot-y", "tf-rot-z", true, "Rotation");

        scaleField.SetValue(80);

        vectorFieldPosition.onVectorUpdateEvent += menuController.UpdateRoadTransform;
        vectorFieldRotation.onVectorUpdateEvent += menuController.UpdateRoadTransform;
        scaleField.onValueUpdateEvent += menuController.UpdateRoadScale;
        resetButton.clicked += MenuElementCollection.TransformElements.ResetValues;

        //MenuElementCollection is a static class used to defined the Visual elements in the menu. This is used for loading values into the menu
        MenuElementCollection.TransformElements.positionController = vectorFieldPosition;
        MenuElementCollection.TransformElements.rotationController = vectorFieldRotation;
        MenuElementCollection.TransformElements.scaleField = scaleField;
    }

    void SetupExportUI()
    {
        //Our custom Numberfields
        NumberField lengthField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-length"), false);
        NumberField videoAmountField = new NumberField(UIDoc.rootVisualElement.Q<TextField>("tf-amount"), false);
        //Unity UI.Elements
        Toggle mixAnomalyToggle = UIDoc.rootVisualElement.Q<Toggle>("tg-mix-anomalies");
        RadioButtonGroup rbgOutType = UIDoc.rootVisualElement.Q<RadioButtonGroup>("rbg-output-type");
        Button exportButton = UIDoc.rootVisualElement.Q<Button>("bt-export");
        exportMenuElement = UIDoc.rootVisualElement.Q("export-menu-element");
        exportMenuElement.style.display = DisplayStyle.None;
        gotoExportButton = UIDoc.rootVisualElement.Q<Button>("bt-goto-export");
        gotoExportButton.RegisterCallback<MouseUpEvent>(x => OpenExportMenu());

        //Default value is loaded for lengthField
        lengthField.SetValue(200f);

        //Events are tied to the controls to update values
        lengthField.onValueUpdateEvent += menuController.UpdateLengthValue;
        videoAmountField.onValueUpdateEvent += menuController.UpdateAmountValue;
        mixAnomalyToggle.RegisterValueChangedCallback(x => menuController.UpdateAnomalyMix(x.currentTarget as Toggle));
        rbgOutType.RegisterValueChangedCallback(x => menuController.UpdateOutputType(x.currentTarget as RadioButtonGroup));

        //The elements are added to our static collection of elements
        MenuElementCollection.ExportElements.videoAmountField = videoAmountField;
        MenuElementCollection.ExportElements.videoLengthField = lengthField;
        MenuElementCollection.ExportElements.mixAnomalyToggle = mixAnomalyToggle;
        MenuElementCollection.ExportElements.outputTypeButtons = rbgOutType;
    }

    void OpenExportMenu()
    {
        exportMenuElement.style.display = DisplayStyle.Flex;
        gotoExportButton.style.display = DisplayStyle.None;
    }
}
