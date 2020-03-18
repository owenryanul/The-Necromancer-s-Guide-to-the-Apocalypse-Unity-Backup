using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceSelection : MonoBehaviour {

    public Vector2 selectedSpace;
    public GameObject selectedThing;

	// Use this for initialization
	void Start () {
        selectedSpace = new Vector2(0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public Vector2 getSelectedSpace(Vector2 currentPosition)
    {
        Camera c = Camera.main;
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            //selectedSpace = c.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(c.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Space"))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want

                selectedSpace = hit.transform.position;
                selectedThing = hit.transform.gameObject;
                return selectedSpace;
            }
        }
        return currentPosition;
    }

}
