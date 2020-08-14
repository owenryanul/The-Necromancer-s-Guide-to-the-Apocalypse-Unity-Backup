using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ability_Database = Ability_Database_Script;
using AbilityID = Ability_Database_Script.AbilityID;
using WeaponID = Weapon_Database_Script.WeaponID;

public class Enemy_AI_script : MonoBehaviour, MouseDownOverrider
{
    [Header("Movement")]
    public GameObject nextSpace;
    public float speed = 1.0f;
    protected GameObject targetSpace;
    protected bool isMoving;

    [Header("Attacks")]
    public float meleeRange = 1.0f;
    public int meleeDamage = 1;
    //protected GameObject minionToAttack;
    protected const float yDistanceToBeConsideredInTheSameRow = 0.1f;

    [Header("AI")]
    public GameObject tryingToKill;

    [Header("Defence")]
    public int maxHP = 1;
    public int currentHP;

    [Header("Animation")]
    protected Animator rigAnimator;
    protected bool isFacingRight;
    protected bool isDying;

    [Header("Visuals")]
    public GameObject hurtParticleEmitter;
    public GameObject ritualCompleteParticleEmitter;

    [Header("Conversion And Energy")]
    public int energyRewardOnKill = 1;
    public int conversionProgressRequired = 10;
    protected bool beingConverted;
    protected bool escapingBeingConverted;
    protected int conversionProgress;
    protected Vector3 conversionHoldingPoint;


    //Sprite SortingLayer Orderings
    protected List<int> baseSpriteSortingLayerOrderings;

    protected Minion_Roster_Script MinionRoster;
    

    // Start is called before the first frame update
    protected void Start()
    {
        tryingToKill = GameObject.FindGameObjectWithTag("Necromancer");
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        isMoving = false;
        isDying = false;
        beingConverted = false;
        escapingBeingConverted = false;
        conversionHoldingPoint = new Vector3(14, 10, 0);
        this.gameObject.GetComponent<ParticleSystem>().Stop();

        //Store the original order in sortinglayer for each part of the minion's rig.
        baseSpriteSortingLayerOrderings = new List<int>();
        foreach (SpriteRenderer aRender in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            baseSpriteSortingLayerOrderings.Add(aRender.sortingOrder);
        }
        updateSpriteSortingLayers();


        currentHP = maxHP;

        MinionRoster = GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<Minion_Roster_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDying)
        {
            return; //override update if playing the death animation
        }

