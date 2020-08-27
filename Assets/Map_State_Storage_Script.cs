using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_State_Storage_Script : MonoBehaviour
{

    public static Map_State_Storage_Script instance;
    public List<MapNodeSaveState> nodeSaveData;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Map Scene")
        {
            loadMapState();
        }
    }

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

    public void loadMapState()
    {
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
    }

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

    public class MapNodeSaveState
    {
        public string mapNodeID;
        public List<string> linkedNodeIDs;
        public Map_Icon_Script.MapNodeState currentState;
    }
}
