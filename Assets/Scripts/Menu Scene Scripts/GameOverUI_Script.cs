using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.Find("Battles Won Text").GetComponent<Text>().text = "Total Battles Won: \n\t" + Stat_Tracking_Script.getBattlesWonStat();
        this.gameObject.transform.Find("Encounters Cleared Text").GetComponent<Text>().text = "Total Encounters Cleared: \n\t" + Stat_Tracking_Script.getEncountersClearedStat();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchToMainMenu()
    {
        Stat_Tracking_Script.setBattlesWonStat(0);
        Stat_Tracking_Script.setEncountersClearedStat(0);
        GameObject.FindGameObjectWithTag("Transition Panel").GetComponent<Scene_Transition_Script>().sceneToMoveTo = "Main Menu Scene";
        GameObject.FindGameObjectWithTag("Transition Panel").GetComponent<Animator>().SetTrigger("Play Scene Outro Transition");
    }
}