        if (this.beingConverted)
        {
            conversionUpdate();
        }
        else if(this.escapingBeingConverted)
        {
            escapingConversionUpdate();
        }
        else
        {
            basicEnemyMovementAndAttacksUpdate();

            rigAnimator.SetBool("IsWalking", isMoving);
            if (isMoving)
            {
                updateSpriteSortingLayers();
            }
        }
    }

    //Attempt to find target space, then move to target space, stopping to attack anything in the way.
    //Also makes the sprite face the direction they're moving in.
    protected void basicEnemyMovementAndAttacksUpdate()
    {
        //Set target to the space currently occupied by the necromancer
        this.targetSpace = tryingToKill.GetComponent<Minion_AI_Script>().getTargetSpace();

        Vector3 myPos = this.transform.position;
        Vector3 nextSpacePos = nextSpace.transform.position;
        if (nextSpacePos.x > myPos.x) //if next space is left of this enemy
        {
            nextSpacePos.x -= meleeRange; //offset the target position by melee range
            flipSpriteRight();
        }
        else if (nextSpacePos.x <= myPos.x) //if next space is right of this enemy
        {
            nextSpacePos.x += meleeRange;
            flipSpriteLeft();
        }
        nextSpacePos.z = this.transform.position.z; //ignore the z dimension



        if (findMinionToAttack() != null) //if a valid target is in melee range, then attack
        {
            this.rigAnimator.SetTrigger("DoAttack");
        }
        else if (myPos != nextSpacePos) //otherwise, if not at nextSpace, move to it
        {
            isMoving = true;
            Vector3 directionVector = (nextSpacePos - this.transform.position);
            directionVector.z = 0;
            Vector3 moveVector = directionVector.normalized * (speed * Time.deltaTime);
            //if target position is close than 1 frame's worth of movement, snap to it, otherwise move towards it
            if (moveVector.magnitude > directionVector.magnitude)
            {
                this.transform.position = nextSpacePos;
            }
            else
            {
                this.transform.position += moveVector;
            }
        }
        else if (nextSpace != targetSpace) //if at nextSpace check if its your target space, if not change nextSpace
        {
            nextSpace = findNextSpace();
        }
        else //if at nextSpace and it is your target space, stop moving
        {
            isMoving = false;
        }
    }

    //Set sorting layer order to render based on y position, so minion higher on the grid render behind lower minions
    protected void updateSpriteSortingLayers()
    {
        int i = 0;
        foreach (SpriteRenderer aRender in this.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            //Set this spriteRenderer's order in the sorting layer to its original order(so it renders correctly relevant to other parts of the minion rig) - an offset based on the minion's sprite position
            aRender.sortingOrder = baseSpriteSortingLayerOrderings[i] - Mathf.FloorToInt(100 * this.gameObject.transform.position.y);
            i++;
        }
    }

    public void OnMouseDownOverride()
    {
        Debug.Log("Enemy Clicked");
        if (User_Input_Script.currentlySelectedMinion != null)
        {
            //If aiming an ability, cast the ability targeting this space
            if (User_Input_Script.currentMouseCommand == User_Input_Script.MouseCommand.CastAbilityOnEnemy)
            {
                Ability_Database.cast(User_Input_Script.currentAbilityToCast, User_Input_Script.currentAbilityIndex, User_Input_Script.currentlySelectedMinion, this.gameObject);
                User_Input_Script.currentMouseCommand = User_Input_Script.MouseCommand.SelectOrMove;
                //Circle does not disappear because it goes back to the ability caster, who s the currently selected minion
            }
        }
    }

    protected GameObject findMinionToAttack()
    {
        Vector3 vectorDirection;
        if(isFacingRight)
        {
            vectorDirection = Vector3.right;
        }
        else
        {
            vectorDirection = Vector3.left;
        }

        //Raycast to detect a minion or necromancer in melee range
        //Debug.DrawRay(this.transform.position, (vectorDirection * this.meleeRange), Color.red, 1);
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, vectorDirection, this.meleeRange, LayerMask.GetMask("Minions"));
        foreach(RaycastHit2D aHit in hits) 
        {
            if(aHit.collider.gameObject.CompareTag("Minion") || aHit.collider.gameObject.CompareTag("Necromancer"))
            {
                float yDistanceToHitMinion = Mathf.Abs(aHit.collider.gameObject.transform.position.y - this.gameObject.transform.position.y);
                if (yDistanceToHitMinion <= yDistanceToBeConsideredInTheSameRow)
                {
                    return aHit.collider.gameObject;
                    
                }
            }
        }
        return null;
    }


    //Find the next space on the path between the currently occupied space and the target space.
    //Note: Will attempt to match target spaces X before matching Y
    protected GameObject findNextSpace()
    {
        Vector2 myGridPos = nextSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 targetGridPos = targetSpace.GetComponent<Space_Script>().gridPosition;
        Vector2 nextGridPos = myGridPos;

        if (myGridPos.x < targetGridPos.x)
        {
            nextGridPos.x++;
        }
        else if (myGridPos.x > targetGridPos.x)
        {
            nextGridPos.x--;
        }
        else if (myGridPos.y < targetGridPos.y)
        {
            nextGridPos.y++;
        }
        else if (myGridPos.y > targetGridPos.y)
        {
            nextGridPos.y--;
        }

        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if (aSpace.GetComponent<Space_Script>().gridPosition == nextGridPos)
            {
                return aSpace;
            }
        }

        Debug.LogError("WARNING: Space could not be found, check your space's gridPositions as no space could be found with co-ordinates: " + nextGridPos + " on the path to " + targetGridPos);
        return null;
    }

    protected void flipSpriteRight()
    {
        isFacingRight = true;
        this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }

    protected void flipSpriteLeft()
    {
        isFacingRight = false;
        this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }

    public void onMeleeAnimationDone()
    {
        GameObject targetMinion = findMinionToAttack();
        if (targetMinion != null)
        {
            targetMinion.GetComponent<Minion_AI_Script>().onHitByAttack(this.meleeDamage);
        }
    }

    public static GameObject findNearestEnemy(Vector3 position)
    {
        Vector3 closestVector = new Vector3(-999999999999999999, -999999999999999999, 0);
        GameObject closestSpace = null;
        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Enemy"))
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
    public static GameObject findNearestEnemyWithinRange(Vector3 position, float range)
    {
        GameObject closestSpace = findNearestEnemy(position);
        Vector3 spacePosIgnoreZ = new Vector3(closestSpace.transform.position.x, closestSpace.transform.position.y, position.z);
        if (Vector3.Distance(position, spacePosIgnoreZ) < range)
        {
            return closestSpace;
        }
        return null;
    }

    
    //[Conversion Methods]
    //Replaces the Update logic while the enemy is being converted
    protected void conversionUpdate()
    {
        //Move the Enemy to the ritual's holding position.
        Vector3 ritualPos = conversionHoldingPoint;
        float dragSpeed = 5.0f;
        ritualPos.z = this.transform.position.z;
        if(this.transform.position != ritualPos)
        {
            Vector3 v = (ritualPos - this.transform.position).normalized * dragSpeed * Time.deltaTime;
            if ((ritualPos - this.transform.position).magnitude < v.magnitude)
            {
                this.transform.position = ritualPos;
            }
            else
            {
                this.transform.position += v;
            }
        }

        //Update Ritual Particles to increase speed and emission rate as ritual progress increases.
        ParticleSystem.ShapeModule ritualParticleShape = this.gameObject.GetComponent<ParticleSystem>().shape;
        ritualParticleShape.radiusSpeed = 1 + (1.0f * (conversionProgress / conversionProgressRequired));
        ParticleSystem.EmissionModule RitualParticleEmit = this.gameObject.GetComponent<ParticleSystem>().emission;
        RitualParticleEmit.rateOverTime = 10 + (90 * (conversionProgress / conversionProgressRequired));

        //If ritual complete, stop animations, spawn the ritual complete particle effect, kill the enemy and add a new minion to roster
        if (conversionProgress >= conversionProgressRequired)
        {
            conversionComplete();
        }
        else if(Enemy_Spawning_Script.hasHordeHasSpawnedAllEnemies() && GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            conversionFailed();
        }

    }

    //Replaces the update logic with logic that moves the minion away from the ritual point and to a space in the necromancer's row.
    protected void escapingConversionUpdate()
    {
        //TODO: change X = 4 to be smart-calculated rather than hard-coded.
        Debug.Log("escaping Conversion to point: (3, " + tryingToKill.GetComponent<Minion_AI_Script>().getTargetSpace().GetComponent<Space_Script>().gridPosition.y + ")");
        Vector3 ritualPos = Space_Script.findGridSpace(new Vector2(3 , tryingToKill.GetComponent<Minion_AI_Script>().getTargetSpace().GetComponent<Space_Script>().gridPosition.y)).transform.position;
        float dragSpeed = 10.0f;
        ritualPos.z = this.transform.position.z;
        if (this.transform.position != ritualPos)
        {
            Vector3 v = (ritualPos - this.transform.position).normalized * dragSpeed * Time.deltaTime;
            if ((ritualPos - this.transform.position).magnitude < v.magnitude)
            {
                this.transform.position = ritualPos;
            }
            else
            {
                this.transform.position += v;
            }
        }
        else
        {
            this.nextSpace = Space_Script.findGridSpace(new Vector2(3, tryingToKill.GetComponent<Minion_AI_Script>().getTargetSpace().GetComponent<Space_Script>().gridPosition.y));
            this.escapingBeingConverted = false;
        }
    }

    protected void conversionComplete()
    {
        this.setBeingConverted(false);
        GameObject.FindGameObjectWithTag("Necromancer Ritual Button").GetComponent<Button>().interactable = true;
        this.gameObject.GetComponent<ParticleSystem>().Stop();
        Instantiate(ritualCompleteParticleEmitter, this.transform.position, this.transform.rotation);
        createNewMinionFromEnemy();
        foreach(GameObject aMote in GameObject.FindGameObjectsWithTag("Dark Energy Mote"))
        {
            Destroy(aMote);
        }
        Destroy(this.gameObject);
    }

    protected void conversionFailed()
    {
        this.setBeingConverted(false);
        this.escapingBeingConverted = true;
        this.gameObject.GetComponent<ParticleSystem>().Stop();
        GameObject.FindGameObjectWithTag("Necromancer Ritual Button").GetComponent<Button>().interactable = true;
        foreach (GameObject aMote in GameObject.FindGameObjectsWithTag("Dark Energy Mote"))
        {
            Destroy(aMote);
        }
    }

    //Note: Virtual denotes that this method should be overriden in derived classes
    protected virtual void createNewMinionFromEnemy()
    {
        //MinionRoster.addNewMinion("Absolute Unit");
        MinionRoster.addNewMinion(1, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, AbilityID.molotov, AbilityID.sprayAndPray, Ability_Database.findRandomAbilityWithTag("general").id);
    }

    //Called by Ability_Database.castNecromancerConversionRitual() when conversion ability is ast on this enemy
    public void setBeingConverted(bool inConverting)
    {
        this.beingConverted = inConverting;
        conversionProgress = 0;
        this.rigAnimator.SetBool("IsConverting", inConverting);
        this.gameObject.GetComponent<BoxCollider2D>().enabled = !inConverting; //disable/enable collision on enemy.
        if (inConverting)
        {
            this.rigAnimator.SetTrigger("StartConverting"); //trigger the transition animation
            this.gameObject.GetComponent<ParticleSystem>().Play();
            
        }
    }

    public bool isBeingConverted()
    {
        return this.beingConverted;
    }

    public void addProgressToConversion(int energyIn)
    {
        this.conversionProgress += energyIn;
    }

    public void onHitByProjectile(Projectile_Logic_Script projectile)
    {
        this.currentHP -= projectile.projectileDamage;
        Instantiate(hurtParticleEmitter, this.transform.position + new Vector3(0, 1, 0), this.transform.rotation);
        if(this.currentHP <= 0)
        {
            die();
        }
    }

    public void onHitByMelee(int damageIn)
    {
        this.currentHP -= damageIn;
        Instantiate(hurtParticleEmitter, this.transform.position + new Vector3(0, 1, 0), this.transform.rotation);
        if (this.currentHP <= 0)
        {
            die();
        }
    }

    public void onHitByDamagingEffect(int damageIn)
    {
        this.currentHP -= damageIn;
        Instantiate(hurtParticleEmitter, this.transform.position + new Vector3(0, 1, 0), this.transform.rotation);
        if (this.currentHP <= 0)
        {
            die();
        }
    }

    public void die()
    {
        Dark_Energy_Meter_Script.addDarkEnergyOnEnemySlain(this.energyRewardOnKill, this.gameObject);
        isDying = true;
        rigAnimator.SetBool("IsDying", isDying);
        this.gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void onDeathAnimationDone()
    {
        Destroy(this.gameObject);
    }
}
