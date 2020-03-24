using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Movement_Script : MonoBehaviour
{
    public GameObject mySpace;
    public GameObject targetSpace;
    public float speed = 1.0f;

    private Animator rigAnimator;

    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 myPos = this.transform.position;
        Vector3 mySpacePos = mySpace.transform.position;
        mySpacePos.z = this.transform.position.z; //ignore the z dimension

        if(myPos != mySpacePos) //if not at mySpace, move to it
        {
            isMoving = true;
            Vector3 directionVector = (mySpacePos - this.transform.position);
            directionVector.z = 0;
            Vector3 moveVector = directionVector.normalized * (speed * Time.deltaTime);
            if (moveVector.magnitude > directionVector.magnitude)
            {
                this.transform.position = mySpacePos;
            }
            else
            {
                this.transform.position += moveVector;
            }
        }
        else if (mySpace != targetSpace) //if at mySpace check if its your target space, if not change mySpace
        {
            mySpace = findNextSpace();
        }
        else //if at mySpace and it is your target space, stop moving
        {
            isMoving = false;
        }

        rigAnimator.SetBool("IsWalking", isMoving);

        if(isMoving)
        {
            //TODO: Set sorting layer order to render based on y position
        }
    }

    //If minion clicked, set them as the currently selected minion
    private void OnMouseDown()
    {
        Debug.Log("Minion Clicked");
        Space_Script.setCurrentlySelectedMinion(this.gameObject);      
    }


    //Find the next space on the path between the currently occupied space and the target space.
    //Note: Will attempt to match target spaces Y before matching X
    private GameObject findNextSpace()
    {
        Vector2 myGridPos = mySpace.GetComponent<Space_Script>().gridPosition;
        Vector2 targetGridPos = targetSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 nextGridPos = myGridPos;

        if (myGridPos.y < targetGridPos.y)
        {
            nextGridPos.y++;
        }
        else if (myGridPos.y > targetGridPos.y)
        {
            nextGridPos.y--;
        }
        else if (myGridPos.x < targetGridPos.x)
        {
            nextGridPos.x++;
        }
        else if (myGridPos.x > targetGridPos.x)
        {
            nextGridPos.x--;
        }

        foreach(GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if(aSpace.GetComponent<Space_Script>().gridPosition == nextGridPos)
            {
                return aSpace;
            }
        }

        Debug.LogError("WARNING: Space could not be found, check your space's gridPositions as no space could be found with co-ordinates: " + nextGridPos + " on the path to " + targetGridPos);
        return null;
    }

    public bool setTargetSpace(GameObject spaceIn)
    {
        //Check if spaceIn is already the target space of another minion
        foreach(GameObject aminion in GameObject.FindGameObjectsWithTag("Minion"))
        {
            if((aminion != this.gameObject) && aminion.GetComponent<Minion_Movement_Script>().targetSpace == spaceIn)
            {
                return false;
            }
        }

        //Check if spaceIn is already the target space of the necromancer
        GameObject aNecromancer = GameObject.FindGameObjectWithTag("Necromancer");
        if ((aNecromancer != this.gameObject) && aNecromancer.GetComponent<Minion_Movement_Script>().targetSpace == spaceIn)
        {
            return false;
        }


        targetSpace = spaceIn;
        return true;
    }
}
