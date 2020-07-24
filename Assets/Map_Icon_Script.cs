using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Icon_Script : MonoBehaviour
{
    [Header("Scenario")]
    [Tooltip("Prefix a hordeName with BATTLE_ to mark this button as triggering a battle")]
    public string scenarioName;

    [Header("Icon Sprites")]
    public Sprite icon;
    public Sprite highlightedIcon;
    public Sprite yellowHighlightedIcon;

    private Inventory_UI_Script inventoryUI;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory UI").GetComponent<Inventory_UI_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!inventoryUI.isInventoryVisible()) //stop player clicking map markers through the inventory screen
        {
            GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().setJournalScenario(this.scenarioName);
            GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().showJournel();
        }
    }

    private void OnMouseEnter()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = highlightedIcon;
    }

    private void OnMouseExit()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = icon;
    }
}
