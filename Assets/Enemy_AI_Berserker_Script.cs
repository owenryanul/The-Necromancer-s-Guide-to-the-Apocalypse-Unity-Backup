using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ability_Database = Ability_Database_Script;

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
        MinionRoster.addNewMinion(1, Weapon_Database_Script.WeaponID.Thrown_bone, Weapon_Database_Script.WeaponID.Unarmed_Melee, Ability_Database.findRandomAbilityWithTag("beserker").id, Ability_Database.findRandomAbilityWithTag("beserker").id, Ability_Database.AbilityID.fleetOfFoot);
    }
}
