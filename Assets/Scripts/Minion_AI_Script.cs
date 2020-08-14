using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AbilityID = Ability_Database_Script.AbilityID;
using Ability_Database = Ability_Database_Script;
using Weapon = Weapon_Database_Script.Weapon;
using WeaponID = Weapon_Database_Script.WeaponID;
using Weapon_Database = Weapon_Database_Script;
using Buffs = Buff_Database_Script;


public class Minion_AI_Script : MonoBehaviour, MouseDownOverrider
{
    public string name = "Unnamed Minion";
    public string minionID;
    public int cost;

    [Header("Movement")]
    public GameObject mySpace;
    private GameObject targetSpace;
    public float baseMoveSpeed = 1.0f;
    private float moveSpeed;
    public bool isFacingRight = true;

    private Animator rigAnimator;

    private bool isMoving;


    [Header("Attack")]
    public WeaponID weapon1 = WeaponID.custom;
    public WeaponID weapon2 = WeaponID.custom;
    private WeaponID currentWeapon;
    private const string weaponSprite_Path = "Skeleton Rig/bone_1/bone_16/bone_17/bone_18/bone_19/Weapon Sprite";
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

    [Header("Abilities")]
    public AbilityID Ability1 = AbilityID.none;
    private float ability1CurrentCooldown;
    public AbilityID Ability2 = AbilityID.none;
    private float ability2CurrentCooldown;
    public AbilityID Ability3 = AbilityID.none;
    private float ability3CurrentCooldown;

    [Header("Buffs")]
    public List<Buffs.Buff> activeBuffs;

    //Sprite SortingLayer Orderings
    private List<int> baseSpriteSortingLayerOrderings;
    

