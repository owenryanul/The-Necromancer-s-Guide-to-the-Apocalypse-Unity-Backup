using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AbilityID = Ability_Database_Script.AbilityID;
using WeaponID = Weapon_Database_Script.WeaponID;
using Buffs = Buff_Database_Script;


public class Minion_Movement_Script : MonoBehaviour, MouseDownOverrider 
{
    public string name = "Unnamed Minion";

    [Header("Movement")]
    public GameObject mySpace;
    private GameObject targetSpace;
    public float speed = 1.0f;
    public bool isFacingRight = true;

    private Animator rigAnimator;

    private bool isMoving;


    [Header("Attack")]
    public WeaponID weapon1 = WeaponID.custom;
    public WeaponID weapon2 = WeaponID.custom;
    public bool isMeleeAttack;
    public int meleeDamage;
    public GameObject projectile;
    public int projectile_Damage;
    public float projectile_Speed;
    public float attackCooldown;
    private float currentAttackCooldown;
    [Tooltip("AttackRange indicates weapon spread. AttackRange.Length determines how many gridSpaces horizontally the attack reaches. While each element determines how many gridspaces the attack spread vertically from the corripoding grid space. E.g. (0,1,2) would reach create a cone that reaches 3 spaces across, has no spread from the 1st space, spreads 1 space above AND below from the 2nd space across and spreads to the 2 spaces above AND below the 3rd space across.")]
    public int[] attackRange;
    private const float yDistanceToBeConsideredInTheSameRow = 0.2f;


    [Header("Defence")]
    public int MaxHp = 1;
    public int currentHp;

    private bool isDying;

    private Weapon_Database_Script WeaponDatabase;

    [Header("Abilities")]
    public AbilityID Ability1 = AbilityID.none;
    private float ability1CurrentCooldown;
    public AbilityID Ability2 = AbilityID.none;
    private float ability2CurrentCooldown;
    public AbilityID Ability3 = AbilityID.none;
    private float ability3CurrentCooldown;

    [Header("Buffs")]
    public List<Buffs.Buff> activeBuffs;

