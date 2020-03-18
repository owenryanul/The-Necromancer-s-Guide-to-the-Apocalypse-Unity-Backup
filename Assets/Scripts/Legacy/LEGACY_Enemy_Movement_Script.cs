using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement_Script : MonoBehaviour {

    private bool isMovingUpDown;
    private float distanceRemainingToMove;
    private Vector2 currentDirection;
    private Vector2 nextSpaceLocation;
    private Vector2 forwardDirection;
    public float speed = 0.5f;
    public float snapToDistance = 0.1f;

    public Vector2 targetSpace;
    public GameObject targetObject;

    //Selection Variables
    public bool isSelected = false;
    public BoxCollider2D selectionHitBox;

    public Vector2 oneSpaceUpDirection = new Vector2(1.2f, 2);
    public Vector2 oneSpaceRightDirection = new Vector2(3.9f, 0);


    public Component debugComponet;
    public Vector2 debugVector;

    // Use this for initialization
    void Start()
    {
        isMovingUpDown = false;
        distanceRemainingToMove = 0.0f;
        selectionHitBox = this.gameObject.GetComponentInChildren<BoxCollider2D>();
        targetSpace = this.gameObject.transform.position;

        targetObject = GameObject.FindGameObjectWithTag("Necromancer");
    }

    // Update is called once per frame
    void Update()
    {
        targetSpace = snapToNearestSpace(GameObject.FindGameObjectWithTag("Necromancer").transform.position);
        
        movementLogic();
        //setPositionInSortingLayer();
    }

    void OnMouseDown()
    {

    }

    Vector2 snapToNearestSpace(Vector2 inPos)
    {
        Vector2 pos = inPos;

        //Remove the x-drift from the y-positioning(due to angled grid)
        if(pos.y > 0)
        {
            pos.x -= (oneSpaceUpDirection.x * pos.y);
        }
        else if (pos.y < 0)
        {
            pos.x += (oneSpaceUpDirection.x * pos.y);
        }

        //Sort leftright without the x-drift interfering
        if (pos.x > 0)
        {
            if ((pos.x % oneSpaceRightDirection.x) >= (oneSpaceRightDirection.x / 2))
            {
                pos.x += (oneSpaceRightDirection.x - (pos.x % oneSpaceRightDirection.x));
            }
            else
            {
                pos.x -= ((pos.x % oneSpaceRightDirection.x));
            }
        }
        else if (pos.x < 0)
        {
            if ((pos.x % oneSpaceRightDirection.x) >= -(oneSpaceRightDirection.x / 2))
            {
                pos.x -= (oneSpaceRightDirection.x + (pos.x % oneSpaceRightDirection.x));
            }
            else
            {
                pos.x += ((pos.x % oneSpaceRightDirection.x));
            }
        }

        //Re-add the x-drift from the y-positioning(due to angled grid)
        if (pos.y > 0)
        {
            pos.x += (oneSpaceUpDirection.x * pos.y);
        }
        else if (pos.y < 0)
        {
            pos.x -= (oneSpaceUpDirection.x * pos.y);
        }


        //Sort the updown
        if (pos.y > 0)
        {
            if ((pos.y % oneSpaceUpDirection.x) >= (oneSpaceUpDirection.x / 2))
            {
                pos.y += (oneSpaceUpDirection.x - (pos.y % oneSpaceUpDirection.x));
            }
            else
            {
                pos.y -= ((pos.y % oneSpaceUpDirection.x));
            }
        }
        else if (pos.y < 0)
        {
            if ((pos.y % oneSpaceUpDirection.x) >= -(oneSpaceUpDirection.x / 2))
            {
                pos.y -= (oneSpaceUpDirection.x + (pos.y % oneSpaceUpDirection.x));
            }
            else
            {
                pos.y += ((pos.y % oneSpaceUpDirection.x));
            }
        }
        debugVector = pos;
        return pos;
    }

    private void movementLogic()
    {
        forwardDirection = targetIsLeftOrRight(targetObject);
        if (!targetDetectedInfront(targetObject))
        {
            if ((this.gameObject.transform.position.y != targetObject.transform.position.y) && !isMovingUpDown)
            {
                if (targetDetectedAbove(targetObject))
                {
                    nextSpaceLocation = this.gameObject.transform.position + (Vector3)oneSpaceUpDirection;
                    currentDirection = oneSpaceUpDirection;
                    isMovingUpDown = true;
                }
                else if (targetDetectedBelow(targetObject))
                {
                    nextSpaceLocation = this.gameObject.transform.position - (Vector3)oneSpaceUpDirection;
                    currentDirection = (-oneSpaceUpDirection);
                    isMovingUpDown = true;
                }
            }

            if (isMovingUpDown)
            {
                moveUpDown(currentDirection);
            }
            else
            {
                moveRightLeft(forwardDirection);
            }
        }
    }

    private void moveUpDown(Vector2 direction)
    {
        Vector3 direction3D = direction.normalized;
        distanceRemainingToMove = Vector2.Distance(this.gameObject.transform.position, nextSpaceLocation);

        if (isMovingUpDown)
        {
            if (distanceRemainingToMove <= snapToDistance)
            {
                this.gameObject.transform.position = nextSpaceLocation;
                isMovingUpDown = false;
                swapToIdleAnimation();
            }
            else
            {
                this.gameObject.transform.position += (direction3D * speed * Time.deltaTime);
            }
        }
        setPositionInSortingLayer();
    }

    private void moveRightLeft(Vector3 direction)
    {
        swapToWalkAnimation();
         //target is to right so
         this.gameObject.transform.position += (direction * speed * Time.deltaTime);
    }

    private bool targetDetectedAbove(GameObject targetObject)
    {
        Vector2 startOfRay = (new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y) + (forwardDirection * 2));
        Ray2D upRay = new Ray2D(startOfRay, oneSpaceUpDirection);
        RaycastHit2D[] rayHit = Physics2D.RaycastAll(startOfRay, oneSpaceUpDirection);
        Debug.DrawRay(startOfRay, oneSpaceUpDirection, Color.red);
        foreach (RaycastHit2D aHit in rayHit)
        {
            if (aHit.transform.parent.gameObject.tag == targetObject.tag)
            {
                Debug.Log("detect above");
                return true;
            }
        }
        //Debug.Log("not detect above");
        return false;

    }

    private bool targetDetectedBelow(GameObject targetObject)
    {
        Vector2 startOfRay = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y) + (forwardDirection * 2);
        RaycastHit2D[] rayHit = Physics2D.RaycastAll(startOfRay, -oneSpaceUpDirection);
        Debug.DrawRay(startOfRay, -oneSpaceUpDirection, Color.blue);
        foreach (RaycastHit2D aHit in rayHit)
        {
            if (aHit.transform.parent.gameObject.tag == targetObject.tag)
            {
                Debug.Log("detect below");
                return true;
            }
        }
        //Debug.Log("not detect below");
        return false;
    }

    //returns true of the target object is detected infront(the direction the enemy is moving) of the enemy
    private bool targetDetectedInfront(GameObject targetObject)
    {
        RaycastHit2D[] rayHit = Physics2D.RaycastAll(this.gameObject.transform.position, forwardDirection, 1);
        Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + (new Vector3(forwardDirection.x, forwardDirection.y) * 1), Color.cyan);
        foreach (RaycastHit2D aHit in rayHit)
        {
            try
            { 
                if (aHit.transform.parent.gameObject.tag == targetObject.tag)
                {
                    Debug.Log("detect infront");
                    return true;
                }
            }
            catch (System.NullReferenceException e)
            {

            }
        }
        return false;
    }

    private Vector2 targetIsLeftOrRight(GameObject targetObject)
    {
        if(targetObject.transform.position.x > this.gameObject.transform.position.x)
        {
            return new Vector2(1, 0);
        }
        else
        {
            return new Vector2(-1, 0);
        }
    }

    private bool isWithin(float vector1, float vector2, float distance)
    {
        if (vector1 - vector2 <= distance)
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
}
