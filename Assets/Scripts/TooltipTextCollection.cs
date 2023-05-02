using System.Collections.Generic;
using UnityEngine;
using System;
using CustomUIClasses;

[CreateAssetMenu]
public class TooltipTextCollection : ScriptableObject
{
    [SerializeField]
    List<TooltipHolder> tooltips = new List<TooltipHolder>();

    public string GetTipFromField(TooltipField field)
    {
        for (int i = 0; i < tooltips.Count; i++) if (tooltips[i].field == field) return tooltips[i].tip;
        Debug.LogError("No tip for field '" + field.ToString() + "'");
        return null;
    }

    public List<TooltipHolder> GetElementTiedTips()
    {
        List<TooltipHolder> returnTips = new List<TooltipHolder>();
        for(int i = 0; i < tooltips.Count; i++)
        {
            if (tooltips[i].field == TooltipField.ElementTied) returnTips.Add(tooltips[i]);
        }
        if (returnTips.Count == 0) Debug.LogWarning("No element tied fields found");
        return returnTips;
    }

    [Serializable]
    public class TooltipHolder
    {
        public TooltipField field;
        public string element;
        public string tip;
        public ToolTip.Alignment alignment = ToolTip.Alignment.Top;
    }
}

public enum TooltipField
{
    ElementTied,
    TrafficOffsetPosition
}