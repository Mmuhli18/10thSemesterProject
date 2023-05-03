using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace CustomUIClasses{
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
        ToolTip toolTip;

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

        public void AddTooltip(string tip)
        {
            toolTip = new ToolTip(textField, tip);
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
        ToolTip toolTip;
        public float value { get; private set; }
        public bool isActive { get; private set; }
        public Action<AnomalyController> onControllerChangedEvent;
        public AnomalyController(VisualElement controllerElement, AnomalyOption option)
        {
            name = option.name;
            var nameLabel = controllerElement.Q<Label>("l-anomaly-name");
            nameLabel.text = name;
            if (option.labelName != "") nameLabel.text = option.labelName;
            if (option.tooltip != "") toolTip = new ToolTip(nameLabel, option.tooltip);
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
            catch (Exception e)
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
        Label nameLabel;
        NumberField leftField;
        NumberField rightField;
        public float value { get; private set; }
        public float offsetLeft { get; private set; }
        public float offsetRight { get; private set; }
        public Action<TrafficSettingController> onControllerChangedEvent;
        ToolTip toolTip;
        public TrafficSettingController(VisualElement controllerElement, TrafficSetting setting)
        {
            name = setting.name;
            //slider
            slider = controllerElement.Q<Slider>("traffic-slider");
            label = controllerElement.Q<Label>("l-value");
            nameLabel = controllerElement.Q<Label>("label");
            nameLabel.text = setting.labelName;
            if(setting.toolTip != "") toolTip = new ToolTip(nameLabel, setting.toolTip);
            if (setting.labelName == "") nameLabel.text = setting.name;
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

                new ToolTip(controllerElement.Q("l-offset-position"), setting.positionToolTipText);
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

        protected NumberField leftField;
        NumberField rightField;
        Toggle toggle;

        public Action<RoadSettingController> onControllerChangedEvent;
        public RoadSettingController(VisualElement controllerElement, RoadSetting setting)
        {
            name = setting.name;
            
            leftField = new NumberField(controllerElement.Q<TextField>("nf-left"), false);
            rightField = new NumberField(controllerElement.Q<TextField>("nf-right"), false);
            toggle = controllerElement.Q<Toggle>("toggle");

            controllerElement.Q<Slider>("slider").style.display = DisplayStyle.None;

            controllerElement.Q("color-element").style.backgroundColor = new StyleColor(setting.color);

            toggle.label = setting.name;
            if (setting.labelName != "") toggle.label = setting.labelName; 
            toggle.RegisterValueChangedCallback(x => ValueChangedAction());

            leftField.onValueUpdateEvent += NumberFieldChangedAction;
            rightField.onValueUpdateEvent += NumberFieldChangedAction;
            SetValue(setting);
            ValueChangedWithoutAction();
        }

        protected void NumberFieldChangedAction(NumberField field)
        {
            ValueChangedAction();
        }

        protected void ValueChangedAction()
        {
            ValueChangedWithoutAction();
            onControllerChangedEvent.Invoke(this);
        }

        protected virtual void ValueChangedWithoutAction()
        {
            leftValue = leftField.value;
            rightValue = rightField.value;
            isActive = toggle.value;

        }

        public virtual void SetValue(RoadSetting setting)
        {
            leftField.SetValue(setting.leftValue * 10f);
            rightField.SetValue(setting.rightValue * 10f);
            toggle.value = setting.isActive;
        }
    }

    public class RoadSettingSliderController : RoadSettingController
    {
        public float value { get; private set; }
        Slider slider;
        public float sliderValue { get; private set; }

        public RoadSettingSliderController(VisualElement controllerElement, RoadSetting setting) : base(controllerElement, setting)
        {
            slider = controllerElement.Q<Slider>("slider");
            //slider.style.display = DisplayStyle.Flex;
            controllerElement.Q<TextField>("nf-right").style.display = DisplayStyle.None;
            controllerElement.Q<TextField>("nf-left").label = "Width";
            controllerElement.Q<Label>("label").style.display = DisplayStyle.None;
            slider.RegisterValueChangedCallback(x => ValueChangedAction());
            SetValue(setting);
            ValueChangedWithoutAction();
        }

        override protected void ValueChangedWithoutAction()
        {
            base.ValueChangedWithoutAction();
            value = leftField.value;
            if (slider != null) sliderValue = slider.value;
        }

        override public void SetValue(RoadSetting setting)
        {
            base.SetValue(setting);
            if (slider != null) slider.value = setting.sliderValue;
            ValueChangedWithoutAction();
        }
    }

    public class ToolTip
    {
        VisualElement parent;
        public string text;
        VisualElement body;
        Label label;
        Alignment alignment;

        public ToolTip(VisualElement p, string t, Alignment algn = Alignment.Top)
        {
            parent = p;
            text = t;

            parent.RegisterCallback<MouseEnterEvent>(x => Display());
            parent.RegisterCallback<MouseLeaveEvent>(x => UnDisplay());

            body = new VisualElement();
            
            body.style.position = Position.Absolute;
            body.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
            body.style.visibility = Visibility.Hidden;

            alignment = algn;

            MenuController.UIDoc.rootVisualElement.Add(body);
        }

        void UnDisplay()
        {
            if(label != null)
            {
                body.style.visibility = Visibility.Hidden;
            }
        }

        void Display()
        {
            if(label == null)
            {
                body.style.left = parent.worldBound.xMin;
                body.style.top = parent.worldBound.yMin;
                if (alignment == Alignment.Top) body.style.top = parent.worldBound.yMin - parent.worldBound.height;
                else if (alignment == Alignment.Bottom) body.style.top = parent.worldBound.yMin + parent.worldBound.height;
                else if (alignment == Alignment.Left) body.style.left = parent.worldBound.xMin - body.worldBound.width;
                else if (alignment == Alignment.Right) body.style.left = parent.worldBound.xMin + body.worldBound.width;
                label = new Label(text);
                label.style.color = Color.black;
                label.style.fontSize = new StyleLength(20);
                body.Add(label);
            }
            body.style.visibility = Visibility.Visible;
        }

        public enum Alignment
        {
            Top,
            Bottom,
            Left,
            Right
        }
    }
}
