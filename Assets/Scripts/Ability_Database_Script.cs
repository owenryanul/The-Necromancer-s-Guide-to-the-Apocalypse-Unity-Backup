using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Database_Script : MonoBehaviour
{
    [Header("Molotov Ability")]
    public GameObject molotovEffectPrefab;

    public enum Ability
    {
        molotov,
        teleport,
        buildBarricade,
        fasterAttackPassive,
        none
    }

    public void cast(Ability abilityToCast,GameObject caster, GameObject targetGridSpace)
    {
        switch(abilityToCast)
        {
            case Ability.molotov: castMolotov(caster, targetGridSpace); break;
        }
    }

    public void castMolotov(GameObject caster, GameObject targetGridSpace)
    {
        Debug.Log("Throwing Molotov");
        GameObject molotov = Instantiate(molotovEffectPrefab, targetGridSpace.transform.position, targetGridSpace.transform.rotation);
        molotov.GetComponent<Molotov_Effect_Script>().mySpace = targetGridSpace;
    }
}
