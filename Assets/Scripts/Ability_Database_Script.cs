using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Database_Script : MonoBehaviour
{
    [Header("Molotov Ability")]
    public GameObject molotovEffectPrefab;
    public float molotovCooldown;
    public AbilityType molotovType;

    [Header("SprayAndPray Ability")]
    public float sprayAndPrayCooldown;
    public AbilityType sprayAndPrayType;

    //Abilty Class Blueprint
    // AbilityID
    // cooldown
    // casttype
    // Map<String, Object> Extras
    // start()
    // cast()

    public enum AbilityType
    {
        aimCast,
        instantCast,
        passive,
        none
    }

    public enum AbilityID
    {
        molotov,
        sprayAndPray,
        teleport,
        buildBarricade,
        fasterAttackPassive,
        none
    }

    public void cast(AbilityID abilityToCast, int abilityIndex, GameObject caster, GameObject targetGridSpace)
    {
        switch(abilityToCast)
        {
            case AbilityID.molotov: castMolotov(caster, abilityIndex, targetGridSpace); break;
            case AbilityID.sprayAndPray: castSprayAndPray(caster, abilityIndex); break;
        }
    }

    public void castMolotov(GameObject caster, int abilityIndex, GameObject targetGridSpace)
    {
        Debug.Log("Throwing Molotov");
        GameObject molotov = Instantiate(molotovEffectPrefab, targetGridSpace.transform.position, targetGridSpace.transform.rotation);
        molotov.GetComponent<Molotov_Effect_Script>().mySpace = targetGridSpace;
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex , molotovCooldown);
    }

    public void castSprayAndPray(GameObject caster, int abilityIndex)
    {
        Debug.Log("Casting Spray and Pray");
        caster.GetComponent<Minion_Movement_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().sprayAndPrayBuff);
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex, molotovCooldown);
    }

    public float getCooldown(AbilityID ability)
    {
        switch(ability)
        {
            case AbilityID.molotov: return molotovCooldown;
            case AbilityID.sprayAndPray: return sprayAndPrayCooldown;
        }
        Debug.LogError(ability + " has undefined abilityCooldown, returning 0.0f.");
        return 0.0f;
    }

    public AbilityType getAbilityType(AbilityID ability)
    {
        switch(ability)
        {
            case AbilityID.molotov: return molotovType;
            case AbilityID.sprayAndPray: return sprayAndPrayType;
        }
        Debug.LogError(ability + " has undefined abilityType, returning AbilityType.none.");
        return AbilityType.none;
    }
}
