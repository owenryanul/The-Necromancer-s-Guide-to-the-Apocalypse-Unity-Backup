using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dark_Energy_Meter_Script : MonoBehaviour
{
    private static int darkEnergy;
    public int startingDarkEnergy = 10;
    public int meterFilledInAt = 300;
    public GameObject darkEnergyParticle;

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

    public void addDarkEnergyOnEnemySlain(int darkEnergyIn, GameObject enemy)
    {
        bool noEnemyBeingConverted = true;
        foreach(GameObject anEnemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(anEnemy.GetComponent<Enemy_AI_script>().isBeingConverted())
            {
                anEnemy.GetComponent<Enemy_AI_script>().addProgressToConversion(darkEnergyIn);
                GameObject effect = Instantiate(darkEnergyParticle, enemy.transform.position, enemy.transform.rotation);
                effect.GetComponent<Dark_Energy_Particle_Script>().target = anEnemy.transform.position;
                noEnemyBeingConverted = false;
                break;
            }
        }

        if (noEnemyBeingConverted)
        {
            darkEnergy += darkEnergyIn;
        }
    }
}
