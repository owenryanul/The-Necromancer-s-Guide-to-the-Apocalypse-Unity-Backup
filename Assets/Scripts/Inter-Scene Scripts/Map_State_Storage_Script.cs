using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_State_Storage_Script : MonoBehaviour
{

    public static Map_State_Storage_Script instance;
    public List<MapNodeSaveState> nodeSaveData;
    private bool resetSaveState;

    public GameObject mapNode_Building;
    public GameObject mapNode_Crypt;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            instance.resetSaveState = false;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    //OnSceneLoaded Listener. When the Map Scene is loaded it will try will trigger either map state loading or map generation depending ont eh state of the resetSaveState flag.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if(scene.name == "Map Scene")
        {
            if (resetSaveState)
            {
                generateMap();
            }
            else
            {
                loadMapState();
            }
            instance.resetSaveState = false;
        }
    }

    //Save the current state of the map
    public void saveMapState()
    {
        Debug.Log("Saving Map State");
        nodeSaveData = new List<MapNodeSaveState>();
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Map Node");
        foreach(GameObject aNode in nodes)
        {
            MapNodeSaveState save = new MapNodeSaveState();
            save.nodePosition = aNode.transform.position;
            save.nodeIcon = aNode.GetComponent<Map_Icon_Script>().icon;
            save.mapNodeID = aNode.GetComponent<Map_Icon_Script>().nodeID;
            save.scenarioName = aNode.GetComponent<Map_Icon_Script>().scenarioName;
            save.currentState = aNode.GetComponent<Map_Icon_Script>().currentState;
            save.linkedNodeIDs = aNode.GetComponent<Map_Icon_Script>().linkedMapIconIDs;
            nodeSaveData.Add(save);
        }
    }

    //Load the previously saved state of the map and apply it to the map
    public void loadMapState()
    {
        foreach (GameObject anOldNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            Destroy(anOldNode);
        }

        Debug.Log("Loading Map State ");
        foreach(MapNodeSaveState aSave in nodeSaveData)
        {
            //GameObject aNode = findNodeByID(aSave.mapNodeID);
            GameObject aNode = instaniateNodeFromSaveState(aSave);
            aNode.GetComponent<Map_Icon_Script>().nodeID = aSave.mapNodeID;
            aNode.GetComponent<Map_Icon_Script>().scenarioName = aSave.scenarioName;
            aNode.GetComponent<Map_Icon_Script>().currentState = aSave.currentState;
            aNode.GetComponent<Map_Icon_Script>().linkedMapIconIDs = aSave.linkedNodeIDs;

            if (aSave.currentState == Map_Icon_Script.MapNodeState.current)
            {
                GameObject.FindGameObjectWithTag("Player Map Marker").GetComponent<Player_Map_Marker>().setCurrentMapNode(aNode);
            }
        }

        drawAllNodeLinkLines();

        //Display the post battle screen
        Journal_Text_Script journalScript = GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>();
        journalScript.setUIReferencesOnStart(); //make sure the ui variables are instaniated as start() seems to be running too late to do it
        journalScript.loadPostBattleScreen();
        journalScript.showJournel();
    }

    //Called by setting resetMapState to true when starting a new game.
    public void generateMap()
    {
        Debug.Log("Generating Map");
        generatePreAlphaMap();
        //Add map generation code here.
    }

    //Called by the new game button onClick listener to tell this script to generate a new map state rather than load the previously saved map state
    public static void flagMapForRegeneration()
    {
        instance.resetSaveState = true;
    }

    private void generatePreAlphaMap()
    {
        foreach(GameObject anOldNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            Destroy(anOldNode);
        }

        int IDindex = 0;
        GameObject node1 = createMapNode(mapNode_Crypt, new Vector3(0, 0, 0), "Scenario_Test_Crypt", ref IDindex, Map_Icon_Script.MapNodeState.current);
        GameObject node2 = createMapNode(mapNode_Building, new Vector3(3, 0, 0), "Scenario_Test_3", ref IDindex);
        GameObject node3 = createMapNode(mapNode_Building, new Vector3(6, 3, 0), "BATTLE_No Battle[_]Scenario_PostBattle_Test", ref IDindex);
        GameObject node4 = createMapNode(mapNode_Building, new Vector3(6, -3, 0), "Scenario_Test_Exit", ref IDindex);
        linkNodes(node1, node2);
        linkNodes(node2, node3);
        linkNodes(node2, node4);
        linkNodes(node3, node4);
        drawAllNodeLinkLines();

        GameObject.FindGameObjectWithTag("Player Map Marker").GetComponent<Player_Map_Marker>().setCurrentMapNode(node1);
    }

    //Marks 2 nodes as being linked to each other on the map. Nodes that are linked will have a line drawn between them and can be moved between.
    private void linkNodes(GameObject node1, GameObject node2)
    {
        node1.GetComponent<Map_Icon_Script>().linkedMapIconIDs.Add(node2.GetComponent<Map_Icon_Script>().nodeID);
        node2.GetComponent<Map_Icon_Script>().linkedMapIconIDs.Add(node1.GetComponent<Map_Icon_Script>().nodeID);
    }

    //Create a map node and assign all the necessary variables to it's Map_Icon_Script component.
    private GameObject createMapNode(GameObject prefab, Vector3 position, string scenarioName, ref int IDindex, Map_Icon_Script.MapNodeState state = Map_Icon_Script.MapNodeState.unvisited)
    {
        GameObject newNode = Instantiate(prefab, position, prefab.transform.rotation);
        newNode.GetComponent<Map_Icon_Script>().nodeID = "MapNode_" + IDindex;
        newNode.GetComponent<Map_Icon_Script>().scenarioName = scenarioName;
        newNode.GetComponent<Map_Icon_Script>().currentState = state;
        IDindex++;
        return newNode;
    }

    //Returns a Map marker Node with the matching NodeID assigned to it
    public GameObject findNodeByID(string aID)
    {
        foreach(GameObject aNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            Debug.Log("Finding Node by id:" + aID + " a nodeID: " + aNode.GetComponent<Map_Icon_Script>().nodeID);
            if(aNode.GetComponent<Map_Icon_Script>().nodeID == aID)
            {
                return aNode;
            }
        }
        return null;
    }

    //Takes a Node save data and instaniates it as an object by using the prefab that matches the icon stored in the save data.
    private GameObject instaniateNodeFromSaveState(MapNodeSaveState aSave)
    {
        if (aSave.nodeIcon == mapNode_Building.GetComponent<Map_Icon_Script>().icon)
        {
            return Instantiate(mapNode_Building, aSave.nodePosition, mapNode_Building.transform.rotation);
        }
        else if (aSave.nodeIcon == mapNode_Crypt.GetComponent<Map_Icon_Script>().icon)
        {
            return Instantiate(mapNode_Crypt, aSave.nodePosition, mapNode_Crypt.transform.rotation);
        }
        else
        {
            //Default instanciate as Crypt Node(indicating that something is wrong as there should really only be one crypt node)
            return Instantiate(mapNode_Crypt, aSave.nodePosition, mapNode_Crypt.transform.rotation);
        }
    }

    //Calls drawLineToLinkedNodes on all Map Nodes
    private void drawAllNodeLinkLines()
    {
        foreach(GameObject aNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            aNode.GetComponent<Map_Icon_Script>().drawLineToLinkedNodes();
        }
    }

    //Save data for a Map Marker. This class is used for storing the data on a map node when switching to and from the map scene.
    //Note: InstatiateNodeFromSave() is used to create Node Objects from the save instead of storing a copy of the original gameObject as storing a
    //  game object as a variable in MapNodeSaveState would only store a reference to the original rather than a copy.
    public class MapNodeSaveState
    {
        public Vector3 nodePosition;
        public Sprite nodeIcon;
        public string mapNodeID; //the id string of the map node
        public string scenarioName;
        public List<string> linkedNodeIDs; //the ID of map nodes connected to this node
        public Map_Icon_Script.MapNodeState currentState; //the current state of the map node (current/unvisited/visited)
    }
}
