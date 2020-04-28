using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip_Button_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(2,10)]
    public string tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip_Script.displayTooltip(tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip_Script.hideTooltip();
    }
}
