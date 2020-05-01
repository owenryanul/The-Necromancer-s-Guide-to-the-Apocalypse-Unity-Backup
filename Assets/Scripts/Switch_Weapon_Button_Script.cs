﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch_Weapon_Button_Script : MonoBehaviour
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
            showButton();
        }
        else
        {
            hideButton();
        }
    }

    private void showButton()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        this.gameObject.GetComponentInChildren<Text>().enabled = true;
        this.transform.Find("Current Weapon").GetComponent<Image>().enabled = true;
        this.transform.Find("Other Weapon").GetComponent<Image>().enabled = true;

        this.transform.Find("Current Weapon").GetComponent<Image>().sprite = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getCurrentWeaponIcon();
        this.transform.Find("Other Weapon").GetComponent<Image>().sprite = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getOtherWeaponIcon();
    }

    private void hideButton()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        this.gameObject.GetComponentInChildren<Text>().enabled = false;
        this.transform.Find("Current Weapon").GetComponent<Image>().enabled = false;
        this.transform.Find("Other Weapon").GetComponent<Image>().enabled = false;
    }
}
