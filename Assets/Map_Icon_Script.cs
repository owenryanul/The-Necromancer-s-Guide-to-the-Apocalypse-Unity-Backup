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

    [Header("Linked Nodes")]
    public GameObject mapLinePrefab;
    public List<GameObject> LinkedMapIcons;
    private bool hasDrawnLinks;

    private Inventory_UI_Script inventoryUI;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory UI").GetComponent<Inventory_UI_Script>();
        hasDrawnLinks = false;
        drawLineToLinkedNodes();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void drawLineToLinkedNodes()
    {
        foreach(GameObject aNode in LinkedMapIcons)
        {
            if(aNode.GetComponent<Map_Icon_Script>() == null)
            {
                Debug.LogError("Map_Icon_Script.linkedMapIcons contains a non-MapIcon game object.");
            }
            else if (!aNode.GetComponent<Map_Icon_Script>().hasDrawnLinks)
            {
                GameObject line = Instantiate(mapLinePrefab);
                line.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
                line.GetComponent<LineRenderer>().SetPosition(1, aNode.transform.position);
            }
            
        }
        this.hasDrawnLinks = true;
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

    public bool getHasDrawnLinks()
    {
        return this.hasDrawnLinks;
    }
}
