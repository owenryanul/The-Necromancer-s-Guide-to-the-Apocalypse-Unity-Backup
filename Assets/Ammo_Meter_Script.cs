using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo_Meter_Script : MonoBehaviour
{
    public static Ammo_Meter_Script instance;
    public int ammoCount;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponentInChildren<Text>().text = "" + ammoCount;
    }

    public static void setAmmoCount(int inAmmo)
    {
        instance.ammoCount = inAmmo;
    }

    public static void addAmmoCount(int inAmmo)
    {
        instance.ammoCount += inAmmo;
    }

    public static int getAmmoCount()
    {
        return instance.ammoCount;
    }
}
