using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI_script : MonoBehaviour
{
    [Header("Movement")]
    public GameObject nextSpace;
    public float speed = 1.0f;
    private GameObject targetSpace;
    private bool isMoving;

    [Header("Attacks")]
    public float meleeRange = 1.0f;
    private GameObject minionToAttack;
    private const float yDistanceToBeConsideredInTheSameRow = 0.1f;

    [Header("AI")]
    public GameObject tryingToKill;

    [Header("Animation")]
    private Animator rigAnimator;
    private bool isFacingRight;

    

    // Start is called before the first frame update
    void Start()
    {
        tryingToKill = GameObject.FindGameObjectWithTag("Necromancer");
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Set target to the space currently occupied by the necromancer
        this.targetSpace =  tryingToKill.GetComponent<Minion_Movement_Script>().targetSpace;

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

        attackLogic();
        if (minionToAttack != null) //if a valid target is in melee range, then attack
        {
            //TODO: Apply effects of attack
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


        rigAnimator.SetBool("IsWalking", isMoving);
        if (isMoving)
        {
            //TODO: Set sorting layer order to render based on y position
        }
    }

    private void attackLogic()
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, vectorDirection, this.meleeRange);
        bool noMinionFound = true;
        foreach(RaycastHit2D aHit in hits) 
        {
            if(aHit.collider.gameObject.CompareTag("Minion") || aHit.collider.gameObject.CompareTag("Necromancer"))
            {
                float yDistanceToHitMinion = Mathf.Abs(aHit.collider.gameObject.transform.position.y - this.gameObject.transform.position.y);
                if (yDistanceToHitMinion <= yDistanceToBeConsideredInTheSameRow)
                {
                    noMinionFound = false;
                    this.minionToAttack = aHit.collider.gameObject;
                    break;
                }
            }
        }

        //if no valid target in range, set to not attacking
        if(noMinionFound)
        {
            this.minionToAttack = null;
            this.rigAnimator.SetBool("IsAttacking", false);
        }
        else
        {
            //Attack
            this.rigAnimator.SetBool("IsAttacking", true);
        }   
    }


    //Find the next space on the path between the currently occupied space and the target space.
    //Note: Will attempt to match target spaces X before matching Y
    private GameObject findNextSpace()
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

    private void flipSpriteRight()
    {
        isFacingRight = true;
        this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }

    private void flipSpriteLeft()
    {
        isFacingRight = false;
        this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }
}
