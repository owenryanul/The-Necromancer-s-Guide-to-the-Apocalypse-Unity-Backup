using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_State_Storage_Script : MonoBehaviour
{

    public static Map_State_Storage_Script instance;
    public List<MapNodeSaveState> nodeSaveData;
    private bool resetSaveState;

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

    void Update()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;  
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
        nodeSaveData = new List<MapNodeSaveState>();
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Map Node");
        foreach(GameObject aNode in nodes)
        {
            MapNodeSaveState save = new MapNodeSaveState();
            save.mapNodeID = aNode.GetComponent<Map_Icon_Script>().nodeID;
            save.currentState = aNode.GetComponent<Map_Icon_Script>().currentState;
            save.linkedNodeIDs = aNode.GetComponent<Map_Icon_Script>().linkedMapIconIDs;
            nodeSaveData.Add(save);
        }
    }

    //Load the previously saved state of the map and apply it to the map
    public void loadMapState()
    {
        Debug.Log("Loading Map State");
        foreach(MapNodeSaveState aSave in nodeSaveData)
        {
            GameObject aNode = findNodeByID(aSave.mapNodeID);
            aNode.GetComponent<Map_Icon_Script>().currentState = aSave.currentState;
            aNode.GetComponent<Map_Icon_Script>().linkedMapIconIDs = aSave.linkedNodeIDs;
            if(aSave.currentState == Map_Icon_Script.MapNodeState.current)
            {
                GameObject.FindGameObjectWithTag("Player Map Marker").GetComponent<Player_Map_Marker>().setCurrentMapNode(aNode);
            }
        }

        Journal_Text_Script journalScript = GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>();
        journalScript.setUIReferencesOnStart(); //make sure the ui variables are instaniated as start() seems to be running too late to do it
        journalScript.loadPostBattleScreen();
        journalScript.showJournel();
    }

    //Called by setting resetMapState to true when starting a new game.
    public void generateMap()
    {
        Debug.Log("Generating Map");
        //Add map generation code here.
    }

    //Called by the new game button onClick listener to tell this script to generate a new map state rather than load the previously saved map state
    public static void flagMapForRegeneration()
    {
        instance.resetSaveState = true;
    }

    //Returns a Map marker Node with the matching NodeID assigned to it
    public GameObject findNodeByID(string aID)
    {
        foreach(GameObject aNode in GameObject.FindGameObjectsWithTag("Map Node"))
        {
            if(aNode.GetComponent<Map_Icon_Script>().nodeID == aID)
            {
                return aNode;
            }
        }
        return null;
    }

    //Save data for a Map Marker. This class is used for storing the data on a map node when switching to and from the map scene.
    public class MapNodeSaveState
    {
        public string mapNodeID; //the id string of the map node
        public List<string> linkedNodeIDs; //the ID of map nodes connected to this node
        public Map_Icon_Script.MapNodeState currentState; //the current state of the map node (current/unvisited/visited)
    }
}
