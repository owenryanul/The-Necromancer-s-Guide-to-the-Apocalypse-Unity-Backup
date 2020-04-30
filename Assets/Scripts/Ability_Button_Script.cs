using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ability_Database = Ability_Database_Script;

//Note: OnClick logic is handled in the inspector, not here. Those function are likely in User_Input_Script

public class Ability_Button_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int abilitySlot;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        string abilityTooltip = Ability_Database.getAbilityTooltip(User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityIDforSlot(abilitySlot));
        Tooltip_Script.displayTooltip(abilityTooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip_Script.hideTooltip();
    }

    private void showButton()
    {
        this.gameObject.GetComponent<Image>().enabled = true;
        this.GetComponentInChildren<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponentInChildren<Text>().enabled = true;
        this.gameObject.transform.Find("Ability Icon").GetComponent<Image>().enabled = true;
        setIconImage();
        showPassiveBorderIfAppropriate();
    }

    private void hideButton()
    {
        this.gameObject.GetComponent<Image>().enabled = false;
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponentInChildren<Text>().enabled = false;
        this.gameObject.transform.Find("Ability Icon").GetComponent<Image>().enabled = false;
        this.gameObject.transform.Find("Passive Fill").GetComponent<Image>().enabled = false;
    }

    private void updateCooldownDial()
    {
        float currentCooldown = User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityCooldown(abilitySlot);
        float maxCooldown = Ability_Database.getCooldown(User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityIDforSlot(abilitySlot));
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

    private void setIconImage()
    {
        Sprite icon = Ability_Database.getAbilityIcon(User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityIDforSlot(abilitySlot));
        this.gameObject.transform.Find("Ability Icon").GetComponent<Image>().sprite = icon;
    }

    private void showPassiveBorderIfAppropriate()
    {
        if(Ability_Database.getAbilityType(User_Input_Script.currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityIDforSlot(abilitySlot)) == Ability_Database_Script.AbilityType.passive)
        {
            this.gameObject.transform.Find("Passive Fill").GetComponent<Image>().enabled = true;
        }
        else
        {
            this.gameObject.transform.Find("Passive Fill").GetComponent<Image>().enabled = false;
        }
    }
}
