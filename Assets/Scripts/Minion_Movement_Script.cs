using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Movement_Script : MonoBehaviour
{
    [Header("Movement")]
    public GameObject mySpace;
    private GameObject targetSpace;
    public float speed = 1.0f;
    public bool isFacingRight = true;

    private Animator rigAnimator;

    private bool isMoving;

    [Header("Attack")]
    public bool isRangedAttack;
    public GameObject projectile;
    public float attackCooldown;
    private float currentAttackCooldown;
    [Tooltip("AttackRange indicates weapon spread. AttackRange.Length determines how many gridSpaces horizontally the attack reaches. While each element determines how many gridspaces the attack spread vertically from the corripoding grid space. E.g. (0,1,2) would reach create a cone that reaches 3 spaces across, has no spread from the 1st space, spreads 1 space above AND below from the 2nd space across and spreads to the 2 spaces above AND below the 3rd space across.")]
    public int[] attackRange;
    private const float yDistanceToBeConsideredInTheSameRow = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        targetSpace = mySpace;
        rigAnimator = this.gameObject.GetComponentInChildren<Animator>();
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    //If minion clicked, set them as the currently selected minion
    private void OnMouseDown()
    {
        Debug.Log("Minion Clicked");
        Space_Script.setCurrentlySelectedMinion(this.gameObject);      
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
                            break;
                        }
                    }
                }
            }

            //Detect enemies along aim
            Debug.DrawRay(this.transform.position, (spaceAimedAt.transform.position - this.transform.position), Color.red, 1);
            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, (spaceAimedAt.transform.position - this.transform.position));
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
                        //Make the attack
                        GameObject proj = Instantiate(projectile, this.transform.position, this.transform.rotation);
                        proj.GetComponent<Projectile_Logic_Script>().mySpace = this.mySpace;
                        proj.GetComponent<Projectile_Logic_Script>().setTargetSpace(spaceAimedAt);
                        break;
                    }
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
}
