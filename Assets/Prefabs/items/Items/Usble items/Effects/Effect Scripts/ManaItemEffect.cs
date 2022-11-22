using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Mana Gain")]
public class ManaItemEffect : UseableItemEffect
{
    public int amount;
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (PlayerUi.Instance.target.gameObject.GetComponent<MagicController>() != null)
            PlayerUi.Instance.target.gameObject.GetComponent<MagicController>().CurrMana += amount;
        else
        {
            Debug.Log("You dont have mana");
        }
    }

    public override string GetDescription()
    {
       return "Recover " + amount + " mana.";
    }
}
