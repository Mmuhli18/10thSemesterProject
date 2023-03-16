using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MenuController : MonoBehaviour
{
    [Header("Stuff for designers")]
    [SerializeField]
    List<AnomalyOption> anomalyOptions = new List<AnomalyOption>();

    [SerializeField]
    List<TrafficSetting> trafficSettings = new List<TrafficSetting>();

    [SerializeField]
    LightingSetting lightingSettings = new LightingSetting();

    [Header("Stuff to make it work")]
    [SerializeField]
    VisualTreeAsset menuLayout;

    [SerializeField]
    ViewportHandler viewportHandler;

    [SerializeField]
    VisualTreeAsset anomolyController;

    [SerializeField]
    VisualTreeAsset trafficSettingController;

    [SerializeField]
    VisualTreeAsset lightingTab;


    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();
    private List<TabElement> tabElements = new List<TabElement>();
    private SettingTabButton.TabType activeTab;

    void Start()
    {
        UIDoc = GetComponent<UIDocument>();
        tabMenuElement = UIDoc.rootVisualElement.Q<VisualElement>("settings-window");
        VisualElement tabs = UIDoc.rootVisualElement.Q<VisualElement>("tabs");

        tabButtons.Add(new SettingTabButton(tabs.Q<Button>("tab-anomalies"), SettingTabButton.TabType.Anomalies));
        tabButtons[0].button.RegisterCallback<MouseUpEvent>(x => SwitchSettingTab(tabButtons[0].buttonTab));

        tabButtons.Add(new SettingTabButton(tabs.Q<Button>("tab-traffic"), SettingTabButton.TabType.Traffic));
        tabButtons[1].button.RegisterCallback<MouseUpEvent>(x => SwitchSettingTab(tabButtons[1].buttonTab));

        tabButtons.Add(new SettingTabButton(tabs.Q<Button>("tab-lighting"), SettingTabButton.TabType.Light));
        tabButtons[2].button.RegisterCallback<MouseUpEvent>(x => SwitchSettingTab(tabButtons[2].buttonTab));

        CreateTabs();

        SwitchSettingTab(SettingTabButton.TabType.Anomalies);

        UIDoc.rootVisualElement.Q<Button>("bt-add-footage").RegisterCallback<MouseUpEvent>(x => viewportHandler.AddFootage(x.currentTarget as Button));
    }

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

    void SwitchSettingTab(SettingTabButton.TabType tab)
    {
        for(int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].DisplayIfType(tab);
        }

        for(int i = 0; i < tabElements.Count; i++)
        {
            tabElements[i].DisplayIfType(tab);
        }
        activeTab = tab;
    }

    void CreateTabs()
    {
        //Creating anomaly tab
        ListView anomalyList = new ListView();
        for (int i = 0; i < anomalyOptions.Count; i++)
        {
            //if you are marco, good luck lmao this is a lost cause to understand lmao
            VisualElement anomaly = anomolyController.Instantiate();
            anomaly.Q<Label>("l-anomaly-name").text = anomalyOptions[i].name;
            Slider anomalySlider = anomaly.Q<Slider>("anomaly-slider");
            anomalySlider.bindingPath = i.ToString();
            anomalySlider.value = anomalyOptions[i].value;
            anomalySlider.RegisterValueChangedCallback(x => UpdateAnomalyValue(x.currentTarget as Slider));
            Toggle anomalyToggle = anomaly.Q<Toggle>("anomaly-toggle");
            anomalyToggle.bindingPath = i.ToString();
            anomalyToggle.value = anomalyOptions[i].active;
            anomalyToggle.RegisterValueChangedCallback(x => UpdateAnomalyValue(x.currentTarget as Toggle));
            anomalyList.hierarchy.Add(anomaly);
        }
        tabElements.Add(new TabElement(anomalyList, SettingTabButton.TabType.Anomalies));
        tabMenuElement.Add(anomalyList);


        //Creating traffic tab
        ListView trafficSettingList = new ListView();
        for (int i = 0; i < trafficSettings.Count; i++)
        {
            VisualElement setting = trafficSettingController.Instantiate();
            Slider trafficSlider = setting.Q<Slider>("traffic-slider");
            trafficSlider.label = trafficSettings[i].name;
            trafficSlider.bindingPath = i.ToString();
            trafficSlider.value = trafficSettings[i].value;
            trafficSlider.RegisterValueChangedCallback(x => UpdateTrafficValue(x.currentTarget as Slider));
            trafficSettingList.hierarchy.Add(setting);
        }
        tabElements.Add(new TabElement(trafficSettingList, SettingTabButton.TabType.Traffic));
        tabMenuElement.Add(trafficSettingList);


        //creating lighting tab, this one is simple hihi
        VisualElement lightingElement = lightingTab.Instantiate();
        Slider ambientLight = lightingElement.Q<Slider>("slider-ambient");
        ambientLight.value = lightingSettings.ambient;
        ambientLight.RegisterValueChangedCallback(x => UpdateAmbient(x.newValue));
        Slider intensityLight = lightingElement.Q<Slider>("slider-intensity");
        intensityLight.value = lightingSettings.intensity;
        intensityLight.RegisterValueChangedCallback(x => UpdateIntensity(x.newValue));
        tabElements.Add(new TabElement(lightingElement, SettingTabButton.TabType.Light));
        tabMenuElement.Add(lightingElement);
    }

    //
    //     Slider functions
    //
    void UpdateAnomalyValue(Slider slider)
    {
        anomalyOptions[int.Parse(slider.bindingPath)].value = slider.value;
    }
    void UpdateAnomalyValue(Toggle toggle)
    {
        anomalyOptions[int.Parse(toggle.bindingPath)].active = toggle.value;
    }
    void UpdateAmbient(float value)
    {
        lightingSettings.ambient = value;
    }
    void UpdateIntensity(float value)
    {
        lightingSettings.intensity = value;
    }
    void UpdateTrafficValue(Slider slider)
    {
        trafficSettings[int.Parse(slider.bindingPath)].value = slider.value;
    }

}

public class SettingTabButton
{
    public TabType buttonTab;
    public Button button;
    public VisualElement darkner;

    public SettingTabButton(Button b, TabType tab)
    {
        button = b;
        darkner = button.Q<VisualElement>("tab-darkner");
        buttonTab = tab;
    }

    public void DisplayIfType(TabType tab)
    {
        if (tab == buttonTab) darkner.style.display = DisplayStyle.None;
        else darkner.style.display = DisplayStyle.Flex;
    }

    public enum TabType
    {
        Anomalies,
        Traffic,
        Light
    }
}

public class TabElement
{
    public VisualElement visualElement;
    public SettingTabButton.TabType tabType;

    public TabElement(VisualElement element, SettingTabButton.TabType type)
    {
        visualElement = element;
        tabType = type;
    }

    public void DisplayIfType(SettingTabButton.TabType type)
    {
        try
        {
            if (type == tabType) visualElement.style.display = DisplayStyle.Flex;
            else visualElement.style.display = DisplayStyle.None;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

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
}


[Serializable]
public class LightingSetting
{
    public float intensity;
    public float ambient;
}

