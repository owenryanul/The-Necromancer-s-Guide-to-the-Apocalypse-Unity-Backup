using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;

public class User_Input_Script : MonoBehaviour
{
    public static GameObject currentlySelectedMinion;
    private static GameObject selectionCircle;

    // Start is called before the first frame update
    void Start()
    {
        selectionCircle = GameObject.FindGameObjectWithTag("Selection Circle");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            flipCurrentlySelectedMinion();
        }
    }

    public static void setCurrentlySelectedMinion(GameObject minionIn)
    {
        currentlySelectedMinion = minionIn;

        if (currentlySelectedMinion != null)
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        //Debug.Log("set selected minion to: " + currentlySelectedMinion);
    }

    public void flipCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null)
        {
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().flipFacing();
        }
    }

    public void switchWeaponsForCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null)
        {
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().switchWeapons();
        }
    }
}
