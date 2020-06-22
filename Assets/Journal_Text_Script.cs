using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Journal_Text_Script : MonoBehaviour
{
    [TextArea]
    [Tooltip("Use [PAGE BREAK] to seperate pages")]
    public string journalText;

    public GameObject pageButtonPrefab;

    private string[] journalPages;
    private int pageNumber;

    private Text leftText;
    private Text rightText;

    private Button previousButton;
    private Button nextButton;

    private Button closeJournalButton;

    private string buttonMarkupReplacementText = ""; //TODO: Design a more elegant solution to this.

    // Start is called before the first frame update
    void Start()
    {
        journalPages = new string[] { };
        leftText = this.gameObject.transform.Find("Left Page Text").GetComponent<Text>();
        rightText = this.gameObject.transform.Find("Right Page Text").GetComponent<Text>();
        previousButton = this.gameObject.transform.Find("Previous Page Button").GetComponent<Button>();
        nextButton = this.gameObject.transform.Find("Next Page Button").GetComponent<Button>();
        closeJournalButton = this.gameObject.transform.Find("Close Button").GetComponent<Button>();
        this.setJournalText(journalText);
        updateText();
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

    public void setJournalScenario(string scenarioName)
    {
        this.journalText = Scenarios_Database_Script.findScenario(scenarioName).journelText;
        this.journalPages = this.journalText.Split(new string[] { "[PAGE BREAK]" }, System.StringSplitOptions.None);
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
        }
    }

    //Replaces [BUTTON] markup with a button.
    //example markup: [BUTTON](ScenarioName){sample text to display on button}
    private void createJournalButtons()
    {
        Debug.Log("Text = " + leftText.text);
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


            Debug.LogError("Out of text bound");
            return new Vector3(0, 0, 0); //Should really return Null, but can't
        }
    }

    public void nextPage()
    {
        this.pageNumber += 2;
        this.updateText();
    }

    public void previousPage()
    {
        this.pageNumber -= 2;
        this.updateText();
    }

    public void loadScenario(string name)
    {
        Scenario scenarioToLoad = Scenarios_Database_Script.findScenario(name);
        this.setJournalText(scenarioToLoad.journelText);
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

        foreach(Transform journalInTextButton in rightText.transform)
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
}
