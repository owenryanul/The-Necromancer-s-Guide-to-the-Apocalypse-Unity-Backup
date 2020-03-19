using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_Script : MonoBehaviour
{
    public Vector2 gridPosition = new Vector2(0,0);
    private GameObject currentlySelectedMinion;

    // Start is called before the first frame update
    void Start()
    {
        
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
        }
    }

    public void setCurrentlySelectedMinion(GameObject minionIn)
    {
        this.currentlySelectedMinion = minionIn;
    }
}
