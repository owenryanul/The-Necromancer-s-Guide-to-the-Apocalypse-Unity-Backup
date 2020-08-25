using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Scenarios_Database_Script : MonoBehaviour
{
    public List<Scenario> scenarios;

    private static Scenarios_Database_Script instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        { 
            instance = this;
            loadAllScenariosFromFiles();
            /*foreach (Scenario aSce in instance.scenarios)
            {
                aSce.saveAsJson();
            }*/
        }
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

    private void loadAllScenariosFromFiles()
    {
        this.scenarios = new List<Scenario>();
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/databases/scenarios/");
        foreach (FileInfo file in di.GetFiles("*.json"))
        {
            string json = Scenario.loadJsonFromFile(file.Name);
            this.scenarios.Add(Scenario.fromJson(json));
        }
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

    public string toJson()
    {
        string raw = JsonUtility.ToJson(this, true);
        return raw;
    }

    public static Scenario fromJson(string jsonIn)
    {
        string j = jsonIn;
        Scenario jScenario = JsonUtility.FromJson<Scenario>(j);
        return jScenario;
    }

    public void saveAsJson()
    {
        string json = this.toJson();

        string path = Application.persistentDataPath + "/databases/scenarios/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path += this.name.Replace(" ", "_") + ".json"; //Replaces any spaces in the horde name with _, to make it compatiable with file storage
        if (!File.Exists(path))
        {
            //File.Create(path);
            Debug.Log("Opening writer to path: " + path);
            StreamWriter writer = File.AppendText(path);
            writer.Write(json);
            writer.Close();
        }
        else
        {
            Debug.Log("Opening writer to path: " + path);
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(json);
            writer.Close();
        }
    }

    public static string loadJsonFromFile(string fileNameWithExtension)
    {
        string path = Application.persistentDataPath + "/databases/scenarios/";
        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Error: Directory: " + path + " not found.");
            return null;
        }

        path += fileNameWithExtension.Replace(" ", "_"); //Replace any spaces in the passed filename with _, as the save system converts all spaces  in the filename to _ when saving the file.

        if (!File.Exists(path))
        {
            Debug.LogWarning("Error: File: " + path + " not found.");
            return null;
        }

        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();

        return json;
    }
}