    // Start is called before the first frame update
    void Start()
    {
        if(this.mySpace == null) { Debug.LogError(this.gameObject.name + " does not have it's MySpace assigned. Please assign it."); }

        targetSpace = this.mySpace;
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        currentHp = MaxHp;
        isMoving = false;
        isDying = false;
        activeBuffs = new List<Buffs.Buff>();
        currentWeapon = this.weapon1;

        //Store the original order in sortinglayer for each part of the minion's rig.
        baseSpriteSortingLayerOrderings = new List<int>();
        foreach(SpriteRenderer aRender in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            baseSpriteSortingLayerOrderings.Add(aRender.sortingOrder);
        }
        updateSpriteSortingLayers();


        triggerPassiveAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDying)
        {
            moveSpeed = baseMoveSpeed; //reset movespeed to base before applying buffs etc.
            loadStatsForCurrentlySelectedWeapon();
            applyBuffEffectsToStats();
            tickBuffDurations();

            movementLogic();

            rigAnimator.SetBool("IsWalking", isMoving);

            if (isMoving)
            {
                //Set sorting layer order to render based on y position, so minion higher on the grid render behind lower minions
                updateSpriteSortingLayers();
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
        if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.SelectOrMove)
        {
            User_Input_Script.setCurrentlySelectedMinion(this.gameObject);
        }
        else if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.CastAbilityOnAlly)
        {
            Ability_Database.cast(User_Input_Script.currentAbilityToCast, User_Input_Script.currentAbilityIndex, User_Input_Script.currentlySelectedMinion, this.gameObject);
            User_Input_Script.currentMouseCommand = User_Input_Script.MouseCommand.SelectOrMove;
            //Circle does not disappear because it goes back to the ability caster, who s the currently selected minion
        }
    }

    private void movementLogic()
    {
        Vector3 myPos = this.transform.position;
        Vector3 mySpacePos = mySpace.transform.position;
        mySpacePos.z = this.transform.position.z; //ignore the z dimension

        if (myPos != mySpacePos) //if not at mySpace, move to it
        {
            isMoving = true;
            Vector3 directionVector = (mySpacePos - this.transform.position);
            directionVector.z = 0;
            Vector3 moveVector = directionVector.normalized * (moveSpeed * Time.deltaTime);
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

    //Set sorting layer order to render based on y position, so minion higher on the grid render behind lower minions
    private void updateSpriteSortingLayers()
    {
        int i = 0;
        foreach(SpriteRenderer aRender in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            //Set this spriteRenderer's order in the sorting layer to its original order(so it renders correctly relevant to other parts of the minion rig) - an offset based on the minion's sprite position
            aRender.sortingOrder = baseSpriteSortingLayerOrderings[i] - Mathf.FloorToInt(100 * this.gameObject.transform.position.y);
            i++;
        }
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
            //if(areEnemiesAlongVector(this.transform.position, vectorToTargetSpace, spaceAimedAt.tag)  && areEnemiesAlongScatterVectors(this.transform.position, spaceAimedAt))
            if (areEnemiesAlongAnyAimVectors(this.transform.position, spaceAimedAt))
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
                    proj.GetComponent<Projectile_Logic_Script>().setScatterDirection(Projectile_Logic_Script.ScatterDirection.Both);
                    proj.GetComponent<Projectile_Logic_Script>().setScatterPattern(this.attackRange);
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

    private bool areEnemiesAlongVector(Vector3 source, Vector3 vectorToTargetSpace, string targetSpaceTag, int gridYOffset = 0)
    {
        //Debug.DrawRay(source, vectorToTargetSpace, Color.red, 1);
        float rayLength;
        if (string.Equals(targetSpaceTag, "End Space") && this.isMeleeAttack) //if targeting an end space with a melee attack, do not use the distance to the space as the length of the raycast
        {
            rayLength = this.attackRange.Length * 3; //TODO: find a more elegant off the board range that just 3 co-ordinate units per gridspace of attackrange.
        }
        else //if a ranged attack, or melee attack targeting a normal space, use the distance to the space as the length of the raycast
        {
            rayLength = vectorToTargetSpace.magnitude;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(source, vectorToTargetSpace, rayLength, LayerMask.GetMask("Enemies"));
        foreach (RaycastHit2D aHit in hits)
        {
            //Detect if object spotted is an Enemy
            if (aHit.collider.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy hit " + aHit.collider.gameObject.ToString());
                //Detect if enemy hit is on the correct row of the grid
                float hitEnemyGridY = aHit.collider.gameObject.GetComponent<Enemy_AI_script>().nextSpace.GetComponent<Space_Script>().gridPosition.y;
                if (hitEnemyGridY == this.mySpace.GetComponent<Space_Script>().gridPosition.y + gridYOffset)
                {
                    //Debug.Log("Enemy hit is on right y ");
                    return true;
                }
            }
        }
        return false;
    }

    private bool areEnemiesAlongAnyAimVectors(Vector3 source, GameObject directTargetSpace)
    {
        Vector2 directTargetGridPos = directTargetSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 directSourceGridPos = this.mySpace.GetComponent<Space_Script>().gridPosition;

        Vector3 vectorToTargetSpace = (directTargetSpace.transform.position - source);


        //If enemies are along direct shot vector, return true
        if (areEnemiesAlongVector(source, vectorToTargetSpace, directTargetSpace.tag))
        {
            return true;
        }

        //If enemies are along scatter vectors, return true....note, there are some redundent checks going on here.
        for (int x = 0; x < this.attackRange.Length; x++)
        {
            for(int y = 1; y <= this.attackRange[x]; y++) //y = 1 because if attackRange[x] = 0, skip this calculation as weapon has no scatter for this x.
            {
                //Calculate x offset to grid position
                int scatterSourceGridPosX = (int)directSourceGridPos.x;
                int scatterTargetGridPosX = (int)directTargetGridPos.x;
                if(this.isFacingRight)
                {
                    scatterSourceGridPosX += x;
                    
                }
                else
                {
                    scatterSourceGridPosX -= x;
                }

                //Get the source and target gridSpaces to scatter to.
                GameObject scatterDownTargetSpace = Space_Script.findGridSpace((int)directTargetGridPos.x, (int)directTargetGridPos.y + y, true);
                GameObject scatterDownSourceSpace = Space_Script.findGridSpace((int)scatterSourceGridPosX, (int)directSourceGridPos.y + y, true); 
                GameObject scatterUpTargetSpace = Space_Script.findGridSpace((int)directTargetGridPos.x, (int)directTargetGridPos.y - y, true);
                GameObject scatterUpSourceSpace = Space_Script.findGridSpace((int)scatterSourceGridPosX, (int)directSourceGridPos.y - y, true); 

                if(scatterDownSourceSpace != null && scatterDownTargetSpace != null)
                {
                    Vector3 vectorToScatterDownTargetSpace = (scatterDownTargetSpace.transform.position - scatterDownSourceSpace.transform.position);
                    Debug.DrawRay(scatterDownSourceSpace.transform.position, vectorToScatterDownTargetSpace, Color.blue, 1);
                    if (areEnemiesAlongVector(scatterDownSourceSpace.transform.position, vectorToScatterDownTargetSpace, scatterDownTargetSpace.tag, y))
                    {
                        return true;
                    }
                }

                if (scatterUpSourceSpace != null && scatterUpTargetSpace != null)
                {
                    Vector3 vectorToScatterUpTargetSpace = (scatterUpTargetSpace.transform.position - scatterUpSourceSpace.transform.position);
                    Debug.DrawRay(scatterUpSourceSpace.transform.position, vectorToScatterUpTargetSpace, Color.blue, 1);
                    if (areEnemiesAlongVector(scatterUpSourceSpace.transform.position, vectorToScatterUpTargetSpace, scatterUpTargetSpace.tag, -y))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //Catches the melee attack animation event to apply the melee attack effects.
    public void onMeleeAttackAnimationFinished()
    {
        //Calculate where we're aiming
        GameObject directTargetSpace = calculateAttacksTargetSpace();
        Vector3 source = this.mySpace.transform.position;

        Vector2 directTargetGridPos = directTargetSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 directSourceGridPos = this.mySpace.GetComponent<Space_Script>().gridPosition;

        Vector3 vectorToTargetSpace = (directTargetSpace.transform.position - source);


        //If enemies are along direct shot vector, damage them
        damageEnemiesAlongVector(source, vectorToTargetSpace, directTargetSpace.tag); 


        //If enemies are along scatter vectors, damage them
        for (int x = 0; x < this.attackRange.Length; x++)
        {
            for (int y = 1; y <= this.attackRange[x]; y++) //y = 1 because if attackRange[x] = 0, skip this calculation as weapon has no scatter for this x.
            {
                //Calculate x offset to grid position
                int scatterSourceGridPosX = (int)directSourceGridPos.x;
                int scatterTargetGridPosX = (int)directTargetGridPos.x;
                if (this.isFacingRight)
                {
                    scatterSourceGridPosX += x;

                }
                else
                {
                    scatterSourceGridPosX -= x;
                }

                //Get the source and target gridSpaces to scatter to.
                GameObject scatterDownTargetSpace = Space_Script.findGridSpace((int)directTargetGridPos.x, (int)directTargetGridPos.y + y, true);
                GameObject scatterDownSourceSpace = Space_Script.findGridSpace((int)scatterSourceGridPosX, (int)directSourceGridPos.y + y, true);
                GameObject scatterUpTargetSpace = Space_Script.findGridSpace((int)directTargetGridPos.x, (int)directTargetGridPos.y - y, true);
                GameObject scatterUpSourceSpace = Space_Script.findGridSpace((int)scatterSourceGridPosX, (int)directSourceGridPos.y - y, true);

                if (scatterDownSourceSpace != null && scatterDownTargetSpace != null)
                {
                    Vector3 vectorToScatterDownTargetSpace = (scatterDownTargetSpace.transform.position - scatterDownSourceSpace.transform.position);
                    Debug.DrawRay(scatterDownSourceSpace.transform.position, vectorToScatterDownTargetSpace, Color.blue, 1);
                    damageEnemiesAlongVector(scatterDownSourceSpace.transform.position, vectorToScatterDownTargetSpace, scatterDownTargetSpace.tag, y);
                }

                if (scatterUpSourceSpace != null && scatterUpTargetSpace != null)
                {
                    Vector3 vectorToScatterUpTargetSpace = (scatterUpTargetSpace.transform.position - scatterUpSourceSpace.transform.position);
                    Debug.DrawRay(scatterUpSourceSpace.transform.position, vectorToScatterUpTargetSpace, Color.blue, 1);
                    damageEnemiesAlongVector(scatterUpSourceSpace.transform.position, vectorToScatterUpTargetSpace, scatterUpTargetSpace.tag, -y);
                }
            }
        }
    }

    //Inflict melee damage on all enemies along the vector from source to targetSpace
    private void damageEnemiesAlongVector(Vector3 source, Vector3 vectorToTargetSpace, string targetSpaceTag, int gridYOffset = 0)
    {
        Debug.DrawRay(source, vectorToTargetSpace, Color.green, 1);
        float rayLength;
        if (string.Equals(targetSpaceTag, "End Space") && this.isMeleeAttack) //if targeting an end space with a melee attack, do not use the distance to the space as the length of the raycast
        {
            rayLength = this.attackRange.Length * 3; //TODO: find a more elegant off the board range that just 3 co-ordinate units per gridspace of attackrange.
        }
        else //if a ranged attack, or melee attack targeting a normal space, use the distance to the space as the length of the raycast
        {
            rayLength = vectorToTargetSpace.magnitude;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(source, vectorToTargetSpace, rayLength, LayerMask.GetMask("Enemies"));
        foreach (RaycastHit2D aHit in hits)
        {
            //Detect if object spotted is an Enemy
            if (aHit.collider.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy hit " + aHit.collider.gameObject.ToString());
                //Detect if enemy hit is on the correct row of the grid
                float hitEnemyGridY = aHit.collider.gameObject.GetComponent<Enemy_AI_script>().nextSpace.GetComponent<Space_Script>().gridPosition.y;
                if (hitEnemyGridY == this.mySpace.GetComponent<Space_Script>().gridPosition.y + gridYOffset)
                {
                    //Debug.Log("Enemy hit is on right y ");
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
            if((aminion != this.gameObject) && aminion.GetComponent<Minion_AI_Script>().targetSpace == spaceIn)
            {
                return false;
            }
        }

        //Check if spaceIn is already the target space of the necromancer
        GameObject aNecromancer = GameObject.FindGameObjectWithTag("Necromancer");
        if ((aNecromancer != this.gameObject) && aNecromancer.GetComponent<Minion_AI_Script>().targetSpace == spaceIn)
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
            Player_Inventory_Script.addPlayersDarkEnergy(-inDamage);
            if (Player_Inventory_Script.getPlayersDarkEnergy() <= 0)
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
        if (currentWeapon != WeaponID.custom)
        {
            switchAttackStatsToWeapon(Weapon_Database.findWeapon(currentWeapon));
            switchWeaponVisualsToWeapon(Weapon_Database.findWeapon(currentWeapon));
        }
    }

    private void switchAttackStatsToWeapon(Weapon weaponIn)
    {
        this.isMeleeAttack = weaponIn.isMeleeWeapon;
        this.meleeDamage = weaponIn.meleeWeaponDamage;
        this.projectile = weaponIn.weaponProjectile;
        this.projectile_Damage = weaponIn.projectile_Damage;
        this.projectile_Speed = weaponIn.projectile_Speed;
        this.attackCooldown = weaponIn.weaponAttackCooldown;
        this.attackRange = weaponIn.weaponRange;
    }

    private void switchWeaponVisualsToWeapon(Weapon weaponIn)
    {
        GameObject weaponSprite = this.transform.Find(weaponSprite_Path).gameObject;
        if (weaponIn.weaponSprite == null)
        {
            weaponSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            weaponSprite.GetComponent<SpriteRenderer>().enabled = true;
            weaponSprite.GetComponent<SpriteRenderer>().sprite = weaponIn.weaponSprite;
            weaponSprite.transform.localPosition = weaponIn.weaponOffset;
            weaponSprite.transform.localEulerAngles = weaponIn.weaponRotation;
        }
    }

    public void switchWeapons()
    {
        if(currentWeapon == weapon1)
        {
            currentWeapon = weapon2;
        }
        else
        {
            currentWeapon = weapon1;
        }
    }

    public Sprite getCurrentWeaponIcon()
    {
        return Weapon_Database.findWeapon(currentWeapon).icon;
    }

    public Sprite getOtherWeaponIcon()
    {
        if (currentWeapon == weapon1)
        {
            return Weapon_Database.findWeapon(weapon2).icon;
        }
        else
        {
            return Weapon_Database.findWeapon(weapon1).icon;
        }
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

    public AbilityID getAbilityIDforSlot(int i)
    {
        switch (i)
        {
            case 1: return Ability1;
            case 2: return Ability2;
            case 3: return Ability3;
            default: return AbilityID.none;
        }
    }

    public void triggerPassiveAbilities()
    {
        try
        {

            if (Ability_Database.getAbilityType(getAbilityIDforSlot(1)) == Ability_Database.AbilityType.passive)
            {
                Ability_Database.cast(getAbilityIDforSlot(1), 1, this.gameObject, null);
            }
            if (Ability_Database.getAbilityType(getAbilityIDforSlot(2)) == Ability_Database.AbilityType.passive)
            {
                Ability_Database.cast(getAbilityIDforSlot(2), 2, this.gameObject, null);
            }
            if (Ability_Database.getAbilityType(getAbilityIDforSlot(3)) == Ability_Database.AbilityType.passive)
            {
                Ability_Database.cast(getAbilityIDforSlot(3), 3, this.gameObject, null);
            }
        }
        catch(NullReferenceException e)
        {
            Debug.LogWarning("TriggerPassiveAbilities encountered a NullReferenceException, retrying TriggerPassiveAbilities(), this may be caused by Database instances being slow to assign, if this message persists past startup check your passive logic.");
            Invoke("triggerPassiveAbilities", 0.1f);
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
            applyBuffToStat(ref this.moveSpeed, aBuff.moveSpeed, aBuff.moveSpeedOperator);
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
            if(aBuff.duration >= 0) //buff duration of -1 are infinate
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

    //Find Nearest Minion
    public static GameObject findNearestMinion(Vector3 position)
    {
        Vector3 closestVector = new Vector3(-999999999999999999, -999999999999999999, 0);
        GameObject closestSpace = null;
        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Minion"))
        {
            Vector3 spacePosIgnoreZ = new Vector3(aSpace.transform.position.x, aSpace.transform.position.y, position.z);
            if (Vector3.Distance(position, spacePosIgnoreZ) < Vector3.Distance(position, closestVector))
            {
                closestVector = spacePosIgnoreZ;
                closestSpace = aSpace;
            }
        }
        return closestSpace;
    }

    //returns the nearest grid space, unless the closest grid space is further away than range, then returns null
    public static GameObject findNearestMinionWithinRange(Vector3 position, float range)
    {
        GameObject closestSpace = findNearestMinion(position);
        Vector3 spacePosIgnoreZ = new Vector3(closestSpace.transform.position.x, closestSpace.transform.position.y, position.z);
        if (Vector3.Distance(position, spacePosIgnoreZ) < range)
        {
            return closestSpace;
        }
        return null;
    }

    //Death
    private void die()
    {
        rigAnimator.SetTrigger("DoDie");
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        this.isDying = true;
        //Flag this minion's entry in the roster as not summoned, so that it's summoning button is re-enable and it may be resummoned.
        GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<Minion_Roster_Script>().flagMinionAsSummoned(this.minionID, false);
    }

    public void onDeathAnimationFinished()
    {
        Destroy(this.gameObject);
    }

}
