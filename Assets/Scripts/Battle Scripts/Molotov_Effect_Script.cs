using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov_Effect_Script : MonoBehaviour
{
    public GameObject mySpace;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                col.gameObject.GetComponent<Enemy_AI_script>().onHitByDamagingEffect(this.damage);
                //GameObject will be destroyed by the onDeath effect of it's particle system
            }
        }
    }
}
