using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
