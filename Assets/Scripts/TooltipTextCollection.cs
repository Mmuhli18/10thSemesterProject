using System.Collections.Generic;
using UnityEngine;
using System;
using CustomUIClasses;

/* Object for holding a collection of tooltip displayed in the application. 
 * Tooltips are diffined using the tooltipHolder class where they are given a name of the element they should
 * display at and then the tooltip text they should display
 */
[CreateAssetMenu]
public class TooltipTextCollection : ScriptableObject
{
    [SerializeField]
    List<TooltipHolder> tooltips = new List<TooltipHolder>();

    //Incert a field type and get the assosiated tip for this field
    public string GetTipFromField(TooltipField field)
    {
        for (int i = 0; i < tooltips.Count; i++) if (tooltips[i].field == field) return tooltips[i].tip;
        Debug.LogError("No tip for field '" + field.ToString() + "'");
        return null;
    }

    //Returns all element tied tips
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

    /* Our class for defining tooltips, has a type, this is mainly elementied tips
     * the name of the element it should be tied to, if element tied, the tip text itself, and what 
     * alignment the tip should have, default is top.
     */
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