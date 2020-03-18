using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gridboard_Script : MonoBehaviour
{
    //Gridspace[y,x],  (0,0) is top left grid space. +y = down. +x = right.

    public int width = 5;
    public int height = 5;

    public List<GridSpace> spaces;

    // Start is called before the first frame update
    void Start()
    {
        if(spaces.Count != (width * height))
        {
            throw new System.Exception("GridBoard_Exception: Grid Size does not match Grid Dimensions");
        }
        resetSpacesXandY();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetSpacesXandY()
    {
        int x = 0;
        int y = 0;
        foreach(GridSpace aSpace in spaces)
        { 
            aSpace.gridX = x;
            aSpace.gridY = y;

            x++;
            if (x >= width)
            {
                x = 0;
                y++;
            }
        }
    }

   

    public GridSpace getGridSpace(int x, int y)
    {
        return spaces[(width * y) + x];
    }

    //returns the gridspace above the provided space, if no space is above, then returns null
    public GridSpace getSpaceUp(GridSpace space)
    {
        if (space.gridY != 0)
        {
            return getGridSpace(space.gridX, space.gridY - 1);
        }
        return null;
    }

    //returns the gridspace below the provided space, if no space is below, then returns null
    public GridSpace getSpaceDown(GridSpace space)
    {
        if (space.gridY != (height - 1))
        {
            return getGridSpace(space.gridX, space.gridY + 1);
        }
        return null;
    }

    //returns the gridspace left of the provided space, if no space is left, then returns null
    public GridSpace getSpaceLeft(GridSpace space)
    {
        if (space.gridX != 0)
        {
            return getGridSpace(space.gridX - 1, space.gridY);
        }
        return null;
    }

    //returns the gridspace right of the provided space, if no space is right, then returns null
    public GridSpace getSpaceRight(GridSpace space)
    {
        if (space.gridX != (width - 1))
        {
            return getGridSpace(space.gridX + 1, space.gridY);
        }
        return null;
    }



    [System.Serializable]
    public class GridSpace
    {
        public int gridX;
        public int gridY;
        public GameObject spaceObject;
    }
}
