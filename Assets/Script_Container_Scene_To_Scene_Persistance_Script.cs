using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Container_Scene_To_Scene_Persistance_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(SceneManager.GetActiveScene().name == "Battle Test Rework Scene")
        {
            this.gameObject.GetComponent<User_Input_Script>().enabled = true;
        }
        else
        {
            this.gameObject.GetComponent<User_Input_Script>().enabled = false;
        }*/
    }

    private void OnDestroy()
    {
        Debug.Log("Script Container has been destroyed safely.");
    }
}
