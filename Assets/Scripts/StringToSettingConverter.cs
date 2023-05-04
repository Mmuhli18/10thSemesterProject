using System.Collections.Generic;
using UnityEngine;

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