    // Start is called before the first frame update
    void Start()
    {
        targetSpace = mySpace;
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        currentHp = MaxHp;
        isMoving = false;
        isDying = false;
        activeBuffs = new List<Buffs.Buff>();

        WeaponDatabase = GameObject.FindGameObjectWithTag("Level Script Container").GetComponent<Weapon_Database_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDying)
        {
            loadStatsForCurrentlySelectedWeapon();
            applyBuffEffectsToStats();
            tickBuffDurations();

            Vector3 myPos = this.transform.position;
            Vector3 mySpacePos = mySpace.transform.position;
            mySpacePos.z = this.transform.position.z; //ignore the z dimension

            if (myPos != mySpacePos) //if not at mySpace, move to it
            {
                isMoving = true;
                Vector3 directionVector = (mySpacePos - this.transform.position);
                directionVector.z = 0;
                Vector3 moveVector = directionVector.normalized * (speed * Time.deltaTime);
                if (moveVector.magnitude > directionVector.magnitude)
                {
                    this.transform.position = mySpacePos;
                }
                else
                {
                    this.transform.position += moveVector;
                }
            }
            else if (mySpace != targetSpace) //if at mySpace check if its your target space, if not change mySpace
            {
                mySpace = findNextSpace();
            }
            else //if at mySpace and it is your target space, stop moving
            {
                isMoving = false;
            }

            rigAnimator.SetBool("IsWalking", isMoving);

            if (isMoving)
            {
                //TODO: Set sorting layer order to render based on y position
            }

            //Attack
            if (!isMoving)
            {
                attackLogic();
            }

            //Abilty Cooldowns
            if(ability1CurrentCooldown > 0)
            {
                ability1CurrentCooldown -= Time.deltaTime;
            }
            if (ability2CurrentCooldown > 0)
            {
                ability2CurrentCooldown -= Time.deltaTime;
            }
            if (ability3CurrentCooldown > 0)
            {
                ability3CurrentCooldown -= Time.deltaTime;
            }

        }
    }

    //If minion clicked, set them as the currently selected minion
    public void OnMouseDownOverride()
    {
        Debug.Log("Minion Clicked");
        User_Input_Script.setCurrentlySelectedMinion(this.gameObject);      
    }


    //Find the next space on the path between the currently occupied space and the target space.
    //Note: Will attempt to match target spaces Y before matching X
    private GameObject findNextSpace()
    {
        Vector2 myGridPos = mySpace.GetComponent<Space_Script>().gridPosition;
        Vector2 targetGridPos = targetSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 nextGridPos = myGridPos;

        if (myGridPos.y < targetGridPos.y)
        {
            nextGridPos.y++;
        }
        else if (myGridPos.y > targetGridPos.y)
        {
            nextGridPos.y--;
        }
        else if (myGridPos.x < targetGridPos.x)
        {
            nextGridPos.x++;
        }
        else if (myGridPos.x > targetGridPos.x)
        {
            nextGridPos.x--;
        }

        foreach(GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if(aSpace.GetComponent<Space_Script>().gridPosition == nextGridPos)
            {
                return aSpace;
            }
        }

        Debug.LogError("WARNING: Space could not be found, check your space's gridPositions as no space could be found with co-ordinates: " + nextGridPos + " on the path to " + targetGridPos);
        return null;
    }

    private void attackLogic()
    {
        currentAttackCooldown += Time.deltaTime;
        if (currentAttackCooldown >= attackCooldown) //if attack off cooldown
        {
            currentAttackCooldown = 0;

            //Calculate where we're aiming
            GameObject spaceAimedAt = calculateAttacksTargetSpace();

            //Detect enemies along aim
            Vector3 vectorToTargetSpace = (spaceAimedAt.transform.position - this.transform.position);
            if(areEnemiesAlongVector(vectorToTargetSpace, spaceAimedAt.tag))
            {
                //Make the attack
                if (this.isMeleeAttack)
                {
                    rigAnimator.SetTrigger("DoMeleeAttack");
                    //continue the logic from the animation event in onMeleeAnimationFinished();
                }
                else
                {
                    rigAnimator.SetTrigger("DoRangedAttack");
                    GameObject proj = Instantiate(projectile, this.transform.position, this.transform.rotation);
                    proj.GetComponent<Projectile_Logic_Script>().mySpace = this.mySpace;
                    proj.GetComponent<Projectile_Logic_Script>().setTargetSpace(spaceAimedAt);
                    proj.GetComponent<Projectile_Logic_Script>().projectileDamage = this.projectile_Damage;
                    proj.GetComponent<Projectile_Logic_Script>().speed = this.projectile_Speed;
                }
            }
        }
    }

    private GameObject calculateAttacksTargetSpace()
    {
        Vector2 range = new Vector2(attackRange.Length, 0);
        if (!isFacingRight)
        {
                range.x *= -1;
        }
        Vector2 aimingAtGridPos = this.targetSpace.GetComponent<Space_Script>().gridPosition + range;


        GameObject spaceAimedAt = Space_Script.findGridSpace(aimingAtGridPos);
        //If spaceAimedAt does not exist, aim at the appriorate endSpace instead. (End Spaces are grid spaces placed off screen for targeting purposes)
        if(spaceAimedAt == null)
        {
            foreach(GameObject anEndSpace in GameObject.FindGameObjectsWithTag("End Space"))
            {
                Vector2 mySpacePos = mySpace.GetComponent<Space_Script>().gridPosition;
                Vector2 anEndSpacePos = anEndSpace.GetComponent<Space_Script>().gridPosition;
                if (anEndSpacePos.y == mySpacePos.y)
                {
                    if((isFacingRight && mySpacePos.x < anEndSpacePos.x) || (!isFacingRight && mySpacePos.x > anEndSpacePos.x))
                    {
                        spaceAimedAt = anEndSpace;
                        return spaceAimedAt;
                    }
                }
            }
        }
        return spaceAimedAt;
    }

    private bool areEnemiesAlongVector(Vector3 vectorToTargetSpace, string targetSpaceTag)
    {
        Debug.DrawRay(this.transform.position, vectorToTargetSpace, Color.red, 1);
        float rayLength;
        if (string.Equals(targetSpaceTag, "End Space") && this.isMeleeAttack) //if targeting an end space with a melee attack, do not use the distance to the space as the length of the raycast
        {
            rayLength = this.attackRange.Length * 3; //TODO: find a more elegant off the board range that just 3 co-ordinate units per gridspace of attackrange.
        }
        else //if a ranged attack, or melee attack targeting a normal space, use the distance to the space as the length of the raycast
        {
            rayLength = vectorToTargetSpace.magnitude;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, vectorToTargetSpace, rayLength, LayerMask.GetMask("Enemies"));
        foreach (RaycastHit2D aHit in hits)
        {
            //Detect if object spotted is an Enemy
            if (aHit.collider.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy hit " + aHit.collider.gameObject.ToString());
                //Detect if enemy hit is on the right grid-row
                float hitEnemyGridY = aHit.collider.gameObject.GetComponent<Enemy_AI_script>().nextSpace.GetComponent<Space_Script>().gridPosition.y;
                if (hitEnemyGridY == this.mySpace.GetComponent<Space_Script>().gridPosition.y)
                {
                    //Debug.Log("Enemy hit is on right y ");
                    return true;
                }
            }
        }
        return false;
    }

    //Catches the melee attack animation event to apply the melee attack effects.
    public void onMeleeAttackAnimationFinished()
    {
        //Calculate where we're aiming
        GameObject spaceAimedAt = calculateAttacksTargetSpace();

        //Detect enemies along aim
        Vector3 vectorToTargetSpace = (spaceAimedAt.transform.position - this.transform.position);
        Debug.DrawRay(this.transform.position, vectorToTargetSpace, Color.red, 1);
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, vectorToTargetSpace, vectorToTargetSpace.magnitude, LayerMask.GetMask("Enemies"));
        foreach (RaycastHit2D aHit in hits)
        {
            //Detect if object spotted is an Enemy
            if (aHit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit " + aHit.collider.gameObject.ToString());
                //Detect if enemy hit is on the right grid-row
                float hitEnemyGridY = aHit.collider.gameObject.GetComponent<Enemy_AI_script>().nextSpace.GetComponent<Space_Script>().gridPosition.y;
                if (hitEnemyGridY == this.mySpace.GetComponent<Space_Script>().gridPosition.y)
                {
                    Debug.Log("Enemy hit is on right y ");
                    aHit.collider.gameObject.GetComponent<Enemy_AI_script>().onHitByMelee(this.meleeDamage);
                }
            }
        }
    }

    public bool setTargetSpace(GameObject spaceIn)
    {
        //Check if spaceIn is already the target space of another minion
        foreach(GameObject aminion in GameObject.FindGameObjectsWithTag("Minion"))
        {
            if((aminion != this.gameObject) && aminion.GetComponent<Minion_Movement_Script>().targetSpace == spaceIn)
            {
                return false;
            }
        }

        //Check if spaceIn is already the target space of the necromancer
        GameObject aNecromancer = GameObject.FindGameObjectWithTag("Necromancer");
        if ((aNecromancer != this.gameObject) && aNecromancer.GetComponent<Minion_Movement_Script>().targetSpace == spaceIn)
        {
            return false;
        }

        targetSpace = spaceIn;
        return true;
    }

    public GameObject getTargetSpace()
    {
        return this.targetSpace;
    }

    public void onHitByAttack(int inDamage)
    {
        if (this.gameObject.CompareTag("Necromancer"))
        {
            Dark_Energy_Meter_Script.addDarkEnergy(-inDamage);
            if (Dark_Energy_Meter_Script.getDarkEnergy() <= 0)
            {
                die();
            }
        }
        else
        {
            this.currentHp -= inDamage;
            if (this.currentHp <= 0)
            {
                die();
            }
        }
    }

    public void flipFacing()
    {
        if (this.isFacingRight)
        {
            this.isFacingRight = false;
        }
        else if (!this.isFacingRight)
        {
            this.isFacingRight = true;
        }
        Vector3 facing = this.transform.GetChild(0).transform.localScale;
        facing.x *= -1;
        this.transform.GetChild(0).transform.localScale = facing;
    }

    private void loadStatsForCurrentlySelectedWeapon()
    {
        WeaponID currentWeapon = weapon1;
        Debug.Log("Weapon1 = " + currentWeapon);
        if (currentWeapon != WeaponID.custom)
        {
            switchAttackStatsToWeapon(WeaponDatabase.findWeapon(weapon1));
        }
    }

    private void switchAttackStatsToWeapon(Weapon_Database_Script.Weapon weaponIn)
    {
        this.isMeleeAttack = weaponIn.isMeleeWeapon;
        this.meleeDamage = weaponIn.meleeWeaponDamage;
        this.projectile = weaponIn.weaponProjectile;
        this.projectile_Damage = weaponIn.projectile_Damage;
        this.projectile_Speed = weaponIn.projectile_Speed;
        this.attackCooldown = weaponIn.weaponAttackCooldown;
        this.attackRange = weaponIn.weaponRange;
    }

    public void switchWeapons()
    {
        WeaponID temp = weapon1;
        weapon1 = weapon2;
        weapon2 = temp;
    }

    //Abilities
    public float getAbilityCooldown(int i)
    {
        switch (i)
        {
            case 1: return ability1CurrentCooldown;
            case 2: return ability2CurrentCooldown;
            case 3: return ability3CurrentCooldown;
            default: return 0.0f;
        }
    }

    public void setAbilityCooldown(int i, float cooldownMax)
    {
        switch (i)
        {
            case 1: ability1CurrentCooldown = cooldownMax; break;
            case 2: ability2CurrentCooldown = cooldownMax; break;
            case 3: ability3CurrentCooldown = cooldownMax; break;
        }
    }

    public AbilityID getAbility(int i)
    {
        switch (i)
        {
            case 1: return Ability1;
            case 2: return Ability2;
            case 3: return Ability3;
            default: return AbilityID.none;
        }
    }

    //Buffs
    public void applyBuff(Buffs.Buff aBuff)
    {
        activeBuffs.Add(aBuff);
        activeBuffs[activeBuffs.Count - 1].durationLeft = activeBuffs[activeBuffs.Count - 1].duration;
    }

    private void applyBuffEffectsToStats()
    {
        foreach(Buffs.Buff aBuff in activeBuffs)
        {
            applyBuffToStat(ref this.attackCooldown, aBuff.weaponAttackCooldown, aBuff.weaponAttackCooldownOperator);
            applyBuffToStat(ref this.meleeDamage, aBuff.meleeWeaponDamage, aBuff.meleeWeaponDamageOperator);
            applyBuffToStat(ref this.projectile_Speed, aBuff.projectileSpeed, aBuff.projectileSpeedOperator);
        }
    }


    private void applyBuffToStat(ref float stat, float buffStat, Buffs.BuffOperator buffOperator)
    {
        switch (buffOperator)
        {
            case Buffs.BuffOperator.set: stat = buffStat; break;
            case Buffs.BuffOperator.add: stat += buffStat; break;
            case Buffs.BuffOperator.subtract: stat -= buffStat; break;
            case Buffs.BuffOperator.multipleBy: stat *= buffStat; break;
            case Buffs.BuffOperator.divideBy: stat /= buffStat; break;
            case Buffs.BuffOperator.unused: break;
        }
    }

    private void applyBuffToStat(ref int stat, int buffStat, Buffs.BuffOperator buffOperator)
    {
        switch (buffOperator)
        {
            case Buffs.BuffOperator.set: stat = buffStat; break;
            case Buffs.BuffOperator.add: stat += buffStat; break;
            case Buffs.BuffOperator.subtract: stat -= buffStat; break;
            case Buffs.BuffOperator.multipleBy: stat *= buffStat; break;
            case Buffs.BuffOperator.divideBy: stat /= buffStat; break;
            case Buffs.BuffOperator.unused: break;
        }
    }

    private void applyBuffToStat(ref bool stat, bool buffStat)
    {
        stat = buffStat;
    }

    private void tickBuffDurations()
    {
        List<Buffs.Buff> expiredBuffs = new List<Buffs.Buff>();
        foreach(Buffs.Buff aBuff in activeBuffs)
        {
            if(aBuff.duration >= 0)
            {
                aBuff.durationLeft -= Time.deltaTime;
                if (aBuff.durationLeft <= 0)
                {
                    expiredBuffs.Add(aBuff);
                }
            }
        }
        activeBuffs = activeBuffs.Except(expiredBuffs).ToList<Buffs.Buff>(); //removes expired buffs from the list of active buffs.
    }

    //Death
    private void die()
    {
        rigAnimator.SetTrigger("DoDie");
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        this.isDying = true;
    }

    public void onDeathAnimationFinished()
    {
        Destroy(this.gameObject);
    }

}
