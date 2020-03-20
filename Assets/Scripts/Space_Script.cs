﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_Script : MonoBehaviour
{
    public Vector2 gridPosition = new Vector2(0,0);
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
        
    }

    private void OnMouseDown()
    {
        Debug.Log("Space Clicked");
        if (currentlySelectedMinion != null)
        {
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().setTargetSpace(this.gameObject);
            setCurrentlySelectedMinion(null);
        }
    }

    public static void setCurrentlySelectedMinion(GameObject minionIn)
    {
        currentlySelectedMinion = minionIn;

        if(currentlySelectedMinion != null)
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        //Debug.Log("set selected minion to: " + currentlySelectedMinion);
    }
}
