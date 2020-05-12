using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs = Buff_Database_Script;

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

    [Header("Effects")]
    public GameObject deathParticles;

    [Header("Scatter Projectile")]
    public GameObject scatterProjectilePrefab;
    private int[] scatterPattern;
    private int scatterSpacesTraveled;
    private ScatterDirection scatterDirection;
    public enum ScatterDirection
    {
        Up,
        Down,
        Both,
        None
    }

    // Use this for initialization
    void Start()
    {
        //currentLifeTime = 0;
        scatterLogic();
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
        else if (mySpace != targetSpace) //if at mySpace check if its your target space, if not change mySpace and run scatter logic
        {
            scatterLogic();
            scatterSpacesTraveled++;
            mySpace = findNextSpaceForProjectile();
        }
        else
        {
            Instantiate(deathParticles, this.transform.position, this.transform.rotation);
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
                Instantiate(deathParticles, this.transform.position, this.transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }

    //Find the next space on the path between the currently occupied space and the target space.
    //Note: Will attempt to match target spaces Y before matching X
    private GameObject findNextSpaceForProjectile()
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

        if (myGridPos.x < targetGridPos.x)
        {
            nextGridPos.x++;
        }
        else if (myGridPos.x > targetGridPos.x)
        {
            nextGridPos.x--;
        }

        GameObject gridSpace = Space_Script.findGridSpace(nextGridPos);
        if(gridSpace != null)
        {
            return gridSpace;
        }

        gridSpace = Space_Script.findGridEndSpace(nextGridPos);
        if(gridSpace != null)
        {
            return gridSpace;
        }

        /*foreach (GameObject aSpace in GameObject.FindGameObjectsWithTag("Space"))
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
        }*/

        Debug.LogError("WARNING: Space could not be found, check your space's gridPositions as no space could be found with co-ordinates: " + nextGridPos + " on the path to " + targetGridPos);
        return null;
    }

    private void scatterLogic()
    {
        if (scatterDirection != ScatterDirection.None)
        {
            if (scatterPattern.Length > scatterSpacesTraveled)
            {
                int scatterValue = scatterPattern[scatterSpacesTraveled];
                if(scatterValue > 0)
                {
                    //Begin Generating Scatter Projectiles.

                    for(int count = 1; count <= scatterValue; count++)
                    {

                        //Generate Projectile that scatters down
                        if (this.scatterDirection == ScatterDirection.Down || this.scatterDirection == ScatterDirection.Both)
                        {
                            //Get the scattering down projectile's target space
                            GameObject aScatterTargetSpace = Space_Script.findGridSpace(this.targetSpace.GetComponent<Space_Script>().gridPosition + new Vector2(0, count), true);
                            if (aScatterTargetSpace != null) //if no approiate space can be found, then don't scatter as you're probablly at a board edge. NOTE THIS HAS NOT BEEN TUNED FOR END SPACES YET
                            {
                                //If target space exists, spawn projectile
                                GameObject aScatterProjectile = Instantiate(scatterProjectilePrefab, this.transform.position, this.transform.rotation);
                                //Assign target space and scatter direction of new projectile.
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().mySpace = this.mySpace;
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setTargetSpace(aScatterTargetSpace);
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setScatterDirection(ScatterDirection.Down);

                                //Pass new scatter pattern to new projectile
                                int[] childPattern = new int[scatterPattern.Length - scatterSpacesTraveled];
                                for (int j = 0; j < childPattern.Length; j++)
                                {
                                    childPattern[j] = (scatterPattern[scatterSpacesTraveled + j] - 1);
                                }
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setScatterPattern(childPattern);
                            }
                        }
                        //Generate Projectile that scatters up
                        if (this.scatterDirection == ScatterDirection.Up || this.scatterDirection == ScatterDirection.Both)
                        {
                            //Get the scattering down projectile's target space
                            GameObject aScatterTargetSpace = Space_Script.findGridSpace(this.targetSpace.GetComponent<Space_Script>().gridPosition - new Vector2(0, count), true);
                            if (aScatterTargetSpace != null) //if no approiate space can be found, then don't scatter as you're probablly at a board edge. NOTE THIS HAS NOT BEEN TUNED FOR END SPACES YET
                            {
                                //If target space exists, spawn projectile
                                GameObject aScatterProjectile = Instantiate(scatterProjectilePrefab, this.transform.position, this.transform.rotation);
                                //Assign target space and scatter direction of new projectile.
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().mySpace = this.mySpace;
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setTargetSpace(aScatterTargetSpace);
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setScatterDirection(ScatterDirection.Up);

                                //Pass new scatter pattern to new projectile
                                int[] childPattern = new int[scatterPattern.Length - scatterSpacesTraveled];
                                for (int j = 0; j < childPattern.Length; j++)
                                {
                                    childPattern[j] = (scatterPattern[scatterSpacesTraveled + j] - 1);
                                }
                                aScatterProjectile.GetComponent<Projectile_Logic_Script>().setScatterPattern(childPattern);
                            }
                        }
                    }

                    //Once scatter projectiles are finished, tell this projectile to not scatter again.
                    this.setScatterDirection(ScatterDirection.None);
                }
            }
        }
    }

    public void setTargetSpace(GameObject inSpace)
    {
        this.targetSpace = inSpace;
    }

    public void setScatterPattern(int[] inPattern)
    {
        this.scatterPattern = inPattern;
    }

    public void setScatterDirection(ScatterDirection directionIn)
    {
        this.scatterDirection = directionIn;
    }

}
