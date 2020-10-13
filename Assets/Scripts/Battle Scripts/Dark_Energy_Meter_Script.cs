﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dark_Energy_Meter_Script : MonoBehaviour
{
    private static Dark_Energy_Meter_Script instance; 
    private int darkEnergy;
    //public int startingDarkEnergy = 10;
    public int meterFilledInAt = 300;
    public GameObject darkEnergyMote;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //darkEnergy = startingDarkEnergy;
        //darkEnergy = Player_Inventory_Script.getPlayersDarkEnergy();
    }

    // Update is called once per frame
    void Update()
    {
        darkEnergy = Player_Inventory_Script.getPlayersDarkEnergy();
        GameObject.FindGameObjectWithTag("Dark Energy Meter Text").GetComponent<Text>().text = (darkEnergy + "\nDark Energy");
        if(darkEnergy > meterFilledInAt)
        {
            this.gameObject.GetComponentInChildren<SpriteMask>().alphaCutoff = (1.0f);
        }
        else
        {
            this.gameObject.GetComponentInChildren<SpriteMask>().alphaCutoff = (1.0f * (darkEnergy / (float)meterFilledInAt));
        }
        
    }

    public static void addDarkEnergyOnEnemySlain(int darkEnergyIn, GameObject enemy)
    {
        bool noEnemyBeingConverted = true;
        foreach(GameObject anEnemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(anEnemy.GetComponent<Enemy_AI_script>().isBeingConverted())
            {
                anEnemy.GetComponent<Enemy_AI_script>().addProgressToConversion(darkEnergyIn);
                GameObject effect = Instantiate(instance.darkEnergyMote, enemy.transform.position, enemy.transform.rotation);
                effect.GetComponent<Dark_Energy_Mote_Script>().target = anEnemy.transform.position;
                noEnemyBeingConverted = false;
                break;
            }
        }

        if (noEnemyBeingConverted)
        {
            Player_Inventory_Script.addPlayersDarkEnergy(darkEnergyIn);
            Enemy_Spawning_And_Horde_Manager_Script.addBattleStat_EnergyEarned(darkEnergyIn);
        }
    }
}
