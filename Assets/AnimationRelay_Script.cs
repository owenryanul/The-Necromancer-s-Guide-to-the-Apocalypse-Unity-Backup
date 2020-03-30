﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRelay_Script : MonoBehaviour
{
    //This class is for relaying animation events from animation rigs to the parent objects
    public void onEnemyDeathAnimationDone()
    {
        this.GetComponentInParent<Enemy_AI_script>().onDeathAnimationDone();
    }

    public void onMeleeAttackAnimationFinished()
    {
        this.GetComponentInParent<Minion_Movement_Script>().onMeleeAttackAnimationFinished();
    }

    public void onRangedAttackAnimationFinished()
    {
        
    }
}
