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

    public void newGame()
    {
        Player_Inventory_Script.loadInventoryFromPlayerSaveFile(Player_Inventory_Script.getPlayerName());
        SceneManager.LoadSceneAsync("Map Scene", LoadSceneMode.Single);
    }
}
