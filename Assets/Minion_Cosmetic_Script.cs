using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CosmeticDatabase = Cosmetic_Database_Script;
using CosmeticID = Cosmetic_Database_Script.CosmeticID ;

public class Minion_Cosmetic_Script : MonoBehaviour
{
    private bool hasRunSetup;

    public bool isEnemy;
    public CosmeticID Hat;
    public CosmeticID Torso;
    private const string hatsprite_Path = "Skeleton Rig/bone_1/bone_16/bone_2/bone_3/Hat Sprite";
    private const string hatsprite_Enemy_Path = "Zomble Rig/bone_1/bone_2/bone_3/bone_4/Hat Sprite";
    private const string torsosprite_Path = "Skeleton Rig/bone_1/bone_16/Torso Sprite";
    private const string torsosprite_Enemy_Path = "Zomble Rig/bone_1/bone_2/Torso Sprite";

    // Start is called before the first frame update
    void Start()
    {
        hasRunSetup = false;
    }

    // Run during the first frame update rather than before, to ensure that all databases have been instanced and static calls won't throw exceptions for being uninstanced yet.
    void StartAfterDatabases()
    {
        this.applyAllCosmetics();
        hasRunSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Run applyCosmetics on start but after Cosmetic Database as run it's OnStart()
        if(!hasRunSetup)
        {
            StartAfterDatabases();
        }
    }

    private void applyAllCosmetics()
    {
        if (isEnemy)
        {
            applyCosmetic(Hat, hatsprite_Enemy_Path);
            applyCosmetic(Torso, torsosprite_Enemy_Path);
        }
        else
        {
            applyCosmetic(Hat, hatsprite_Path);
            applyCosmetic(Torso, torsosprite_Path);
        }
        
    }

    public void applyCosmetic(CosmeticID cosmetic, string path)
    {
        GameObject cosmeticSprite = this.transform.Find(path).gameObject;
        if (cosmetic != CosmeticID.None)
        {
            cosmeticSprite.GetComponent<SpriteRenderer>().enabled = true;
            cosmeticSprite.GetComponent<SpriteRenderer>().sprite = CosmeticDatabase.findCosmetic(cosmetic).cosmeticSprite;
            //Debug.Log(this.gameObject.name + "'s Hat Pos: " + CosmeticDatabase.findCosmetic(Hat).offset + " with rotation: " + CosmeticDatabase.findCosmetic(Hat).rotation);
            if (isEnemy)
            {
                cosmeticSprite.transform.localPosition = CosmeticDatabase.findCosmetic(cosmetic).enemyOffset;
                cosmeticSprite.transform.localEulerAngles = CosmeticDatabase.findCosmetic(cosmetic).enemyRotation;
            }
            else
            {
                cosmeticSprite.transform.localPosition = CosmeticDatabase.findCosmetic(cosmetic).offset;
                cosmeticSprite.transform.localEulerAngles = CosmeticDatabase.findCosmetic(cosmetic).rotation;
            }
        }
        else
        {
            cosmeticSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
