using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Map_Marker : MonoBehaviour
{

    public GameObject targetNode;
    private bool isMoving;
    private float speed;
    private Vector3 currentNodeOffset; // the offset of the player marker's position relative to the current map node.

    // Start is called before the first frame update
    void Start()
    {
        speed = 5.0f;
        currentNodeOffset = new Vector3(-0.4f, -0.4f, 0);
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Vector3 move = (targetNode.transform.position - this.transform.position).normalized * speed * Time.deltaTime;
            if ((targetNode.transform.position - this.transform.position).magnitude <= move.magnitude)
            {
                Camera.main.GetComponent<Map_Camera_Script>().lockedToPlayerMarker = false;
                this.transform.position = targetNode.transform.position + currentNodeOffset;
                isMoving = false;
                //On reaching the map node, open the scenario for that node
                GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().setJournalScenario(this.targetNode.GetComponent<Map_Icon_Script>().scenarioName);
                GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().showJournel();
            }
            else
            {
                this.transform.position += move;
            }
        }
    }

    public void moveToNewMapNode(GameObject newNode)
    {
        //Mark the previous current node as visited
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Map Node");
        foreach(GameObject aNode in nodes)
        {
            if(aNode.GetComponent<Map_Icon_Script>().currentState == Map_Icon_Script.MapNodeState.current)
            {
                aNode.GetComponent<Map_Icon_Script>().currentState = Map_Icon_Script.MapNodeState.visited;
                break;
            }
        }

        //Set the new node as the current node occupied by the player
        newNode.GetComponent<Map_Icon_Script>().currentState = Map_Icon_Script.MapNodeState.current;
        //Move the player icon to the centre of the previous node
        this.transform.position = targetNode.transform.position;
        //Set the player marker to move to the new node and lock the camera to the player marker.
        this.targetNode = newNode;
        Camera.main.GetComponent<Map_Camera_Script>().lockedToPlayerMarker = true;
        isMoving = true;
        Stat_Tracking_Script.addEncountersClearedStat();
    }

    public void setCurrentMapNode(GameObject node)
    {
        this.targetNode = node;
        currentNodeOffset = new Vector3(-0.4f, -0.4f, 0);
        this.transform.position = targetNode.transform.position + currentNodeOffset;
    }
}
