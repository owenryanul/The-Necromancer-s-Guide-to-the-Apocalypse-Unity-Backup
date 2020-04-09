using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability_Button_Script : MonoBehaviour
{
    public int abilitySlot;
    private Ability_Database_Script Ability_Database;

    // Start is called before the first frame update
    void Start()
    {
        Ability_Database = GameObject.FindGameObjectWithTag("Level Script Container").GetComponent<Ability_Database_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        if (User_Input_Script.currentlySelectedMinion != null && !User_Input_Script.currentlySelectedMinion.CompareTag("Necromancer"))
        {
            showButton();
            updateCooldownDial();
        }
        else
        {
            hideButton();
        }
    }

    private void showButton()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        this.GetComponentInChildren<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponentInChildren<Text>().enabled = true;
    }

    private void hideButton()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponentInChildren<Text>().enabled = false;
    }

    private void updateCooldownDial()
    {
        float currentCooldown = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>().getAbilityCooldown(abilitySlot);
        float maxCooldown = Ability_Database.getCooldown(User_Input_Script.currentlySelectedMinion.GetComponent<Minion_Movement_Script>().getAbility(abilitySlot));
        if (currentCooldown > 0)
        {
            this.GetComponentInChildren<SpriteRenderer>().enabled = true;
            this.GetComponentInChildren<SpriteMask>().alphaCutoff = 1.0f - (1.0f * (currentCooldown / maxCooldown));
        }
        else
        {
            this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        
    }
}
