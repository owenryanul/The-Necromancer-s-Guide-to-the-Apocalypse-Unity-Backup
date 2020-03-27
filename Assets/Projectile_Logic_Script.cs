using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Logic_Script : MonoBehaviour
{
    public bool isEnemyProjectile = false;
    //public float lifeTime = 10;
    
    //private float currentLifeTime;

    [Header("Movement")]
    public GameObject mySpace;
    private GameObject targetSpace;
    public float speed = 1.0f;

    [Header("Damage")]
    public int projectileDamage = 1;

    // Use this for initialization
    void Start()
    {
        //currentLifeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {


        Vector3 myPos = this.transform.position;
        Vector3 mySpacePos = mySpace.transform.position;
        mySpacePos.z = this.transform.position.z; //ignore the z dimension

        if (myPos != mySpacePos) //if not at mySpace, move to it
        {
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
        else
        {
            Destroy(this.gameObject);
        }

        //TODO: Destroy on reaching target
        /*currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            Destroy(this.gameObject);
        }*/
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy") //If collided with enemy
        {
            float hitEnemyGridY = col.gameObject.GetComponent<Enemy_AI_script>().nextSpace.GetComponent<Space_Script>().gridPosition.y;
            //if enemy is on the same grid row
            if (hitEnemyGridY == this.mySpace.GetComponent<Space_Script>().gridPosition.y)
            {
                //Debug.Log("Enemy hit");
                col.gameObject.GetComponent<Enemy_AI_script>().onHitByProjectile(this.gameObject.GetComponent<Projectile_Logic_Script>());

                Destroy(this.gameObject);
            }
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

        foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
        {
            if (aSpace.GetComponent<Space_Script>().gridPosition == nextGridPos)
            {
                return aSpace;
            }
        }

        foreach (GameObject aEndSpace in GameObject.FindGameObjectsWithTag("End Space"))
        {
            if (aEndSpace.GetComponent<Space_Script>().gridPosition == nextGridPos)
            {
                return aEndSpace;
            }
        }

        Debug.LogError("WARNING: Space could not be found, check your space's gridPositions as no space could be found with co-ordinates: " + nextGridPos + " on the path to " + targetGridPos);
        return null;
    }

    public void setTargetSpace(GameObject inSpace)
    {
        this.targetSpace = inSpace;
    }
}
