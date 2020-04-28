using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Database_Script : MonoBehaviour
{
    public enum AbilityType
    {
        aimCast,
        instantCast,
        targetEnemyCast,
        passive,
        none
    }

    public enum AbilityID
    {
        necromancer_ConversationRitual,
        molotov,
        sprayAndPray,
        fasterBullets,
        teleport,
        buildBarricade,
        fasterAttackPassive,
        fleetOfFoot,
        none
    }

    [System.Serializable]
    public class Ability
    {
        [Header("ID")]
        public AbilityID id;
        [Header("Common Data")]
        public AbilityType castType;
        public float cooldown;
        [TextArea(2,10)]
        public string abilityToolTip;
        [Header("Ability Specfic Data")]
        public List<AbilityExtra> extras; //Used by some abilties for extra data, e.g. molotov's effects prefab

        public Ability(AbilityID inID, AbilityType castTypeIn, float cooldownIn)
        {
            this.id = inID;
            this.castType = castTypeIn;
            this.cooldown = cooldownIn;
            this.extras = new List<AbilityExtra>();
        }

        public Ability(AbilityID inID, AbilityType castTypeIn, float cooldownIn, string[] keys)
        {
            this.id = inID;
            this.castType = castTypeIn;
            this.cooldown = cooldownIn;
            this.extras = new List<AbilityExtra>();
            foreach (string aKey in keys)
            {
                this.extras.Add(new AbilityExtra(aKey));
            }
        }

        public Object getExtra(string keyIn)
        {
            foreach (AbilityExtra anExtra in extras)
            {
                if (string.Equals(anExtra.key, keyIn))
                {
                    return anExtra.value;
                }
            }
            return null;
        }

        [System.Serializable]
        public class AbilityExtra
        {
            public string key;
            public Object value;

            public AbilityExtra(string keyIn) { this.key = keyIn; }
        }
    }

    public void cast(AbilityID abilityToCast, int abilityIndex, GameObject caster, GameObject targetGridSpace)
    {
        switch (abilityToCast)
        {
            case AbilityID.necromancer_ConversationRitual: castNecromancerConversionRitual(caster, targetGridSpace); break;
            case AbilityID.molotov: castMolotov(caster, abilityIndex, targetGridSpace); break;
            case AbilityID.sprayAndPray: castSprayAndPray(caster, abilityIndex); break;
            case AbilityID.fasterBullets: castFasterBullets(caster, abilityIndex); break;
            case AbilityID.fleetOfFoot: castFleetOfFoot(caster, abilityIndex); break;
        }
    }

    public float getCooldown(AbilityID abilityIDin)
    {
        return findAbility(abilityIDin).cooldown;
    }

    public AbilityType getAbilityType(AbilityID abilityIDin)
    {
        return findAbility(abilityIDin).castType;
    }

    public string getAbilityTooltip(AbilityID abilityIDin)
    {
        return findAbility(abilityIDin).abilityToolTip;
    }

    public Ability findAbility(AbilityID abilityIDin)
    {
        switch (abilityIDin)
        {
            case AbilityID.necromancer_ConversationRitual: return ABILITY_NECROMANCER_ConversionRitual;
            case AbilityID.molotov: return ABILITY_Throw_Molotov;
            case AbilityID.sprayAndPray: return ABILITY_SprayAndPray;
            case AbilityID.fasterBullets: return ABILITY_FasterBullets;
            case AbilityID.fleetOfFoot: return ABILITY_Fleet_Of_Foot;
            default: return ABILITY_NONE;
        }

    }

    /*To Add New Ability:
        1. Add Ability below
        2. Add Ability to findAbility() method
        3. Add CastNewAbility() method for new ability
        4. Add Cast method to cast() method above 
    */
    private Ability ABILITY_NONE = new Ability(AbilityID.none, AbilityType.none, 0.0f);

    public Ability ABILITY_NECROMANCER_ConversionRitual = new Ability(AbilityID.necromancer_ConversationRitual, AbilityType.targetEnemyCast, 0.0f);

    public Ability ABILITY_Throw_Molotov = new Ability(AbilityID.molotov, AbilityType.aimCast, 5.0f, new string[] { "MolotovEffectPrefab" });
    public Ability ABILITY_SprayAndPray = new Ability(AbilityID.sprayAndPray, AbilityType.instantCast, 5.0f);
    public Ability ABILITY_FasterBullets = new Ability(AbilityID.fasterBullets, AbilityType.instantCast, 5.0f);
    public Ability ABILITY_Fleet_Of_Foot = new Ability(AbilityID.fleetOfFoot, AbilityType.passive, 5.0f);

    private void castNecromancerConversionRitual(GameObject caster, GameObject targetEnemy)
    {
        Debug.Log("Casting Conversion Ritual");
        Debug.Log("Casting Conversion Ritual targeting " + targetEnemy.GetComponent<Enemy_AI_script>().name);
        targetEnemy.GetComponent<Enemy_AI_script>().setBeingConverted(true);
    }

    private void castMolotov(GameObject caster, int abilityIndex, GameObject targetGridSpace)
    {
        Debug.Log("Throwing Molotov");
        //Ability molotovAbility = findAbility(AbilityID.molotov);
        GameObject molotov = Instantiate((GameObject)ABILITY_Throw_Molotov.getExtra("MolotovEffectPrefab"), targetGridSpace.transform.position, targetGridSpace.transform.rotation);
        molotov.GetComponent<Molotov_Effect_Script>().mySpace = targetGridSpace;
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex, ABILITY_Throw_Molotov.cooldown);
    }

    private void castSprayAndPray(GameObject caster, int abilityIndex)
    {
        Debug.Log("Casting Spray and Pray");
        //Ability sprayAndPray = findAbility(AbilityID.sprayAndPray);
        caster.GetComponent<Minion_Movement_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().sprayAndPrayBuff);
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex, ABILITY_SprayAndPray.cooldown);
    }

    private void castFasterBullets(GameObject caster, int abilityIndex)
    {
        Debug.Log("Casting Faster Bullets");
        //Ability fasterBullets = findAbility(AbilityID.fasterBullets);
        caster.GetComponent<Minion_Movement_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().fasterBulletsBuff);
        caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex, ABILITY_FasterBullets.cooldown);
    }

    private void castFleetOfFoot(GameObject caster, int abilityIndex)
    {
        Debug.Log("Casting Fleet of Foot");
        //Ability fasterBullets = findAbility(AbilityID.fasterBullets);
        caster.GetComponent<Minion_Movement_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().fleetOfFootBuff);
        //caster.GetComponent<Minion_Movement_Script>().setAbilityCooldown(abilityIndex, ABILITY_FasterBullets.cooldown);
    }
}
