using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CustomUIClasses;

/* A massive static class with a collection of all the elements from the menu
 * This was originally put in place if a save/load system was to be implemented, as this did not happen
 * this is mainly unused. The only functionallity currently used is for the transform menu where the reset 
 * functionallity is used. 
*/
public static class MenuElementCollection
{
    //
    //      Anomaly options
    //
    public class AnomalyOptionElements 
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
                        anomalyControllers[j].SetValue(options[i]);
                        break;
                    }
                }
            }
        }
    }

    //
    //      Traffic settings
    //
    public class TrafficSettingElements 
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
    //      Lighting Settings
    //
    public class LightingElements
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
    //      Road Settings
    //
    public class RoadSettingElements
    {
        public static List<RoadSettingController> roadSettingControllers;

        public static void LoadValues(List<RoadSetting> settings)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                for (int j = 0; j < roadSettingControllers.Count; j++)
                {
                    if (settings[i].name == roadSettingControllers[j].name)
                    {
                        roadSettingControllers[j].SetValue(settings[i]);
                        break;
                    }
                }
            }
        }
    }


    //
    //      Transform settings
    //
    public class TransformElements
    {
        public static VectorFieldController positionController;
        public static VectorFieldController rotationController;
        public static NumberField scaleField;
        public static Vector3 fSpyRotation;

        /* Checks if the values are currently any loaded fSpy position, if not, it will first reset to the fSpy location.
         * Pressing again will reset to 0, this is mainly done for usabillity as a user may often only want to go back to the
         * fSpy location.
         */
        public static void ResetValues()
        {
            bool gotReset = false;
            if(fSpyRotation != null)
            {
                if (rotationController.value != fSpyRotation)
                {
                    rotationController.SetValue(fSpyRotation);
                    positionController.SetValue(new Vector3(0f, 0f, 0f));
                    scaleField.SetValue(80f);
                    gotReset = true;
                }
            }
            if (!gotReset)
            {
                positionController.SetValue(new Vector3(0f, 0f, 0f));
                rotationController.SetValue(new Vector3(0f, 0f, 0f));
                scaleField.SetValue(80f);
            }
        }
        public static void LoadValues(RoadTransformSetting setting)
        {
            positionController.SetValue(setting.position);
            rotationController.SetValue(setting.rotation);
            scaleField.SetValue(setting.distance);
        }
    }
    

    //
    //      Export Settings
    //
    public class ExportElements
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
}
