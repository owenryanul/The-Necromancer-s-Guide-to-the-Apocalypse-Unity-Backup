using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle_Script : MonoBehaviour
{
    public Sprite selectionCircle_Green;
    public Sprite selectionCircle_Yellow;
    public float snapToSpaceRange = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.SelectOrMove && User_Input_Script.currentlySelectedMinion != null)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = selectionCircle_Green;
            this.transform.position = User_Input_Script.currentlySelectedMinion.transform.position;
        }
        else if(User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.CastAbility)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = selectionCircle_Yellow;
            GameObject nearestGridSpace = Space_Script.findNearestGridSpaceWithinRange(Camera.main.ScreenToWorldPoint(Input.mousePosition), snapToSpaceRange);
            if(nearestGridSpace != null)
            {
                this.transform.position = nearestGridSpace.transform.position;
            }
            else
            {
                Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                v.z = this.transform.position.z;
                this.transform.position = v;
            }
        }
    }

    
}
