using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability_Database_Script : MonoBehaviour
{
    private static Ability_Database_Script instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public enum AbilityType
    {
        aimCast,
        instantCast,
        targetEnemyCast,
        passive,
        targetAllyCast,
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
        firstAid,
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
        public int ammoCost;
        public Sprite icon;

        [TextArea(2,10)]
        public string abilityToolTip;

        [Header("Tags")]
        public List<string> tags;
        [Header("Ability Specfic Data")]
        public List<AbilityExtra> extras; //Used by some abilties for extra data, e.g. molotov's effects prefab

        public Ability(AbilityID inID, AbilityType castTypeIn, float cooldownIn)
        {
            this.id = inID;
            this.castType = castTypeIn;
            this.cooldown = cooldownIn;
            this.ammoCost = 0;
            this.extras = new List<AbilityExtra>();
            this.tags = new List<string>();
        }

        public Ability(AbilityID inID, AbilityType castTypeIn, float cooldownIn, string[] keys)
        {
            this.id = inID;
            this.castType = castTypeIn;
            this.cooldown = cooldownIn;
            this.ammoCost = 0;
            this.extras = new List<AbilityExtra>();
            foreach (string aKey in keys)
            {
                this.extras.Add(new AbilityExtra(aKey));
            }
            this.tags = new List<string>();
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

    public static void cast(AbilityID abilityToCast, int abilityIndex, GameObject caster, GameObject targetGameObject)
    {
        Player_Inventory_Script.addPlayersAmmo(-(getAbilityAmmoCost(abilityToCast)));
        Enemy_Spawning_Script.addBattleStat_AmmoSpent(getAbilityAmmoCost(abilityToCast));
        switch (abilityToCast)
        {
            case AbilityID.necromancer_ConversationRitual: instance.castNecromancerConversionRitual(caster, targetGameObject); break;
            case AbilityID.molotov: instance.castMolotov(caster, abilityIndex, targetGameObject); break;
            case AbilityID.sprayAndPray: instance.castSprayAndPray(caster, abilityIndex); break;
            case AbilityID.fasterBullets: instance.castFasterBullets(caster, abilityIndex); break;
            case AbilityID.fleetOfFoot: instance.castFleetOfFoot(caster, abilityIndex); break;
            case AbilityID.firstAid: instance.castFirstAid(caster, abilityIndex, targetGameObject); break;
        }
    }

    public static float getCooldown(AbilityID abilityIDin)
    {
        return instance.findAbility(abilityIDin).cooldown;
    }

    public static AbilityType getAbilityType(AbilityID abilityIDin)
    {
        return instance.findAbility(abilityIDin).castType;
    }

    public static Sprite getAbilityIcon(AbilityID abilityIDin)
    {
        return instance.findAbility(abilityIDin).icon;
    }

    public static string getAbilityTooltip(AbilityID abilityIDin)
    {
        return instance.findAbility(abilityIDin).abilityToolTip;
    }

    public static int getAbilityAmmoCost(AbilityID abilityIDin)
    {
        return instance.findAbility(abilityIDin).ammoCost;
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
            case AbilityID.firstAid: return ABILITY_First_Aid;
            default: return ABILITY_NONE;
        }

    }

    public static Ability findRandomAbilityWithTag(string tag)
    {
        List<Ability> abilitiesWithTag = new List<Ability>();
        addAbilityWithTag(ref abilitiesWithTag, instance.ABILITY_Throw_Molotov, tag);
        addAbilityWithTag(ref abilitiesWithTag, instance.ABILITY_SprayAndPray, tag);
        addAbilityWithTag(ref abilitiesWithTag, instance.ABILITY_FasterBullets, tag);
        addAbilityWithTag(ref abilitiesWithTag, instance.ABILITY_Fleet_Of_Foot, tag);
        addAbilityWithTag(ref abilitiesWithTag, instance.ABILITY_First_Aid, tag);

        return abilitiesWithTag[(int)(Random.value * abilitiesWithTag.Count)];
    }

    private static void addAbilityWithTag(ref List<Ability> matchingAbilities, Ability anAbility, string tag)
    {
        if(anAbility.tags.Contains(tag))
        {
            matchingAbilities.Add(anAbility);
        }
    }

    /*To Add New Ability:
        1. Add Ability below
        2. Add Ability to findAbility() method
        3. Add CastNewAbility() method for new ability
        4. Add Cast method to cast() method above 
    */

    private Ability ABILITY_NONE = new Ability(AbilityID.none, AbilityType.none, 0.0f);

    //Necromancer Abilities
    public Ability ABILITY_NECROMANCER_ConversionRitual = new Ability(AbilityID.necromancer_ConversationRitual, AbilityType.targetEnemyCast, 0.0f);

    //Minion Abilities
    public Ability ABILITY_Throw_Molotov = new Ability(AbilityID.molotov, AbilityType.aimCast, 5.0f, new string[] { "MolotovEffectPrefab" });
    public Ability ABILITY_SprayAndPray = new Ability(AbilityID.sprayAndPray, AbilityType.instantCast, 5.0f);
    public Ability ABILITY_FasterBullets = new Ability(AbilityID.fasterBullets, AbilityType.instantCast, 5.0f);
    public Ability ABILITY_Fleet_Of_Foot = new Ability(AbilityID.fleetOfFoot, AbilityType.passive, 5.0f);
    public Ability ABILITY_First_Aid = new Ability(AbilityID.firstAid, AbilityType.targetAllyCast, 5.0f, new string[] { "FirstAidEffectPrefab" });

    private void castNecromancerConversionRitual(GameObject caster, GameObject targetEnemy)
    {
        //Debug.Log("Casting Conversion Ritual");
        //Debug.Log("Casting Conversion Ritual targeting " + targetEnemy.GetComponent<Enemy_AI_script>().name);
        targetEnemy.GetComponent<Enemy_AI_script>().setBeingConverted(true);
        GameObject.FindGameObjectWithTag("Necromancer Ritual Button").GetComponent<Button>().interactable = false;
    }

    private void castMolotov(GameObject caster, int abilityIndex, GameObject targetGridSpace)
    {
        //Debug.Log("Throwing Molotov");
        GameObject molotov = Instantiate((GameObject)ABILITY_Throw_Molotov.getExtra("MolotovEffectPrefab"), targetGridSpace.transform.position, targetGridSpace.transform.rotation);
        molotov.GetComponent<Molotov_Effect_Script>().mySpace = targetGridSpace;
        caster.GetComponent<Minion_AI_Script>().setAbilityCooldown(abilityIndex, ABILITY_Throw_Molotov.cooldown);
    }

    private void castSprayAndPray(GameObject caster, int abilityIndex)
    {
        //Debug.Log("Casting Spray and Pray");
        caster.GetComponent<Minion_AI_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().sprayAndPrayBuff);
        caster.GetComponent<Minion_AI_Script>().setAbilityCooldown(abilityIndex, ABILITY_SprayAndPray.cooldown);
    }

    private void castFasterBullets(GameObject caster, int abilityIndex)
    {
        //Debug.Log("Casting Faster Bullets");
        caster.GetComponent<Minion_AI_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().fasterBulletsBuff);
        caster.GetComponent<Minion_AI_Script>().setAbilityCooldown(abilityIndex, ABILITY_FasterBullets.cooldown);
    }

    private void castFleetOfFoot(GameObject caster, int abilityIndex)
    {
        //Debug.Log("Casting Fleet of Foot");
        caster.GetComponent<Minion_AI_Script>().applyBuff(this.gameObject.GetComponent<Buff_Database_Script>().fleetOfFootBuff);
        //don't set cooldown because this is a passive
    }

    private void castFirstAid(GameObject caster, int abilityIndex, GameObject targetAlly)
    {
        //Debug.Log("Casting First Aid");
        targetAlly.GetComponent<Minion_AI_Script>().currentHp = targetAlly.GetComponent<Minion_AI_Script>().MaxHp;
        GameObject FirstAidEffect = Instantiate((GameObject)ABILITY_First_Aid.getExtra("FirstAidEffectPrefab"), targetAlly.transform.position, targetAlly.transform.rotation);
        caster.GetComponent<Minion_AI_Script>().setAbilityCooldown(abilityIndex, ABILITY_First_Aid.cooldown);
    }
}
