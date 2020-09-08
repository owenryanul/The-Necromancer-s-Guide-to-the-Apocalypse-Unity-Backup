using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dark_Energy_Mote_Script : MonoBehaviour
{
    public Vector3 target;
    public float speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if (this.transform.position != target)
            {
                Vector3 v = (target - this.transform.position).normalized * speed * Time.deltaTime;
                if ((target - this.transform.position).magnitude < v.magnitude)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    this.transform.position += v;
                }
            }
        }
    }
}
