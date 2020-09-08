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
        //drawLineToLinkedNodes();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Draw a line on the map that connects nodes that can be moved to from this node
    public void drawLineToLinkedNodes()
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

    //On clicking on this node.
    private void OnMouseDown()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = icon; //UnHighlights the map icon when the node is clicked on.
        if (!inventoryUI.isInventoryVisible() && this.currentState != MapNodeState.current) //stop player clicking map markers through the inventory screen
        {
            if (isLinkedToPlayersCurrentNode())
            {
                GameObject.FindGameObjectWithTag("Player Map Marker").GetComponent<Player_Map_Marker>().moveToNewMapNode(this.gameObject);
            }
        }
    }

    //Highlight the map icon when hovered over, if it's a node that can be moved to.
    private void OnMouseEnter()
    {
        if (isLinkedToPlayersCurrentNode())
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = highlightedIcon;
        }
    }

    //UnHighlights the map icon when the mouse is moved away, if it's a node that can be moved to.
    private void OnMouseExit()
    {
        if (isLinkedToPlayersCurrentNode())
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = icon;
        }
    }

    //returns true if this node has already drawn lines on the map to it's connected nodes. Used by other node's drawLineToLinkedNodes() to avoid drawing redundent lines
    public bool getHasDrawnLinks()
    {
        return this.hasDrawnLinks;
    }
}
