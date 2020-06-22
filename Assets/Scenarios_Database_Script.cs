using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenarios_Database_Script : MonoBehaviour
{
    public List<Scenario> scenarios;

    private static Scenarios_Database_Script instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Scenario findScenario(string scenarioName)
    {
        foreach(Scenario aScenario in instance.scenarios)
        {
            if(aScenario.name == scenarioName)
            {
                return aScenario;
            }
        }
        Debug.LogWarning("Warning: Unable to find Scenario in ScenarioDatabase with name = " + scenarioName);
        return new Scenario("ERROR_Scenario_Name_Not_Found", "I AM ERROR!");
    }
}

[System.Serializable]
public class Scenario
{
    public string name;
    [TextArea(15,20)]
    public string journelText;

    public Scenario(string namein, string journelTextin)
    {
        this.name = namein;
        this.journelText = journelTextin;
    }
}
