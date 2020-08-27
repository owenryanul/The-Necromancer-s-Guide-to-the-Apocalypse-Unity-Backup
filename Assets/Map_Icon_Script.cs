using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Icon_Script : MonoBehaviour
{
    [Header("ID")]
    public string nodeID;

    [Header("Scenario")]
    [Tooltip("Prefix a hordeName with BATTLE_ to mark this button as triggering a battle")]
    public string scenarioName;

    [Header("Icon Sprites")]
    public Sprite icon;
    public Sprite highlightedIcon;
    public Sprite yellowHighlightedIcon;

    [Header("State")]
    public MapNodeState currentState;
    public enum MapNodeState
    {
        unvisited,
        visited,
        current,
        overrun
    }

    [Header("Linked Nodes")]
    public GameObject mapLinePrefab;
    public List<string> linkedMapIconIDs;
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
        foreach(GameObject aNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            if(aNode.GetComponent<Map_Icon_Script>() == null)
            {
                Debug.LogError("Map_Icon_Script.linkedMapIcons contains a non-MapIcon game object.");
            }
            else if (!aNode.GetComponent<Map_Icon_Script>().hasDrawnLinks && this.linkedMapIconIDs.Contains(aNode.GetComponent<Map_Icon_Script>().nodeID))
            {
                GameObject line = Instantiate(mapLinePrefab);
                line.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
                line.GetComponent<LineRenderer>().SetPosition(1, aNode.transform.position);
            }
            
        }
        this.hasDrawnLinks = true;
    }

    //Returns true if at least one of the map nodes linked to this map node is the node that the player currently occupies
    private bool isLinkedToPlayersCurrentNode()
    {
        foreach(GameObject aNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            if(aNode.GetComponent<Map_Icon_Script>().currentState == MapNodeState.current)
            {
                return linkedMapIconIDs.Contains(aNode.GetComponent<Map_Icon_Script>().nodeID);
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        if (!inventoryUI.isInventoryVisible() && this.currentState != MapNodeState.current) //stop player clicking map markers through the inventory screen
        {
            if (isLinkedToPlayersCurrentNode())
            {
                GameObject.FindGameObjectWithTag("Player Map Marker").GetComponent<Player_Map_Marker>().moveToNewMapNode(this.gameObject);
            }
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
