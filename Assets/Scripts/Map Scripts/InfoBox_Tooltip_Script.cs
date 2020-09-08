using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoBox_Tooltip_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(2, 10)]
    public string titleText;
    public string descriptionText;

    public void Start()
    {
        Text title = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Title").GetComponent<Text>();
        Text body = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Body").GetComponent<Text>();
        title.text = "";
        body.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text title = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Title").GetComponent<Text>();
        Text body = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Body").GetComponent<Text>();
        title.text = titleText;
        body.text = descriptionText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text title = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Title").GetComponent<Text>();
        Text body = GameObject.FindGameObjectWithTag("Inventory Infobox 2 Body").GetComponent<Text>();
        title.text = "";
        body.text = "";
    }
}
