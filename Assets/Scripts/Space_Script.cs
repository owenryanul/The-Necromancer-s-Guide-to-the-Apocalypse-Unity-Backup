using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ability_Database = Ability_Database_Script;

public class Space_Script : MonoBehaviour, MouseDownOverrider
{
    public Vector2 gridPosition = new Vector2(0,0);
    private Minion_Roster_Script rosterScript;


    // Start is called before the first frame update
    void Start()
    {
        rosterScript = GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<Minion_Roster_Script>();
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
                User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().setTargetSpace(this.gameObject);
                User_Input_Script.setCurrentlySelectedMinion(null); //Will make selection circle disappear
            }
            //If aiming an ability, cast the ability targeting this space
            else if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.CastAbilityOnSpace)
            {
                Ability_Database.cast(User_Input_Script.currentAbilityToCast, User_Input_Script.currentAbilityIndex, User_Input_Script.currentlySelectedMinion, this.gameObject);
                User_Input_Script.currentMouseCommand = User_Input_Script.MouseCommand.SelectOrMove;
                //Circle does not disappear because it goes back to the ability caster, who s the currently selected minion
            }
        }
        else if (User_Input_Script.currentlySelectedMinion == null)
        {
            //if aiming minion summon, and the space is unoccupied, spawn the minion to be summoned on this space
            if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.SummonMinion)
            {
                bool thisSpaceIsUnoccupied = true;
                foreach(GameObject aMinion in GameObject.FindGameObjectsWithTag("Minion"))
                {
                    if(aMinion.GetComponent<Minion_AI_Script>().getTargetSpace() == this.gameObject)
                    {
                        thisSpaceIsUnoccupied = false;
                        break;
                    }
                }

                if (thisSpaceIsUnoccupied)
                {
                    summonMinion(User_Input_Script.currentMinionToSummon);
                    User_Input_Script.currentMouseCommand = User_Input_Script.MouseCommand.SelectOrMove;
                    User_Input_Script.setCurrentlySelectedMinion(null); //Will make selection circle disappear
                }
            }
        }
    }

    private void summonMinion(Minion_Roster_Script.MinionEntry minionToSummonData)
    {
        Debug.Log("Summoning Minion");
        Vector3 spawnVector = this.gameObject.transform.position;
        spawnVector.z = 0;
        GameObject summonedMinion = Instantiate(rosterScript.minion_prefab, spawnVector, this.gameObject.transform.rotation);
        summonedMinion.GetComponent<Minion_AI_Script>().mySpace = this.gameObject;
        summonedMinion.GetComponent<Minion_AI_Script>().setTargetSpace(this.gameObject);

        summonedMinion.GetComponent<Minion_AI_Script>().minionID = minionToSummonData.minionID;
        summonedMinion.GetComponent<Minion_AI_Script>().name = minionToSummonData.minionName;
        summonedMinion.GetComponent<Minion_AI_Script>().cost = minionToSummonData.minionSummonCost;
        summonedMinion.GetComponent<Minion_AI_Script>().baseMoveSpeed = minionToSummonData.baseMovementSpeed;
        summonedMinion.GetComponent<Minion_AI_Script>().isFacingRight = true;
        summonedMinion.GetComponent<Minion_AI_Script>().MaxHp = minionToSummonData.minionMaxHp;
        summonedMinion.GetComponent<Minion_AI_Script>().currentHp = minionToSummonData.minionMaxHp;
        summonedMinion.GetComponent<Minion_AI_Script>().weapon1 = minionToSummonData.Weapon1ID;
        summonedMinion.GetComponent<Minion_AI_Script>().weapon2 = minionToSummonData.Weapon2ID;
        summonedMinion.GetComponent<Minion_AI_Script>().Ability1 = minionToSummonData.ability1ID;
        summonedMinion.GetComponent<Minion_AI_Script>().Ability2 = minionToSummonData.ability2ID;
        summonedMinion.GetComponent<Minion_AI_Script>().Ability3 = minionToSummonData.ability3ID;
        summonedMinion.GetComponent<Minion_Cosmetic_Script>().Hat = minionToSummonData.hat;
        summonedMinion.GetComponent<Minion_Cosmetic_Script>().Torso = minionToSummonData.torso;
        summonedMinion.GetComponent<Minion_Cosmetic_Script>().Mask = minionToSummonData.mask;

        Player_Inventory_Script.addPlayersDarkEnergy(-minionToSummonData.minionSummonCost);
        Enemy_Spawning_Script.addBattleStat_EnergySpent(minionToSummonData.minionSummonCost);
        //Mark the MinionEntry in the roster as summoned, disabling the button so it can't be summoned again. On death a minion will flag the entry as unSummoned.
        GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<Minion_Roster_Script>().flagMinionAsSummoned(minionToSummonData.minionID, true);
    }

    public static GameObject findGridSpace(int x, int y, bool returnNearestEndSpaceIfSpaceNotFound = false)
    {
        return findGridSpace(new Vector2(x, y), returnNearestEndSpaceIfSpaceNotFound);
    }

    public static GameObject findGridSpace(Vector2 position, bool returnNearestEndSpaceIfSpaceNotFound = false)
    {    
        foreach(GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if(aSpace.GetComponent<Space_Script>().gridPosition == position)
            {
                return aSpace;
            }
        }

        //If a gridSpace matching the provided position cannot be found, and if instructed to in methodCall, will attempt to return the endSpace closest to the provided co-ordinates.
        if (returnNearestEndSpaceIfSpaceNotFound)
        {
            GameObject endSpace = findGridEndSpace(position);
            if(endSpace != null)
            {
                return endSpace;
            }
            else
            {
                return findEndSpaceOnSameRow(position);
            }
        }
        else //otherwise return null.
        {
            //throw new MissingReferenceException("Grid Space with grid position " + position.ToString() + " not found");
            return null;
        }
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

    public static GameObject findEndSpaceOnSameRow(Vector2 position)
    {
        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("End Space"))
        {
            if (aSpace.GetComponent<Space_Script>().gridPosition.y == position.y && (Mathf.Sign(aSpace.GetComponent<Space_Script>().gridPosition.x) == Mathf.Sign(position.x)))
            {
                return aSpace;
            }
        }
        //throw new MissingReferenceException("Grid Space with grid position " + position.ToString() + " not found");
        return null;
    }
}
