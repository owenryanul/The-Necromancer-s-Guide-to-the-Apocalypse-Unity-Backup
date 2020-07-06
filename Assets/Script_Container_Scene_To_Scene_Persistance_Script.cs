using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Container_Scene_To_Scene_Persistance_Script : MonoBehaviour
{
    //Singleton Pattern
    public static Script_Container_Scene_To_Scene_Persistance_Script instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null) //If this script does not already exist, make all static methods target this script.
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else //if this script does already exist, destroy the Script_Container_Object containing this script as one already exists and this one is redundent.
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Debug.Log("Script Container has been destroyed safely.");
    }
}
