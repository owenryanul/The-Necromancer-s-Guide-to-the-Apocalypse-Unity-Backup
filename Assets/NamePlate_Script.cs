using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePlate_Script : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (User_Input_Script.currentlySelectedMinion != null && !User_Input_Script.currentlySelectedMinion.CompareTag("Necromancer"))
        {
            showNamePlate();
            Minion_Movement_Script minion = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>();
            this.gameObject.GetComponentInChildren<Text>().text = minion.name;
        }
        else
        {
            hideNamePlate();
        }
    }

    private void showNamePlate()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        this.gameObject.GetComponentInChildren<Text>().enabled = true;
    }

    private void hideNamePlate()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        this.gameObject.GetComponentInChildren<Text>().enabled = false;
    }
}
