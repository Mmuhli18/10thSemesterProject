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

    [Header("Stuff to make it work")]
    [SerializeField]
    VisualTreeAsset menuLayout;

    [SerializeField]
    VisualTreeAsset anomolyController;

    private UIDocument UIDoc;
    private VisualElement tabMenuElement;
    private ListView anomalyList;
    private List<SettingTabButton> tabButtons = new List<SettingTabButton>();

    void Start()
    {
        UIDoc = GetComponent<UIDocument>();
        tabMenuElement = UIDoc.rootVisualElement.Q<VisualElement>("settings-window");

        tabButtons.Add(new SettingTabButton());
        tabButtons[(int)SettingTabButton.TabType.Anomalies].buttonTab = SettingTabButton.TabType.Anomalies;
        tabButtons[(int)SettingTabButton.TabType.Anomalies].button = tabMenuElement.Q<Button>("tab-anomalies");

        tabButtons.Add(new SettingTabButton());
        tabButtons[(int)SettingTabButton.TabType.Traffic].buttonTab = SettingTabButton.TabType.Traffic;
        tabButtons[(int)SettingTabButton.TabType.Traffic].button = tabMenuElement.Q<Button>("tab-traffic");

        tabButtons.Add(new SettingTabButton());
        tabButtons[(int)SettingTabButton.TabType.Light].buttonTab = SettingTabButton.TabType.Light;
        tabButtons[(int)SettingTabButton.TabType.Light].button = tabMenuElement.Q<Button>("tab-lighting");

        OpenAnomalyTab();
    }

    void OpenAnomalyTab()
    {
        if(anomalyList != null)
        {
            anomalyList.style.display = DisplayStyle.Flex;
        }
        else
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
    }
}

public class SettingTabButton
{
    public TabType buttonTab;
    public Button button;

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