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

    void Start()
    {
        ListView anomalyList = new ListView();
        UIDoc = GetComponent<UIDocument>();
        tabMenuElement = UIDoc.rootVisualElement.Q<VisualElement>("settings-window");
        for(int i = 0; i < anomalyOptions.Count; i++)
        {
            VisualElement anomaly = anomolyController.Instantiate();
            anomaly.Q<Label>("l-anomaly-name").text = anomalyOptions[i].name;
            anomalyList.hierarchy.Add(anomaly);
        }
        tabMenuElement.Add(anomalyList);
    }
}

[Serializable]
public struct AnomalyOption
{
    public int value;
    public string name;
}