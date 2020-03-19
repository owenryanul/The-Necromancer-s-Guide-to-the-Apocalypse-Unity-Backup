using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Movement_Script : MonoBehaviour
{
    public GameObject mySpace;
    public GameObject targetSpace;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mySpacePos = mySpace.transform.position;
        mySpacePos.z = this.transform.position.z;
        if(mySpacePos != this.transform.position)
        {
            Vector3 moveVector = (mySpacePos - this.transform.position).normalized;
            moveVector.z = 0;
            this.transform.position += moveVector * (speed * Time.deltaTime);
        }
        else if (mySpace != targetSpace)
        {
            //TODO: get closest space
            mySpace = targetSpace;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Minion Clicked");
        foreach(GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            aSpace.GetComponent<Space_Script>().setCurrentlySelectedMinion(this.gameObject);
        }        
    }

    public void setTargetSpace(GameObject spaceIn)
    {
        targetSpace = spaceIn;
    }
}
