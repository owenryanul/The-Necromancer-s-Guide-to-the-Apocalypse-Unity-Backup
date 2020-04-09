using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Database_Script : MonoBehaviour
{
    [Header("Molotov Ability")]
    public GameObject molotovEffectPrefab;
    public float molotovCooldown;

    public enum Ability
    {
        molotov,
        teleport,
        buildBarricade,
        fasterAttackPassive,
        none
    }

    public void cast(Ability abilityToCast, int abilityIndex, GameObject caster, GameObject targetGridSpace)
    {
        switch(abilityToCast)
        {
            case Ability.molotov: castMolotov(caster, abilityIndex, targetGridSpace); break;
        }
    }

    public void castMolotov(GameObject caster, int abilityIndex, GameObject targetGridSpace)
    {
        Debug.Log("Throwing Molotov");
        GameObject molotov = Instantiate(molotovEffectPrefab, targetGridSpace.transform.position, targetGridSpace.transform.rotation);
        molotov.GetComponent<Molotov_Effect_Script>().mySpace = targetGridSpace;
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex , molotovCooldown);
    }

    public float getCooldown(Ability ability)
    {
        switch(ability)
        {
            case Ability.molotov: return molotovCooldown;
        }
        return 0.0f;
    }
}
