using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dark_Energy_Meter_Script : MonoBehaviour
{
    private static int darkEnergy;
    public int startingDarkEnergy = 10;
    public int meterFilledInAt = 300;

    // Start is called before the first frame update
    void Start()
    {
        darkEnergy = startingDarkEnergy;
    }

    // Update is called once per frame
    void Update()
    {
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

    public static int getDarkEnergy()
    {
        return darkEnergy;
    }

    public static void setDarkEnergy(int darkEnergyIn)
    {
        darkEnergy = darkEnergyIn;
    }

    public static void addDarkEnergy(int darkEnergyIn)
    {
        darkEnergy += darkEnergyIn;
    }
}
