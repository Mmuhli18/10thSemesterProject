using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MenuController : MonoBehaviour
{
    [Header("Stuff for designers")]
    [SerializeField]
    public List<AnomalyOption> anomalyOptions = new List<AnomalyOption>();

    [SerializeField]
    public List<TrafficSetting> trafficSettings = new List<TrafficSetting>();

    [SerializeField]
    public LightingSetting lightingSettings = new LightingSetting();

    [Header("Stuff to make it work")]
    [SerializeField]
    VisualTreeAsset menuLayout;

    [SerializeField]
    VisualTreeAsset anomolyController;

    [SerializeField]
    VisualTreeAsset trafficSettingController;

    [SerializeField]
    VisualTreeAsset lightingTab;

    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private ListView anomalyList;
    private ListView trafficSettingList;
    private VisualElement lightingElement;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();
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

        OpenAnomalyTab();
        SwitchSettingTab(SettingTabButton.TabType.Anomalies);
    }

    void SwitchSettingTab(SettingTabButton.TabType tab)
    {
        for(int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].darkner.style.opacity = new StyleFloat(100);
        }
        tabButtons[(int)tab].darkner.style.opacity = new StyleFloat(0.0);
        CloseAllTabs();
        switch (tab)
        {
            case SettingTabButton.TabType.Anomalies:
                OpenAnomalyTab();
                break;
            case SettingTabButton.TabType.Traffic:
                OpenTrafficTab();
                break;
            case SettingTabButton.TabType.Light:
                OpenLightingTab();
                break;
        }
    }

    void CloseAllTabs()
    {
        if(anomalyList != null)
            anomalyList.style.display = DisplayStyle.None;
        if (trafficSettingList != null)
            trafficSettingList.style.display = DisplayStyle.None;
        if (lightingElement != null)
            lightingElement.style.display = DisplayStyle.None;
    }

    void OpenAnomalyTab()
    {
        if (anomalyList == null)
        {
            anomalyList = new ListView();
            for (int i = 0; i < anomalyOptions.Count; i++)
            {
                //if you are marco, good luck lmao this is a lost cause to understand lmao
                VisualElement anomaly = anomolyController.Instantiate();
                anomaly.Q<Label>("l-anomaly-name").text = anomalyOptions[i].name;
                Slider anomalySlider = anomaly.Q<Slider>("anomaly-slider");
                anomalySlider.bindingPath = i.ToString();
                anomalySlider.value = anomalyOptions[i].value;
                anomalySlider.RegisterValueChangedCallback(x => UpdateAnomalyValue(x.currentTarget as Slider));
                anomalyList.hierarchy.Add(anomaly);
            }
            tabMenuElement.Add(anomalyList);
        }

        if (anomalyList.style.display != DisplayStyle.Flex)
        {
            anomalyList.style.display = DisplayStyle.Flex;
        }

        activeTab = SettingTabButton.TabType.Anomalies;
    }

    void UpdateAnomalyValue(Slider slider)
    {
        anomalyOptions[int.Parse(slider.bindingPath)].value = slider.value;
    }

    void OpenTrafficTab()
    {
        if(trafficSettingList == null)
        {
            trafficSettingList = new ListView();
            for(int i = 0; i < trafficSettings.Count; i++)
            {
                VisualElement setting = trafficSettingController.Instantiate();
                Slider trafficSlider = setting.Q<Slider>("traffic-slider");
                trafficSlider.label = trafficSettings[i].name;
                trafficSlider.bindingPath = i.ToString();
                trafficSlider.value = trafficSettings[i].value;
                trafficSlider.RegisterValueChangedCallback(x => UpdateTrafficValue(x.currentTarget as Slider));
                trafficSettingList.hierarchy.Add(setting);
            }
            tabMenuElement.Add(trafficSettingList);
        }

        if(trafficSettingList.style.display != DisplayStyle.Flex)
        {
            trafficSettingList.style.display = DisplayStyle.Flex;
        }

        activeTab = SettingTabButton.TabType.Traffic;
    }

    void UpdateTrafficValue(Slider slider)
    {
        trafficSettings[int.Parse(slider.bindingPath)].value = slider.value;
    }

    void OpenLightingTab()
    {
        if(lightingElement == null)
        {
            lightingElement = lightingTab.Instantiate();
            tabMenuElement.Add(lightingElement);
        }

        if(lightingElement.style.display != DisplayStyle.Flex)
        {
            lightingElement.style.display = DisplayStyle.Flex;
        }

        activeTab = SettingTabButton.TabType.Light;
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

    public enum TabType
    {
        Anomalies,
        Traffic,
        Light
    }
}



[Serializable]
public class AnomalyOption
{
    public float value;
    public string name;
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