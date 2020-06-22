using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Icon_Script : MonoBehaviour
{
    public string scenarioName;

    public Sprite icon;
    public Sprite highlightedIcon;
    public Sprite yellowHighlightedIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked on Map Icon: " + this.gameObject.name);
        GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().loadScenario(this.scenarioName);
        GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().showJournel();
    }

    private void OnMouseEnter()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = highlightedIcon;
    }

    private void OnMouseExit()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = icon;
    }
}
