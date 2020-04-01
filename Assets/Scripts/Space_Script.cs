using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_Script : MonoBehaviour
{
    public Vector2 gridPosition = new Vector2(0,0);
    

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
        if (User_Input_Script.currentlySelectedMinion != null)
        {
            User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>().setTargetSpace(this.gameObject);
            User_Input_Script.setCurrentlySelectedMinion(null);
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
