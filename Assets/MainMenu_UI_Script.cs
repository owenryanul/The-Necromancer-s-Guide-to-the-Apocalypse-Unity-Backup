using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //OnClick Listener for the NewGame Button. Starts a new game.
    public void newGame()
    {
        Player_Inventory_Script.loadInventoryFromPlayerSaveFile(Player_Inventory_Script.getPlayerName());
        Map_State_Storage_Script.flagMapForRegeneration();
        SceneManager.LoadSceneAsync("Map Scene", LoadSceneMode.Single);
    }
}
