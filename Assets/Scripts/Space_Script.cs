using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_Script : MonoBehaviour, MouseDownOverrider
{
    public Vector2 gridPosition = new Vector2(0,0);
    private Ability_Database_Script abilityDatabase;
    

    // Start is called before the first frame update
    void Start()
    {
        abilityDatabase = GameObject.FindGameObjectWithTag("Level Script Container").GetComponent<Ability_Database_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDownOverride()
    {
        Debug.Log("Space Clicked");
        if (User_Input_Script.currentlySelectedMinion != null)
        {
            //If issuing a move order to a minion, change the minion's targetSpace to this space and deselect them
            if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.SelectOrMove)
            {
                User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>().setTargetSpace(this.gameObject);
                User_Input_Script.setCurrentlySelectedMinion(null);
            }
            //If aiming an ability, cast the ability targeting this space
            else if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.CastAbility)
            {
                abilityDatabase.cast(User_Input_Script.currentAbilityToCast, User_Input_Script.currentlySelectedMinion, this.gameObject);
                User_Input_Script.currentMouseCommand = User_Input_Script.MouseCommand.SelectOrMove;
            }
        }
    }

    public static GameObject findGridSpace(int x, int y)
    {
        return findGridSpace(new Vector2(x, y));
    }

    public static GameObject findGridSpace(Vector2 position)
    {    
        foreach(GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if(aSpace.GetComponent<Space_Script>().gridPosition == position)
            {
                return aSpace;
            }
        }
        //throw new MissingReferenceException("Grid Space with grid position " + position.ToString() + " not found");
        return null;
    }

    public static GameObject findNearestGridSpace(Vector3 position)
    {
        Vector3 closestVector = new Vector3(-999999999999999999, -999999999999999999, 0);
        GameObject closestSpace = null;
        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            Vector3 spacePosIgnoreZ = new Vector3(aSpace.transform.position.x, aSpace.transform.position.y, position.z);
            if (Vector3.Distance(position, spacePosIgnoreZ) < Vector3.Distance(position, closestVector))
            {
                closestVector = spacePosIgnoreZ;
                closestSpace = aSpace;
            }
        }
        return closestSpace;
    }

    //returns the nearest grid space, unless the closest grid space is further away than range, then returns null
    public static GameObject findNearestGridSpaceWithinRange(Vector3 position, float range)
    {
        GameObject closestSpace = findNearestGridSpace(position);
        Vector3 spacePosIgnoreZ = new Vector3(closestSpace.transform.position.x, closestSpace.transform.position.y, position.z);
        if (Vector3.Distance(position, spacePosIgnoreZ) < range)
        {
            return closestSpace;
        }
        return null;
    }

    public static GameObject findGridEndSpace(int x, int y)
    {
        return findGridEndSpace(new Vector2(x, y));
    }

    public static GameObject findGridEndSpace(Vector2 position)
    {
        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("End Space"))
        {
            if (aSpace.GetComponent<Space_Script>().gridPosition == position)
            {
                return aSpace;
            }
        }
        //throw new MissingReferenceException("Grid Space with grid position " + position.ToString() + " not found");
        return null;
    }
}
