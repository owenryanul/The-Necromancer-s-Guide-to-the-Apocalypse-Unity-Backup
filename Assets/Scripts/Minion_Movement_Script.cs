using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Movement_Script : MonoBehaviour {

    private bool isMoving;
    private float distanceRemainingToMove;
    private Vector2 currentDirection;
    private Vector2 nextSpaceLocation;
    public float speed = 0.5f;
    public float snapToDistance = 0.1f;
    private Vector2 facing;

    public Vector2 targetSpace;

    //Selection Variables
    public bool isSelected = false;
    public BoxCollider2D selectionHitBox;


    public Vector2 oneSpaceUpDirection = new Vector2(1.2f,2);
    public Vector2 oneSpaceRightDirection = new Vector2(3.9f, 0);


    public Component debugComponet;

    public float weaponRange = -1;
    public float weaponCooldown = 1;
    private float timeSinceWeaponFired;
    public GameObject weaponProjectile;
    public Vector3 projectileDisplacement;

	// Use this for initialization
	void Start () {
        isMoving = false;
        distanceRemainingToMove = 0.0f;
        selectionHitBox = this.gameObject.GetComponentInChildren<BoxCollider2D>();
        targetSpace = this.gameObject.transform.position;
        facing = new Vector2(1, 0);
        timeSinceWeaponFired = 0;
	}
	
	// Update is called once per frame
	void Update () {

        selectionLogic();
        movementLogic();
        attackLogic();
    }

    private void selectionLogic()
    {
        Camera c = Camera.main;
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(isSelected)
            {
                targetSpace = Camera.main.GetComponent<spaceSelection>().getSelectedSpace(this.gameObject.transform.position);
            }

            if (selectionHitBox.bounds.Contains((Vector2) c.ScreenToWorldPoint(Input.mousePosition)))
            {
                isSelected = true;
            }
            else
            {
                isSelected = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isSelected = false;
        }
    }

    private void movementLogic()
    {
        if (!isMoving)
        {
            if (Mathf.Abs(this.gameObject.transform.position.y - targetSpace.y) > snapToDistance)
            {
                if (this.gameObject.transform.position.y < targetSpace.y)
                {
                    nextSpaceLocation = this.gameObject.transform.position + (Vector3)oneSpaceUpDirection;
                    currentDirection = oneSpaceUpDirection;
                    isMoving = true;
                    swapToWalkAnimation();
                }
                else if (this.gameObject.transform.position.y > targetSpace.y)
                {
                    nextSpaceLocation = this.gameObject.transform.position - (Vector3)oneSpaceUpDirection;
                    currentDirection = (-oneSpaceUpDirection);
                    isMoving = true;
                    swapToWalkAnimation();
                }
            }
            else if (Mathf.Abs(this.gameObject.transform.position.x - targetSpace.x) > snapToDistance)
            { 
                if (this.gameObject.transform.position.x > targetSpace.x)
                {
                    nextSpaceLocation = this.gameObject.transform.position - (Vector3)oneSpaceRightDirection;
                    currentDirection = (-oneSpaceRightDirection);
                    isMoving = true;
                    swapToWalkAnimation();
                }
                else if (this.gameObject.transform.position.x < targetSpace.x)
                {
                    nextSpaceLocation = this.gameObject.transform.position + (Vector3)oneSpaceRightDirection;
                    currentDirection = oneSpaceRightDirection;
                    isMoving = true;
                    swapToWalkAnimation();
                }
            }
        }
        else
        {
            move(currentDirection);
        }
    }

    private void move(Vector2 direction)
    {
        Vector3 direction3D = direction.normalized;
        distanceRemainingToMove = Vector2.Distance(this.gameObject.transform.position, nextSpaceLocation);
        
        if (isMoving)
        {
            if (distanceRemainingToMove <= snapToDistance)
            {
                this.gameObject.transform.position = nextSpaceLocation;
                isMoving = false;
                swapToIdleAnimation();
            }
            else
            {
                this.gameObject.transform.position += (direction3D * speed * Time.deltaTime);
            }
        }
        setPositionInSortingLayer();
    }

    private bool isWithin(float vector1, float vector2, float distance)
    {
        if(vector1 - vector2 <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void setPositionInSortingLayer()
    {
        debugComponet = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(this.gameObject.transform.position.y);
    }

    private void swapToWalkAnimation()
    {
        Animator animator = this.gameObject.GetComponentInChildren<Animator>();
        animator.SetBool("Walk", true);
    }

    private void swapToIdleAnimation()
    {
        Animator animator = this.gameObject.GetComponentInChildren<Animator>();
        animator.SetBool("Walk", false);
    }


    private void attackLogic()
    {
        if ((timeSinceWeaponFired >= weaponCooldown))
        {
            if (isEnemyInRange())
            {
                GameObject projectile = Instantiate(weaponProjectile);
                projectile.transform.position = (this.transform.position + projectileDisplacement);
                projectile.GetComponent<Projectile_Logic>().direction = facing;
                timeSinceWeaponFired = 0;
            }
        }
        else
        {
            timeSinceWeaponFired += Time.deltaTime;
        }
    }

    private bool isEnemyInRange()
    {
        Vector2 startOfRay = (new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y));
        Ray2D aimRay = new Ray2D(startOfRay, facing);
        RaycastHit2D[] rayHit;
        if (weaponRange < 0)
        {
            rayHit = Physics2D.RaycastAll(startOfRay, facing);
            Debug.DrawRay(startOfRay, facing, Color.red);
        }
        else
        {
            rayHit = Physics2D.RaycastAll(startOfRay, facing, weaponRange);
            Debug.DrawRay(startOfRay, facing, Color.red);
        }

        foreach (RaycastHit2D aHit in rayHit)
        {
            try
            {
                if (aHit.transform.parent.gameObject.tag == "Enemy")
                {
                    //Debug.Log("enemy spotted");
                    return true;
                }
            }
            catch(System.NullReferenceException e)
            {

            }
        }
        //Debug.Log("not detect above");
        return false;
    }

}
