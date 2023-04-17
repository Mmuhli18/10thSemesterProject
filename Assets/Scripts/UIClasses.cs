using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class SettingTabButton
{
    public TabType buttonTab;
    Button button;
    public VisualElement darkner;
    public Action<TabType> onPressEvent;

    public SettingTabButton(VisualElement tabs, string name, TabType tab)
    {
        button = tabs.Q<Button>(name);
        button.RegisterCallback<MouseUpEvent>(x => OnClicked());
        darkner = button.Q<VisualElement>("tab-darkner");
        buttonTab = tab;
    }

    void OnClicked()
    {
        try
        {
            onPressEvent.Invoke(buttonTab);
        }
        catch { }
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
        Light,
        Road
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

public class NumberField
{
    public TextField textField { get; private set; }
    VisualElement label;
    float lastXPos;
    float sensitivity = 0.5f;
    public bool allowNegativeNumbers = true;
    public float value { get; private set; }
    public Action<NumberField> onValueUpdateEvent;
    public static MonoBehaviour instance; //instance for doing dragging

    public NumberField(TextField textField, bool allowNegative = true, string name = "")
    {
        allowNegativeNumbers = allowNegative;
        this.textField = textField;
        label = textField.Q<Label>();
        label.RegisterCallback<MouseDownEvent>(x => instance.StartCoroutine(DoMouseDrag()));
        if (name != "") textField.label = name;
        this.textField.RegisterValueChangedCallback(x => SetValue());
    }

    public void SetValue()
    {
        KeepTextFieldAsNumbers();
        value = int.Parse(textField.value);
        try
        {
            onValueUpdateEvent.Invoke(this);
        }
        catch { }
    }

    public void SetValue(float newValue)
    {
        textField.value = newValue.ToString();
        SetValue();
    }

    public void SetValue(int newValue)
    {
        textField.value = newValue.ToString();
        SetValue();
    }

    void KeepTextFieldAsNumbers()
    {
        string tempValue = textField.value;
        string newValue = "";
        bool isNegative = false;
        for (int i = 0; i < tempValue.Length; i++)
        {
            if (int.TryParse(tempValue[i].ToString(), out int intValue))
            {
                newValue += intValue;
            }
            else if (i == 0 && tempValue[i].ToString() == "-")
            {
                newValue += "-";
                isNegative = true;
            }
        }
        if (isNegative && allowNegativeNumbers == false) newValue = "1";
        textField.SetValueWithoutNotify(newValue);
    }

    IEnumerator DoMouseDrag()
    {
        lastXPos = Input.mousePosition.x;
        yield return new WaitUntil(() => MouseSpyware());
    }

    bool MouseSpyware()
    {
        SetValue((int)(value + (Input.mousePosition.x - lastXPos) * sensitivity));
        lastXPos = Input.mousePosition.x;
        return Input.GetMouseButtonUp(0);
    }

}

public class VectorFieldController
{
    NumberField xField;
    NumberField yField;
    NumberField zField;
    public Action<Vector3, string> onVectorUpdateEvent;
    public Vector3 value { get; private set; }
    public string name { get; private set; }

    public VectorFieldController(NumberField x, NumberField y, NumberField z, string name = "")
    {
        this.name = name;
        xField = x;
        yField = y;
        zField = z;

        xField.onValueUpdateEvent += UpdateVector;
        yField.onValueUpdateEvent += UpdateVector;
        zField.onValueUpdateEvent += UpdateVector;
    }

    public VectorFieldController(VisualElement holdingElement, string xName, string yName, string zName, bool allowNegatives = true, string controllerName = "")
    {
        name = controllerName;
        xField = new NumberField(holdingElement.Q<TextField>(xName), allowNegatives);
        yField = new NumberField(holdingElement.Q<TextField>(yName), allowNegatives);
        zField = new NumberField(holdingElement.Q<TextField>(zName), allowNegatives);

        xField.onValueUpdateEvent += UpdateVector;
        yField.onValueUpdateEvent += UpdateVector;
        zField.onValueUpdateEvent += UpdateVector;
    }

    void UpdateVector(NumberField field)
    {
        value = new Vector3(xField.value, yField.value, zField.value);
        onVectorUpdateEvent.Invoke(value, name);
    }

    public void SetValue(Vector3 vector)
    {
        xField.SetValue(vector.x);
        yField.SetValue(vector.y);
        zField.SetValue(vector.z);
    }
}

public class AnomalyController
{
    public string name { get; private set; }
    Slider slider;
    Toggle toggle;
    Label label;
    public float value { get; private set; }
    public bool isActive { get; private set; }
    public Action<AnomalyController> onControllerChangedEvent;
    public AnomalyController(VisualElement controllerElement, AnomalyOption option)
    {
        name = option.name;
        controllerElement.Q<Label>("l-anomaly-name").text = name;
        slider = controllerElement.Q<Slider>("anomaly-slider");
        toggle = controllerElement.Q<Toggle>("anomaly-toggle");
        label = controllerElement.Q<Label>("l-value");

        slider.RegisterValueChangedCallback(x => ValueChangedAction());
        toggle.RegisterValueChangedCallback(x => ValueChangedAction());

        SetValue(option.active, option.value);
        ValueChangedWithoutAction();
    }

    void ValueChangedAction()
    {
        value = slider.value;
        isActive = toggle.value;
        label.text = value.ToString();

        try
        {
            onControllerChangedEvent.Invoke(this);
        }
        catch(Exception e)
        {
            Debug.LogWarning("Value changed event probably not assigned \n" + e);
        }
    }

    void ValueChangedWithoutAction()
    {
        value = slider.value;
        isActive = toggle.value;
        label.text = value.ToString();
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetValue(bool value)
    {
        toggle.value = value;
    }

    public void SetValue(bool bValue, float fValue)
    {
        SetValue(bValue);
        SetValue(fValue);
    }
}

public class TrafficSettingController
{
    public string name { get; private set; }
    Slider slider;
    Label label;
    NumberField leftField;
    NumberField rightField;
    public float value { get; private set; }
    public float offsetLeft { get; private set; }
    public float offsetRight { get; private set; }
    public Action<TrafficSettingController> onControllerChangedEvent;
    public TrafficSettingController(VisualElement controllerElement, TrafficSetting setting)
    {
        name = setting.name;
        //slider
        slider = controllerElement.Q<Slider>("traffic-slider");
        label = controllerElement.Q<Label>("l-value");
        slider.label = setting.name;
        slider.RegisterValueChangedCallback(x => ValueChangedAction());
        
        //offsets
        if (!setting.useOffsets)
        {
            controllerElement.Q<VisualElement>("ve-offset").style.display = DisplayStyle.None;
        }
        else
        {
            leftField = new NumberField(controllerElement.Q<TextField>("tf-left"));
            rightField = new NumberField(controllerElement.Q<TextField>("tf-right"));

            leftField.onValueUpdateEvent += OffsetValueChangedAction;
            rightField.onValueUpdateEvent += OffsetValueChangedAction;
        }
        SetValue(setting);
        ValueChangedWithoutAction();
    }

    void OffsetValueChangedAction(NumberField field)
    {
        ValueChangedAction();
    }

    void ValueChangedAction()
    {
        ValueChangedWithoutAction();
        onControllerChangedEvent.Invoke(this);
    }

    void ValueChangedWithoutAction()
    {
        value = slider.value;
        label.text = value.ToString();
        if (leftField != null) offsetLeft = leftField.value;
        if (rightField != null) offsetRight = rightField.value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetValue(TrafficSetting setting)
    {
        slider.value = setting.value;
        if (leftField != null) leftField.SetValue(setting.offsetLeft);
        if (rightField != null) rightField.SetValue(setting.offsetRight);
    }
}

public class RoadSettingController
{
    public string name { get; private set; }
    public float leftValue { get; private set; }
    public float rightValue { get; private set; }
    public bool isActive { get; private set; }
    public float sliderValue { get; private set; }
    NumberField leftField;
    NumberField rightField;
    Toggle toggle;
    Slider slider;

    public Action<RoadSettingController> onControllerChangedEvent;
    public RoadSettingController(VisualElement controllerElement, RoadSetting setting)
    {
        name = setting.name;
        leftField = new NumberField(controllerElement.Q<TextField>("nf-left"), false);
        rightField = new NumberField(controllerElement.Q<TextField>("nf-right"), false);
        toggle = controllerElement.Q<Toggle>("toggle");
        slider = controllerElement.Q<Slider>("slider");

        if (!setting.useSlider) slider.style.display = DisplayStyle.None;
        slider.RegisterValueChangedCallback(x => ValueChangedAction());

        toggle.label = setting.name;
        toggle.RegisterValueChangedCallback(x => ValueChangedAction());

        leftField.onValueUpdateEvent += NumberFieldChangedAction;
        rightField.onValueUpdateEvent += NumberFieldChangedAction;
        SetValue(setting);
        ValueChangedWithoutAction();
    }

    void NumberFieldChangedAction(NumberField field)
    {
        ValueChangedAction();
    }

    void ValueChangedAction()
    {
        ValueChangedWithoutAction();
        onControllerChangedEvent.Invoke(this);
    }

    void ValueChangedWithoutAction()
    {
        leftValue = leftField.value;
        rightValue = rightField.value;
        isActive = toggle.value;
        sliderValue = slider.value;
    }

    public void SetValue(RoadSetting setting)
    {
        slider.value = setting.sliderValue;
        leftField.SetValue(setting.leftValue);
        rightField.SetValue(setting.rightValue);
        toggle.value = setting.isActive;
    }
}
