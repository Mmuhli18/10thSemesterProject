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

    [Header("Stuff to make it work")]
    [SerializeField]
    VisualTreeAsset menuLayout;

    [SerializeField]
    VisualTreeAsset anomolyController;

    [SerializeField]
    VisualTreeAsset trafficSettingController;

    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private ListView anomalyList;
    private ListView trafficSettingList;
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

        /*for(int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].button.RegisterCallback<MouseUpEvent>(x => SwitchSettingTab(tabButtons[i].buttonTab));

        }*/
        OpenTrafficTab();
        SwitchSettingTab(SettingTabButton.TabType.Traffic);
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
        }
    }

    void CloseAllTabs()
    {
        if(anomalyList != null)
            anomalyList.style.display = DisplayStyle.None;
        if (trafficSettingList != null)
            trafficSettingList.style.display = DisplayStyle.None;
    }

    void OpenAnomalyTab()
    {
        if (anomalyList == null)
        {
            anomalyList = new ListView();
            for (int i = 0; i < anomalyOptions.Count; i++)
            {
                VisualElement anomaly = anomolyController.Instantiate();
                anomaly.Q<Label>("l-anomaly-name").text = anomalyOptions[i].name;
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

    void OpenTrafficTab()
    {
        Debug.Log("lolololol");
        if(trafficSettingList == null)
        {
            trafficSettingList = new ListView();
            for(int i = 0; i < trafficSettings.Count; i++)
            {
                VisualElement setting = trafficSettingController.Instantiate();
                setting.Q<Slider>("slider").label = trafficSettings[i].name;
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
public struct AnomalyOption
{
    public int value;
    public string name;
}

[Serializable]
public struct TrafficSetting
{
    public int value;
    public string name;
}