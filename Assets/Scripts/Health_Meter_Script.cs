using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Meter_Script : MonoBehaviour
{
    private GameObject healthMeterText;

    // Start is called before the first frame update
    void Start()
    {
        healthMeterText = GameObject.FindGameObjectWithTag("Health Meter Text");
    }

    // Update is called once per frame
    void Update()
    {
        if(User_Input_Script.currentlySelectedMinion != null && !User_Input_Script.currentlySelectedMinion.CompareTag("Necromancer"))
        {
            showHealthMeter();
            Minion_Movement_Script minion = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>();
            healthMeterText.GetComponent<Text>().text = minion.currentHp + "/" + minion.MaxHp;
            this.gameObject.GetComponentInChildren<SpriteMask>().alphaCutoff = 1.0f - (1.0f * ((float)minion.currentHp / (float)minion.MaxHp));
        }
        else
        {
            hideHealthMeter();
        }
    }

    private void showHealthMeter()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        healthMeterText.GetComponent<Text>().enabled = true;
    }

    private void hideHealthMeter()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        healthMeterText.GetComponent<Text>().enabled = false;
    }

}
