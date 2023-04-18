using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MenuElementCollection
{
    //
    //      Anomaly opstions
    //
    public static class AnomalyOptionElements
    {
        public static List<AnomalyController> anomalyControllers;

        public static void LoadValues(List<AnomalyOption> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                for (int j = 0; j < anomalyControllers.Count; j++)
                {
                    if (options[i].name == anomalyControllers[j].name)
                    {
                        anomalyControllers[j].SetValue(options[i].active, options[i].value);
                        break;
                    }
                }
            }
        }
    }

    //
    //      Traffic settings
    //
    public static class TrafficSettingElements
    {
        public static List<TrafficSettingController> trafficSettingControllers;

        public static void LoadValues(List<TrafficSetting> settings)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                for (int j = 0; j < trafficSettingControllers.Count; j++)
                {
                    if (settings[i].name == trafficSettingControllers[j].name)
                    {
                        trafficSettingControllers[j].SetValue(settings[i]);
                        break;
                    }
                }
            }
        }
    }

    //
    //      Transform settings
    //
    public static class TransformElements
    {
        public static VectorFieldController positionController;
        public static VectorFieldController rotationController;
        public static NumberField scaleField;

        public static void ResetValues()
        {
            positionController.SetValue(new Vector3(0f, 0f, 0f));
            rotationController.SetValue(new Vector3(0f, 0f, 0f));
            scaleField.SetValue(1);
        }
        public static void LoadValues(RoadTransformSetting setting)
        {
            positionController.SetValue(setting.position);
            rotationController.SetValue(setting.rotation);
            scaleField.SetValue(setting.scale);
        }
    }
    

    //
    //      Export Settings
    //
    public static class ExportElements
    {
        public static NumberField videoAmountField;
        public static NumberField videoLengthField;
        public static Toggle mixAnomalyToggle;
        public static RadioButtonGroup outputTypeButtons;

        public static void LoadValues(ExportSetting setting)
        {
            videoAmountField.SetValue(setting.videoAmount);
            videoLengthField.SetValue(setting.videoLength);
            mixAnomalyToggle.value = setting.mixAnomalies;
            outputTypeButtons.value = (int)setting.outputType;
        }
    }

    

    //
    //      Lighting Settings
    //
    public static class LightingElements
    {
        public static Slider intensitySlider;
        public static Slider ambientSlider;
        public static VectorFieldController directionController;
        public static VectorFieldController shadowController;
        public static NumberField alphaField;

        public static void LoadValues(LightingSetting settings)
        {
            intensitySlider.value = settings.intensity;
            ambientSlider.value = settings.ambient;
            directionController.SetValue(settings.direction);
            shadowController.SetValue(new Vector3(settings.shadowColor.x, settings.shadowColor.y, settings.shadowColor.z));
            alphaField.SetValue(settings.shadowColor.w);
        }
    }

    

    //
    //      
    //
}
