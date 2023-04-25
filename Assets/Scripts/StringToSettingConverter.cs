using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return null;
    }
}
