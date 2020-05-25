using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CosmeticDatabase = Cosmetic_Database_Script;
using CosmeticID = Cosmetic_Database_Script.CosmeticID ;

public class Minion_Cosmetic_Script : MonoBehaviour
{
    private bool hasRunSetup;


    public CosmeticID Hat;
    private const string hatsprite_Path = "Skeleton Rig/bone_1/bone_16/bone_2/bone_3/Hat Sprite";

    // Start is called before the first frame update
    void Start()
    {
        hasRunSetup = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Run applyCosmetics on start but after Cosmetic Database as run it's OnStart()
        if(!hasRunSetup)
        {
            this.applyCosmetics();
            hasRunSetup = true;
        }
    }

    public void applyCosmetics()
    {
        GameObject hatSprite = this.transform.Find(hatsprite_Path).gameObject;
        if(Hat == CosmeticID.None)
        {
            hatSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            hatSprite.GetComponent<SpriteRenderer>().enabled = true;
            hatSprite.GetComponent<SpriteRenderer>().sprite = CosmeticDatabase.findCosmetic(Hat).cosmeticSprite;
            Debug.Log(this.gameObject.name + "'s Hat Pos: " + CosmeticDatabase.findCosmetic(Hat).offset + " with rotation: " + CosmeticDatabase.findCosmetic(Hat).rotation);
            hatSprite.transform.localPosition = CosmeticDatabase.findCosmetic(Hat).offset;
            hatSprite.transform.localEulerAngles = CosmeticDatabase.findCosmetic(Hat).rotation;
        }
        
    }
}
