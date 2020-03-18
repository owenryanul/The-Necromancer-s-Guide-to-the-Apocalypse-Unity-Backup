using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Logic : MonoBehaviour {

    public bool isEnemyProjectile = false;
    public Vector3 direction = new Vector3(1,0);
    public float lifeTime = 10;
    public float speed = 1;
    private float currentLifeTime;

	// Use this for initialization
	void Start () {
        currentLifeTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position += (direction * speed * Time.deltaTime);
        currentLifeTime += Time.deltaTime;
        if(currentLifeTime >= lifeTime)
        {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        try
        {
            if (col.gameObject.transform.parent.gameObject.tag == "Enemy")
            {
                Debug.Log("Enemy hit");
                Destroy(col.gameObject.transform.parent.gameObject);
                Destroy(this.gameObject);
            }
        }
        catch(System.NullReferenceException e)
        {
            //if collider has no parents, ignore
        }
    }
}
