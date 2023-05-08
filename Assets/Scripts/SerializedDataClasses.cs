using System.Collections.Generic;
using UnityEngine;
using System;
using CustomUIClasses;


//
//    Serializable classes
//
/*These are all our serialized classes, these hold differnet informations and values for settings throughout the menu */

//A base class used for all setting that are in list and use names for identification, such as AnomalyOptions or TrafficSettings
public class BaseNamedSetting
{
    public string name;
    public string labelName;
    public string tooltip;
    public ToolTip.Alignment tooltipAlignment = ToolTip.Alignment.Top;
}


[Serializable]
public class AnomalyOption : BaseNamedSetting
{
    public float value;
    public bool active;
}

[Serializable]
public class TrafficSetting : BaseNamedSetting
{
    public float value;
    public bool useOffsets;
    public float offsetRight;
    public float offsetLeft;
    public Color color;
    [HideInInspector]
    public string positionToolTipText;
}


[Serializable]
public class LightingSetting
{
    public float intensity;
    public float ambient;
    public Vector3 direction;
    public Vector4 shadowColor;
}

[Serializable]
public class RoadTransformSetting
{
    public Vector3 position;
    public Vector3 rotation;
    public float distance;
}

[Serializable]
public class RoadSetting : BaseNamedSetting
{
    public float leftValue;
    public float rightValue;
    public bool isActive;
    public bool useSlider;
    public float sliderValue;
    public Color color;
}

[Serializable]
public class ExportSetting
{
    public int videoLength;
    public int videoAmount;
    public bool mixAnomalies;
    public ExportOutputType outputType;
}

public enum ExportOutputType
{
    ImageSequence,
    VideoFile,
    Gif,
    SingleImage
}



/*A converter class that takes in a List of generic BaseNamedSetting.
 * It then lookes for a matching name in the list based on the string given
 * this class is then returned, if one is found */
public class StringToSettingConverter
{
    public T GetNamedSetting<T>(string name, List<T> settings) where T : BaseNamedSetting
    {
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].name == name) 
            {
                return settings[i] as T;
            }
        }
        Debug.LogError("Setting of name '" + name + "' could not be found");
        return null;
    }
}
