using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/ Max Mana Gain")]
public class MaxManaItemEffect : UseableItemEffect
{ 
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (PlayerUi.Instance.target.gameObject.GetComponent<MagicController>() != null)
            PlayerUi.Instance.target.gameObject.GetComponent<MagicController>().CurrMana = PlayerUi.Instance.target.gameObject.GetComponent<MagicController>().MaxMana;
        else
        {
            Debug.Log("You dont have mana");
        }
    }

    public override string GetDescription()
    {
        return "Fully recover all mana.";
    }
}
