using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_UI_Script : MonoBehaviour
{

    private bool isLoading;

    // Start is called before the first frame update
    void Start()
    {
        beginLoadingGameFiles();
    }

    // Update is called once per frame
    void Update()
    {
        if(isLoading)
        {
            GameObject.Find("New Game Button").GetComponent<Button>().interactable = false;
            GameObject.Find("Loading Text").GetComponent<Text>().enabled = true;
        }
        else
        {
            GameObject.Find("New Game Button").GetComponent<Button>().interactable = true;
            GameObject.Find("Loading Text").GetComponent<Text>().enabled = false;
        }
    }

    //OnClick Listener for the NewGame Button. Starts a new game.
    public void newGame()
    {
        Player_Inventory_Script.loadInventoryFromPlayerSaveFile(Player_Inventory_Script.getPlayerName());
        Map_State_Storage_Script.flagMapForRegeneration();
        GameObject.FindGameObjectWithTag("Transition Panel").GetComponent<Scene_Transition_Script>().sceneToMoveTo = "Map Scene";
        GameObject.FindGameObjectWithTag("Transition Panel").GetComponent<Animator>().SetTrigger("Play Scene Outro Transition");
    }

    private void beginLoadingGameFiles()
    {
        isLoading = true;
        //If database folders do not exist, copy them from StreamingDataPath then call all Databases to read from files
        if(!Directory.Exists(Application.persistentDataPath + "/databases/"))
        {
            Debug.LogWarning("Directory: "+  Application.persistentDataPath + "/databases" + " does not exist. Preparing to load databases from " + Application.streamingAssetsPath);

            Directory.CreateDirectory(Application.persistentDataPath + "/databases/hordes");
            DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/databases/hordes");
            foreach (FileInfo file in di.GetFiles("*.json"))
            {
                Debug.Log("Copying file: " + file.Name);
                File.Copy(Application.streamingAssetsPath + "/databases/hordes/" + file.Name, Application.persistentDataPath + "/databases/hordes/" + file.Name, true);
            }

            Directory.CreateDirectory(Application.persistentDataPath + "/databases/names");
            di = new DirectoryInfo(Application.streamingAssetsPath + "/databases/names");
            foreach (FileInfo file in di.GetFiles("*.json"))
            {
                Debug.Log("Copying file: " + file.Name);
                File.Copy(Application.streamingAssetsPath + "/databases/names/" + file.Name, Application.persistentDataPath + "/databases/names/" + file.Name, true);
            }

            Directory.CreateDirectory(Application.persistentDataPath + "/databases/scenarios");
            di = new DirectoryInfo(Application.streamingAssetsPath + "/databases/scenarios");
            foreach (FileInfo file in di.GetFiles("*.json"))
            {
                Debug.Log("Copying file: " + file.Name);
                File.Copy(Application.streamingAssetsPath + "/databases/scenarios/" + file.Name, Application.persistentDataPath + "/databases/scenarios/" + file.Name, true);
            }

            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
            di = new DirectoryInfo(Application.streamingAssetsPath + "/saves/");
            foreach (FileInfo file in di.GetFiles("*.json"))
            {
                Debug.Log("Copying file: " + file.Name);
                File.Copy(Application.streamingAssetsPath + "/saves/" + file.Name, Application.persistentDataPath + "/saves/" + file.Name, true);
            }


            //isLoading = false;
            callAllDatabaseToReadFiles();
            isLoading = false;
        }
        else
        {
            isLoading = false;
        }
        
    }

    private void callAllDatabaseToReadFiles()
    {
        Enemy_Spawning_And_Horde_Manager_Script.loadAllHordesFromFiles();
    }
}
