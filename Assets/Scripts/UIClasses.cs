using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

// Collection of classes used for differnet UIElements throughout the application
namespace CustomUIClasses{
    /* Controller class for the buttons used to switch between the tabs, has an onClickedEvent and with darken/display 
     * if given the correct tab type
     */
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
            if(onPressEvent.Method != null) onPressEvent.Invoke(buttonTab);
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

    // Controller class for the visual elements that contain our differnt tabs, will display if given the correct type
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

    /* A more generic class used for making a textfield into a numberfield, this includes limiting text to only be numbers,
     * allowing or disallowing negative numbers, changing number based mouse dragging, and an event action for when the number is changed.
     */
    public class NumberField
    {
        public TextField textField { get; private set; }
        VisualElement label;
        float lastXPos;
        float sensitivity = 0.5f;
        public bool allowNegativeNumbers = true;
        public float value { get; private set; }
        public Action<NumberField> onValueUpdateEvent;
        public static MonoBehaviour instance; //instance needed for doing dragging

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

        //Runs MouseSpyware until the user lets go of mouse button
        IEnumerator DoMouseDrag()
        {
            lastXPos = Input.mousePosition.x;
            yield return new WaitUntil(() => MouseSpyware());
        }

        //Changes value of numberfield based on the mouse position, returns true when the user lets go of mouse button
        bool MouseSpyware()
        {
            SetValue((int)(value + (Input.mousePosition.x - lastXPos) * sensitivity));
            lastXPos = Input.mousePosition.x;
            return Input.GetMouseButtonUp(0);
        }

        public void AddTooltip(string tip)
        {
            new ToolTip(textField, tip);
        }
    }

    /* A more generic class used for making three textfields into three numberfields, or taking three numberfield and then making those into 
     * a 3D vector controller. Each numberfield then controlling a field in the vector. Extends the ValueUpdateEvent from the numberfield 
     * to then be a VectorUpdateEvent.
     */
    public class VectorFieldController
    {
        NumberField xField;
        NumberField yField;
        NumberField zField;
        public Action<Vector3, string> onVectorUpdateEvent;
        public Vector3 value { get; private set; }
        public string name { get; private set; }

        //Constructor that takes 3 predefined numberfields
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

        //Constructor that finds three textfields in a visual element and makes them numberfields
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

    /* A base class for our differnt controller elements that are generated from serialzed information on startup.
     * Sets up basic functionallity such as applying the controller name and adding tooltips to the controller label.
     */
    public class NamedClassController
    {
        public string name { get; private set; }
        public Action<NamedClassController> onControllerChangedEvent;


        public NamedClassController(BaseNamedSetting setting, Label nameLabel)
        {
            name = setting.name;
            nameLabel.text = name;
            if (setting.labelName != "") nameLabel.text = setting.labelName;
            if (setting.tooltip != "") new ToolTip(nameLabel, setting.tooltip, setting.tooltipAlignment);
        }

        protected virtual void ValueChangedWithoutAction() { }

        protected virtual void ValueChangedAction()
        {
            ValueChangedWithoutAction();
            if (onControllerChangedEvent.Method != null)
                onControllerChangedEvent.Invoke(this);
            else
                Debug.LogWarning("Value changed event not assigned a method");
        }

        public virtual void SetValue(BaseNamedSetting setting) { }

    }

    /* Controller for our anomaly options, subclass of the NamedClassController since anomaly options are serialzed information.
     * Runs functionallity for the frequency slider of anomalies and the toggle to enable and disable anomalies in the controller.
     */
    public class AnomalyController : NamedClassController
    {
        Slider slider;
        Toggle toggle;
        Label label;
        public float value { get; private set; }
        public bool isActive { get; private set; }
        public AnomalyController(VisualElement controllerElement, AnomalyOption option) : base(option as BaseNamedSetting, controllerElement.Q<Label>("l-anomaly-name"))
        {
            slider = controllerElement.Q<Slider>("anomaly-slider");
            toggle = controllerElement.Q<Toggle>("anomaly-toggle");
            label = controllerElement.Q<Label>("l-value");

            slider.RegisterValueChangedCallback(x => ValueChangedAction());
            toggle.RegisterValueChangedCallback(x => ValueChangedAction());

            SetValue(option);
            ValueChangedWithoutAction();
        }

