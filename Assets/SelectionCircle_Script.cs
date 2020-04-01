using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle_Script : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (User_Input_Script.currentlySelectedMinion != null)
        {
            this.transform.position = User_Input_Script.currentlySelectedMinion.transform.position;
        }
    }
}
