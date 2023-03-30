using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MenuElementCollection
{
    //
    //      Transform settings
    //
    public static class TransformElements
    {
        public static VectorFieldController positionController;
        public static VectorFieldController rotationController;
        public static NumberField scaleField;
    }
    public static void ResetTransform()
    {
        TransformElements.positionController.SetValue(new Vector3(0f, 0f, 0f));
        TransformElements.rotationController.SetValue(new Vector3(0f, 0f, 0f));
        TransformElements.scaleField.SetValue(1);
    }
    public static void LoadTransformValues(RoadTransformSetting setting)
    {
        TransformElements.positionController.SetValue(setting.position);
        TransformElements.rotationController.SetValue(setting.rotation);
        TransformElements.scaleField.SetValue(setting.scale);
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
    }

    public static void LoadExportOptionValues(ExportSetting setting)
    {
        ExportElements.videoAmountField.SetValue(setting.videoAmount);
        ExportElements.videoLengthField.SetValue(setting.videoLength);
        ExportElements.mixAnomalyToggle.value = setting.mixAnomalies;
        ExportElements.outputTypeButtons.value = (int)setting.outputType;
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
    }

    public static void LoadLightingValues(LightingSetting settings)
    {
        LightingElements.intensitySlider.value = settings.intensity;
        LightingElements.ambientSlider.value = settings.ambient;
        LightingElements.directionController.SetValue(settings.direction);
        LightingElements.shadowController.SetValue(new Vector3(settings.shadowColor.x, settings.shadowColor.y, settings.shadowColor.z));
        LightingElements.alphaField.SetValue(settings.shadowColor.w);
    }

    //
    //      
    //
}