        override protected void ValueChangedWithoutAction()
        {
            value = slider.value;
            isActive = toggle.value;
            label.text = value.ToString();
        }

        public override void SetValue(BaseNamedSetting setting)
        {
            slider.value = (setting as AnomalyOption).value;
            toggle.value = (setting as AnomalyOption).active;
        }
    }

    /* Controller for our traffic settings, subclass of the NamedClassController since traffic settings are serialzed information.
     * Runs functionallity for the amount slider and numberfields for offsets in the controller.
     */
    public class TrafficSettingController : NamedClassController
    {
        Slider slider;
        Label label;
        NumberField leftField;
        NumberField rightField;
        public float value { get; private set; }
        public float offsetLeft { get; private set; }
        public float offsetRight { get; private set; }
        public TrafficSettingController(VisualElement controllerElement, TrafficSetting setting) : base(setting as BaseNamedSetting, controllerElement.Q<Label>("label"))
        {
            //slider
            slider = controllerElement.Q<Slider>("traffic-slider");
            label = controllerElement.Q<Label>("l-value");
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

        override protected void ValueChangedWithoutAction()
        {
            value = slider.value;
            label.text = value.ToString();
            if (leftField != null) offsetLeft = leftField.value;
            if (rightField != null) offsetRight = rightField.value;
        }

        override public void SetValue(BaseNamedSetting setting)
        {
            slider.value = (setting as TrafficSetting).value;
            if (leftField != null) leftField.SetValue((setting as TrafficSetting).offsetLeft);
            if (rightField != null) rightField.SetValue((setting as TrafficSetting).offsetRight);
        }
    }

    /* Controller for our road settings, subclass of the NamedClassController since road settings are serialzed information.
     * Runs functionallity for enable and disable toggle, color code, and size numberfields in the controller.
     */
    public class RoadSettingController : NamedClassController
    {
        public float leftValue { get; private set; }
        public float rightValue { get; private set; }
        public bool isActive { get; private set; }

        protected NumberField leftField;
        NumberField rightField;
        Toggle toggle;
        public RoadSettingController(VisualElement controllerElement, RoadSetting setting) : base(setting as BaseNamedSetting, controllerElement.Q<Toggle>("toggle").Q<Label>())
        {
            
            leftField = new NumberField(controllerElement.Q<TextField>("nf-left"), false);
            rightField = new NumberField(controllerElement.Q<TextField>("nf-right"), false);
            toggle = controllerElement.Q<Toggle>("toggle");

            controllerElement.Q<Slider>("slider").style.display = DisplayStyle.None;

            controllerElement.Q("color-element").style.backgroundColor = new StyleColor(setting.color);

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

        override protected void ValueChangedWithoutAction()
        {
            leftValue = leftField.value;
            rightValue = rightField.value;
            isActive = toggle.value;

        }

        override public void SetValue(BaseNamedSetting setting)
        {
            leftField.SetValue((setting as RoadSetting).leftValue * 10f);
            rightField.SetValue((setting as RoadSetting).rightValue * 10f);
            toggle.value = (setting as RoadSetting).isActive;
        }
    }

    /* A variation of the RoadSettingController, this one is used when the slider is enabled, for the final iteration there
     * is however no slider, instead this is actually used to only display one width field size the car part of the road
     * only uses one field. 
     */
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

        override public void SetValue(BaseNamedSetting setting)
        {
            base.SetValue(setting);
            if (slider != null) slider.value = (setting as RoadSetting).sliderValue;
            ValueChangedWithoutAction();
        }
    }

    /* Generic class used for creating a tooltip that is attached to a visual element. The tip will display
     * upon a MouseEnterEvent on the visual element, and hide again on a MouseLeaveEvent.
     */
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
            /* Upon being displayed the first time, the tooltip will create its body. We do this here to
             * make sure the parent visual element has been instanciated and placed in its layout
             */
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
            //later display call will simply show the tooltip
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
