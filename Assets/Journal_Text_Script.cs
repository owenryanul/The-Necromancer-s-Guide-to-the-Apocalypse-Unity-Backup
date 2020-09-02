﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Journal_Text_Script : MonoBehaviour
{
    [TextArea]
    [Tooltip("Use [PAGE BREAK] to seperate pages")]
    public string journalText;

    public GameObject pageButtonPrefab;

    [TextArea]
    [Tooltip("Use [SIZE] to mark placement of horde size and [COMPOSITION] to mark where to place the composition")]
    public string preBattleText;
    [TextArea]
    [Tooltip("Use [SIZE] to mark placement of horde size and [COMPOSITION] to mark where to place the composition")]
    public string postBattleText;

    private string[] journalPages;
    private int pageNumber;

    private Text leftText;
    private Text rightText;

    private Button previousButton;
    private Button nextButton;

    private Button closeJournalButton;

    private Button toBattleButton;
    private string currentBattleName;

    private Button leaveBattleSummaryButton;

    private string buttonMarkupReplacementText = ""; //TODO: Design a more elegant solution to this.

    // Start is called before the first frame update
    void Start()
    {
        journalPages = new string[] { };
        setUIReferencesOnStart();
        this.setJournalText(journalText);
        updateText();
    }

    //Setup all the variables to elements of the UI if they are not already. Called from OnStart and also from Map_State_Storage_Script.loadMapState() 
    public void setUIReferencesOnStart()
    {
        if (leftText == null)
        {
            leftText = this.gameObject.transform.Find("Left Page Text").GetComponent<Text>();
            rightText = this.gameObject.transform.Find("Right Page Text").GetComponent<Text>();
            previousButton = this.gameObject.transform.Find("Previous Page Button").GetComponent<Button>();
            nextButton = this.gameObject.transform.Find("Next Page Button").GetComponent<Button>();
            closeJournalButton = this.gameObject.transform.Find("Close Button").GetComponent<Button>();
            toBattleButton = this.gameObject.transform.Find("To Battle Button").GetComponent<Button>();
            Debug.Log(" " + toBattleButton.name);
            leaveBattleSummaryButton = this.gameObject.transform.Find("Summary Exit Button").GetComponent<Button>();
            Debug.Log(" " + leaveBattleSummaryButton.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setJournalText(string textin)
    {
        this.journalText = textin;
        this.journalPages = this.journalText.Split(new string[] { "[PAGE BREAK]" }, System.StringSplitOptions.None);
        pageNumber = 0;
        updateText();
    }

    //Loads a scenario/battle into the journel and sets the journal text to that of the scenario.
    //Called from Map_Icon_Script and from JournalButton's OnCLick()(created in this.createJournalButtons). 
    public void setJournalScenario(string scenarioName)
    {
        leaveBattleSummaryButton.gameObject.SetActive(false);
        leaveBattleSummaryButton.onClick.RemoveAllListeners();

        if (scenarioName.StartsWith("BATTLE_"))
        {
            loadBattleScreen(scenarioName.Substring(7));
        }
        else if (scenarioName.StartsWith("POST_BATTLE_SCREEN"))
        {
            loadPostBattleScreen();
        }
        else //else load the scenario text
        {
            this.journalText = Scenarios_Database_Script.findScenario(scenarioName).journelText;
            this.journalText = processEffectsMarkup(this.journalText);
            this.journalText = moveExitButtonMarkupToEndOfText(this.journalText);
            this.journalPages = this.journalText.Split(new string[] { "[PAGE BREAK]" }, System.StringSplitOptions.None);
        }
        pageNumber = 0;
        updateText();
    }

    private void updateText()
    {
        deleteJournalButtons();

        leftText.text = this.journalPages[pageNumber];
        if (pageNumber + 1 < this.journalPages.Length)
        {
            rightText.text = this.journalPages[pageNumber + 1];
        }
        else
        {
            rightText.text = "";
        }

        if (pageNumber + 2 >= this.journalPages.Length)
        {
            nextButton.enabled = false;
        }
        else
        {
            nextButton.enabled = true;
        }

        if (pageNumber - 2 < 0)
        {
            previousButton.enabled = false;
        }
        else
        {
            previousButton.enabled = true;
        }

        if (leftText.text != "Badger\n")
        {
            createJournalButtons();
            createExitButtons();
        }
    }

    //Process markup for the effects of a scenario on the player's resources, this changes the users resources 
    //and compiles all of the effects into a list at the end of a the scenario text.
    //Uses the markup [EFFECT](Keyword){amount}: e.g. [EFFECT](ADD_AMMO){12}
    private string processEffectsMarkup(string journalTextin)
    {
        string text = journalTextin;
        if(text.Contains("[EFFECT]"))
        {
            text += "[PAGE BREAK] Effects:\n";
        }
        while(text.Contains("[EFFECT]"))
        {
            int startOfMarkup = text.IndexOf("[EFFECT]");
            int endOfMarkup = text.IndexOf("]", startOfMarkup); ;
            int startOfKeyword = text.IndexOf("(", startOfMarkup);
            int endOfKeyword = text.IndexOf(")", startOfMarkup);
            int startOfAmount = text.IndexOf("{", startOfMarkup);
            int endOfAmount = text.IndexOf("}", startOfMarkup);
            string effect = text.Substring(startOfKeyword + 1, endOfKeyword - (startOfKeyword + 1));
            string amountString = text.Substring(startOfAmount + 1, endOfAmount - (startOfAmount + 1));
            Debug.Log("amountString = " + amountString);
            int amount = int.Parse(amountString);
            string record = "";
            switch(effect)
            {
                case "ADD_AMMO": Player_Inventory_Script.addPlayersAmmo(amount); record = ("Gained " + amount + " Ammo"); break;
                case "SUB_AMMO": Player_Inventory_Script.addPlayersAmmo(-amount); record = ("Lost " + amount + " Ammo"); break;
                case "ADD_ENERGY": Player_Inventory_Script.addPlayersDarkEnergy(amount); record = ("Gained " + amount + " Dark Energy"); break;
                case "SUB_ENERGY": Player_Inventory_Script.addPlayersDarkEnergy(-amount); record = ("Lost " + amount + " Dark Energy"); break;
                default: Debug.LogWarning("Effects markup keyword: " + effect + " not recognised. Check your journal markup for the current scenario."); break;
            }
            text = text.Remove(startOfMarkup, (endOfAmount + 1) - startOfMarkup);
            text += ("\n" + record);
        }

        Debug.Log("Effects Markup successfully converted to text: " + text);
        return text;
    }

    //Moves the exit button markup so that it is always at the end of the text.
    private string moveExitButtonMarkupToEndOfText(string journalTextin)
    {
        string text = journalTextin;
        if (text.Contains("[EXIT BUTTON]"))
        {
            text = text.Replace("[EXIT BUTTON]", "");
            text += "\n[EXIT BUTTON]";
            Debug.Log("Moved [EXIT BUTTON] Markup to end of text. " + text);
        }
        return text;
    }

    //Replaces [BUTTON] markup with a button.
    //example markup: [BUTTON](ScenarioName){sample text to display on button}
    private void createJournalButtons()
    {
        //TODO: Add Error checking

        int numberOfButtonMarkups = leftText.text.Split(new string[] { "[BUTTON]" }, System.StringSplitOptions.None).Length - 1;
        for (int i = 0; i < numberOfButtonMarkups; i++)
        {
            //Parse Markup to collect: index of markup, name of the scenario the button leads to, text on the button. 
            int startOfButtonMarkup = leftText.text.IndexOf("[BUTTON]");
            int endOfButtonMarkup = leftText.text.IndexOf("[BUTTON]") + 7;
            int startOfScenarioName = leftText.text.IndexOf("(");
            int endOfScenarioName = leftText.text.IndexOf(")", startOfScenarioName);
            string scenarioName = leftText.text.Substring(startOfScenarioName + 1, endOfScenarioName - (startOfScenarioName + 1));
            int startOfButtonText = leftText.text.IndexOf("{", endOfScenarioName);
            int endOfButtonText = leftText.text.IndexOf("}", endOfScenarioName);
            string buttonText = leftText.text.Substring(startOfButtonText + 1, endOfButtonText - (startOfButtonText + 1));
            Debug.Log("ScenarioName found: " + scenarioName + " Button Text Found: " + buttonText);

            //Build Button
            GameObject button = Instantiate(pageButtonPrefab, leftText.gameObject.transform);
            button.transform.localPosition = getCharPositionOnScreen(leftText, startOfButtonMarkup);
            button.GetComponentInChildren<Text>().text = buttonText;
            button.GetComponent<Button>().onClick.AddListener(() => setJournalScenario(scenarioName));

            //Remove Markup Text, so next iteration gets the next markup
            Debug.Log("Stuffs = " + leftText.text.Substring(startOfButtonMarkup, (endOfButtonText + 1) - startOfButtonMarkup));
            leftText.text = leftText.text.Remove(startOfButtonMarkup, (endOfButtonText + 1) - startOfButtonMarkup);
            leftText.text = leftText.text.Insert(startOfButtonMarkup, buttonMarkupReplacementText);
        }

        numberOfButtonMarkups = rightText.text.Split(new string[] { "[BUTTON]" }, System.StringSplitOptions.None).Length - 1;
        for (int i = 0; i < numberOfButtonMarkups; i++)
        {
            //Parse Markup to collect: index of markup, name of the scenario the button leads to, text on the button.
            int startOfButtonMarkup = rightText.text.IndexOf("[BUTTON]");
            int endOfButtonMarkup = rightText.text.IndexOf("[BUTTON]") + 7;
            int startOfScenarioName = rightText.text.IndexOf("(");
            int endOfScenarioName = rightText.text.IndexOf(")", startOfScenarioName);
            string scenarioName = rightText.text.Substring(startOfScenarioName + 1, endOfScenarioName - (startOfScenarioName + 1));
            int startOfButtonText = rightText.text.IndexOf("{", endOfScenarioName);
            int endOfButtonText = rightText.text.IndexOf("}", endOfScenarioName);
            string buttonText = rightText.text.Substring(startOfButtonText + 1, endOfButtonText - (startOfButtonText + 1));
            Debug.Log("ScenarioName found: " + scenarioName + " Button Text Found: " + buttonText);

            //Build Button
            GameObject button = Instantiate(pageButtonPrefab, rightText.gameObject.transform);
            button.transform.localPosition = getCharPositionOnScreen(rightText, startOfButtonMarkup);
            button.GetComponentInChildren<Text>().text = buttonText;
            button.GetComponent<Button>().onClick.AddListener(() => setJournalScenario(scenarioName));

            //Remove Markup Text, so next iteration gets the next markup
            Debug.Log("Stuffs = " + rightText.text.Substring(startOfButtonMarkup, (endOfButtonText + 1) - startOfButtonMarkup));
            rightText.text = rightText.text.Remove(startOfButtonMarkup, (endOfButtonText + 1) - startOfButtonMarkup);
            rightText.text = rightText.text.Insert(startOfButtonMarkup, buttonMarkupReplacementText);
        }
    }

    //Replaces [EXIT BUTTON] markup with a button.
    //example markup: [EXIT BUTTON]
    private void createExitButtons()
    {
        //TODO: Add Error checking

        int numberOfButtonMarkups = leftText.text.Split(new string[] { "[EXIT BUTTON]" }, System.StringSplitOptions.None).Length - 1;
        for (int i = 0; i < numberOfButtonMarkups; i++)
        {
            //Parse Markup to collect: index of markup, name of the scenario the button leads to, text on the button. 
            int startOfButtonMarkup = leftText.text.IndexOf("[EXIT BUTTON]");
            int endOfButtonMarkup = leftText.text.IndexOf("[EXIT BUTTON]") + 12;

            string buttonText = "Move On";

            //Build Button
            GameObject button = Instantiate(pageButtonPrefab, leftText.gameObject.transform);
            button.transform.localPosition = getCharPositionOnScreen(leftText, startOfButtonMarkup);
            button.GetComponentInChildren<Text>().text = buttonText;
            button.GetComponent<Button>().onClick.AddListener(() => hideJournel());

            //Remove Markup Text, so next iteration gets the next markup
            Debug.Log("Stuffs = " + leftText.text.Substring(startOfButtonMarkup, (endOfButtonMarkup + 1) - startOfButtonMarkup));
            leftText.text = leftText.text.Remove(startOfButtonMarkup, (endOfButtonMarkup + 1) - startOfButtonMarkup);
            leftText.text = leftText.text.Insert(startOfButtonMarkup, buttonMarkupReplacementText);
        }

        numberOfButtonMarkups = rightText.text.Split(new string[] { "[EXIT BUTTON]" }, System.StringSplitOptions.None).Length - 1;
        for (int i = 0; i < numberOfButtonMarkups; i++)
        {
            //Parse Markup to collect: index of markup, name of the scenario the button leads to, text on the button. 
            int startOfButtonMarkup = rightText.text.IndexOf("[EXIT BUTTON]");
            int endOfButtonMarkup = rightText.text.IndexOf("[EXIT BUTTON]") + 12;

            string buttonText = "Move On";

            //Build Button
            GameObject button = Instantiate(pageButtonPrefab, rightText.gameObject.transform);
            button.transform.localPosition = getCharPositionOnScreen(rightText, startOfButtonMarkup);
            button.GetComponentInChildren<Text>().text = buttonText;
            button.GetComponent<Button>().onClick.AddListener(() => hideJournel());

            //Remove Markup Text, so next iteration gets the next markup
            Debug.Log("Stuffs = " + rightText.text.Substring(startOfButtonMarkup, (endOfButtonMarkup + 1) - startOfButtonMarkup));
            rightText.text = rightText.text.Remove(startOfButtonMarkup, (endOfButtonMarkup + 1) - startOfButtonMarkup);
            rightText.text = rightText.text.Insert(startOfButtonMarkup, buttonMarkupReplacementText);
        }
    }

    private void deleteJournalButtons()
    {
        List<GameObject> buttonsToRemove = new List<GameObject>();
        foreach (Transform journalInTextButton in rightText.transform)
        {
            if (journalInTextButton.tag == "Journal Button")
            {
                buttonsToRemove.Add(journalInTextButton.gameObject);
            }
        }

        foreach (Transform journalInTextButton in leftText.transform)
        {
            if (journalInTextButton.tag == "Journal Button")
            {
                buttonsToRemove.Add(journalInTextButton.gameObject);
            }
        }

        foreach (GameObject a in buttonsToRemove)
        {
            Destroy(a);
        }
    }


    //Returns the local-position of the character at character index in the text.
    private Vector3 getCharPositionOnScreen(Text textComp, int charIndex)
    {
        string text = textComp.text.ToString();
        Debug.Log("getting char position of " + text[charIndex] + " at index " + charIndex);

        if (charIndex >= text.Length)
        {
            Debug.LogWarning("Button charIndex of " + charIndex + " exceeds Text.Lenght");
            return new Vector3(0, 0, 0); //Should really return Null, but can't
        }

        //Recreate text in a textGenerator
        TextGenerator textGen = new TextGenerator(text.Length);
        Vector2 extents = textComp.gameObject.GetComponent<RectTransform>().rect.size;
        textGen.Populate(text, textComp.GetGenerationSettings(extents));
        Debug.Log("Text gen extends = " + textGen.rectExtents.position + " centre:" + textGen.rectExtents.center + " size: " + textGen.rectExtents.size);
        Debug.Log(text.Replace(" ", "").Replace("\n", "").Length + " chars (excluding whitespace and line breaks) with number of Vertexes = " + textGen.verts.Count);
        Debug.Log("First Vertex = " + textGen.verts[0].position);


        int newLine = text.Substring(0, charIndex).Split('\n').Length - 1; // new lines in rich text do not produce vertixes, so this is not necessary
        int whiteSpace = text.Substring(0, charIndex).Split(' ').Length - 1; // likewise white space in rich text does not produce vertixes.
        int indexOfTextQuad = charIndex * 4; //(charIndex * 4) + (newLine * 4) - 4;
        Debug.LogWarning("Bounds Detected: indexOfTextQuad: " + indexOfTextQuad + " and textGen.VertexCount: " + textGen.vertexCount);
        if (indexOfTextQuad < textGen.vertexCount)
        {
            Debug.Log("Verts = " + textGen.verts[indexOfTextQuad].position + " + " + textGen.verts[indexOfTextQuad + 1].position + " + " + textGen.verts[indexOfTextQuad + 2].position + " + " + textGen.verts[indexOfTextQuad + 3].position + " + ");
            
            //Get the average position of the 4 vertices that make up the letter
            Vector3 avgPos = (textGen.verts[indexOfTextQuad].position +
                textGen.verts[indexOfTextQuad + 1].position +
                textGen.verts[indexOfTextQuad + 2].position +
                textGen.verts[indexOfTextQuad + 3].position) / 4.0f;

            //Line breaks cause inconsistent shift to the right, lock the button to the left side(not an ideal solution)
            avgPos = new Vector3(16.5f, avgPos.y);
            //avgPos = new Vector3(avgPos.x, avgPos.y);

            Debug.Log("avgPos of char, post bounds adjustment = " + avgPos);
            //return textComp.transform.TransformPoint(avgPos);
            return avgPos;
        }
        else
        {
            //5, 34, 101, 168, 229
            //  29   67, 67, 61


            Debug.LogError("Out of text bound: indexOfTextQuad: " + indexOfTextQuad + " exceeds textGen.VertexCount: " + textGen.vertexCount);
            return new Vector3(0, 0, 0); //Should really return Null, but can't
        }
    }

    public void nextPage()
    {
        this.pageNumber += 2; //+2 because the journal displays 2 pages at a time
        this.updateText();
    }

    public void previousPage()
    {
        this.pageNumber -= 2; //-2 because the journal displays 2 pages at a time
        this.updateText();
    }

    //Loads the battle screen and populates it with details(number of enemies and composition) about the horde with the matching hordeName.
    //Battle Encounter Names use the format: BATTLE_hordeName[_]postBattleScenarioName, while this method receives hordeName[_]postBattleScenarioName
    //Also loads the "To Battle" button that will take the user to the battle scene.
    public void loadBattleScreen(string hordeName_postBattleScenario)
    {
        this.journalPages = this.journalText.Split(new string[] { "[PAGE BREAK]" }, System.StringSplitOptions.None);
        string hordeName = hordeName_postBattleScenario.Split(new string[1]{ "[_]" }, System.StringSplitOptions.None)[0];
        string postScenarioName = "";
        if (hordeName_postBattleScenario.Contains("[_]"))
        {
            postScenarioName = hordeName_postBattleScenario.Split(new string[1] { "[_]" }, System.StringSplitOptions.None)[1];
        }
        
        Enemy_Spawning_Script.Horde horde = Enemy_Spawning_Script.findHorde(hordeName);
        Enemy_Spawning_Script.setCurrentPostBattleScenarioName(postScenarioName);
        Enemy_Spawning_Script.resetBattleStatVariables();
        string battleText = this.preBattleText;
        battleText = battleText.Replace("[SIZE]", "" + horde.getTotalSize());

        string compoText = "";
        foreach(Enemy_Spawning_Script.Wave aWave in horde.waves)
        {
            foreach(Enemy_Spawning_Script.EnemyPool aPool in aWave.enemyPools)
            {
                if(!compoText.Contains(aPool.enemyPrefab.name))
                {
                    compoText += (aPool.enemyPrefab.name + "\n");
                }
            }
        }

        battleText = battleText.Replace("[COMPOSITION]", compoText);
        this.setJournalText(battleText);
        currentBattleName = hordeName;
        toBattleButton.gameObject.SetActive(true);
    }

    //Loads the post battle screen and populates it with details about the previous battle.
    //Also loads the button that will take the user to the scenario that was set to occur after the battle.
    public void loadPostBattleScreen()
    {
        this.journalPages = this.journalText.Split(new string[] { "[PAGE BREAK]" }, System.StringSplitOptions.None);
        string battleText = this.postBattleText;
        string postBattleScenarioName = Enemy_Spawning_Script.getCurrentPostBattleScenarioName();
        battleText = battleText.Replace("[KILLS]", "Enemies Slain: " + Enemy_Spawning_Script.getBattleStat_Kills());
        battleText = battleText.Replace("[ENERGY SPENT]", "Energy Spent: " + Enemy_Spawning_Script.getBattleStat_EnergySpent());
        battleText = battleText.Replace("[ENERGY GAINED]", "Energy Gained: " + Enemy_Spawning_Script.getBattleStat_EnergyEarned());
        battleText = battleText.Replace("[AMMO SPENT]", "Ammo Expended: " + Enemy_Spawning_Script.getBattleStat_AmmoSpent());
        battleText = battleText.Replace("[DAMAGE TAKEN]", "Necromancer Damage Taken: " + Enemy_Spawning_Script.getBattleStat_NecromancerDamageTaken());

        string minionText = "";
        if (Enemy_Spawning_Script.getBattleStat_NewMinions().Count > 0)
        {
            minionText = "New Minions: ";
            foreach(string aMinion in Enemy_Spawning_Script.getBattleStat_NewMinions())
            {
                minionText += "\n" + aMinion;
            }
            
        }
        battleText = battleText.Replace("[MINIONS GAINED]", minionText);

        this.setJournalText(battleText);
        leaveBattleSummaryButton.gameObject.SetActive(true);
        if (postBattleScenarioName == "")
        {
            leaveBattleSummaryButton.onClick.AddListener(() => hideJournel());
        }
        else
        {
            leaveBattleSummaryButton.onClick.AddListener(() => setJournalScenario(postBattleScenarioName));
        }
        //currentBattleName = hordeName;
        //toBattleButton.gameObject.SetActive(true);
    }

    public void showJournel()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        leftText.enabled = true;
        rightText.enabled = true;
        previousButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        closeJournalButton.gameObject.SetActive(true);

        updateText();
    }

    public void hideJournel()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        leftText.enabled = false;
        rightText.enabled = false;
        previousButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        closeJournalButton.gameObject.SetActive(false);
        toBattleButton.gameObject.SetActive(false);
        leaveBattleSummaryButton.gameObject.SetActive(false);
        leaveBattleSummaryButton.onClick.RemoveAllListeners();

        foreach (Transform journalInTextButton in rightText.transform)
        {
            if(journalInTextButton.tag == "Journal Button")
            {
                journalInTextButton.gameObject.SetActive(false);
            }
        }

        foreach (Transform journalInTextButton in leftText.transform)
        {
            if (journalInTextButton.tag == "Journal Button")
            {
                journalInTextButton.gameObject.SetActive(false);
            }
        }
    }

    public void goToBattleScene()
    {
        Debug.LogWarning("Loading Battle Scene with horde: " + currentBattleName);
        Map_State_Storage_Script.instance.saveMapState();
        Enemy_Spawning_Script.setHorde(currentBattleName);
        SceneManager.LoadSceneAsync("Battle Test Rework Scene", LoadSceneMode.Single);
    }
}
