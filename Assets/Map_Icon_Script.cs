using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Icon_Script : MonoBehaviour
{
    [Header("Scenario")]
    [Tooltip("Prefix a hordeName with BATTLE_ to mark this button as triggering a battle")]
    public string scenarioName;

    [Header("Icon Sprites")]
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
        GameObject.FindGameObjectWithTag("Map Journal").GetComponent<Journal_Text_Script>().setJournalScenario(this.scenarioName);
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
