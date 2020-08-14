using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ability_Database = Ability_Database_Script;
using WeaponID = Weapon_Database_Script.WeaponID;
using CosmeticID = Cosmetic_Database_Script.CosmeticID;

public class Enemy_AI_Berserker_Script : Enemy_AI_script
{

    protected bool isBeserk;
    protected bool isPlayingBeserkAnimation;

    [Header("Beserker Stats")]
    public int beserkerSpeedBoost;

    void Start()
    {
        isBeserk = false;
        base.Start();
    }

    //Override
    void Update()
    {
        if (base.isDying)
        {
            return; //override update if playing the death animation
        }

        if (this.beingConverted)
        {
            base.conversionUpdate();
        }
        else
        {
            UpdateBeserkAI();

            base.rigAnimator.SetBool("IsWalking", isMoving);
            if (base.isMoving)
            {
                base.updateSpriteSortingLayers();
            }
        }
    }

    void UpdateBeserkAI()
    {
        if(!isBeserk && base.currentHP < base.maxHP)
        {
            base.rigAnimator.SetTrigger("StartRaging");
            isPlayingBeserkAnimation = true;
            isBeserk = true;
        }

        if (!isPlayingBeserkAnimation)
        {
            base.basicEnemyMovementAndAttacksUpdate();
        }
    }

    public void onBeserkAnimationDone()
    {
        base.speed += this.beserkerSpeedBoost;
        isPlayingBeserkAnimation = false;
    }

    protected override void createNewMinionFromEnemy()
    {
        MinionRoster.addNewMinion(1, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, Ability_Database.findRandomAbilityWithTag("beserker").id, Ability_Database.findRandomAbilityWithTag("beserker").id, Ability_Database.AbilityID.fleetOfFoot, CosmeticID.None, CosmeticID.None, CosmeticID.Crazy_Paint);
    }
}
