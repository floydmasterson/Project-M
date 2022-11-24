using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Mana Gain")]
public class ManaItemEffect : UseableItemEffect
{
    public int amount;
    MagicController MC;


    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (MC != null)
            MC.CurrMana += amount;
        else
        {
            Debug.Log("You dont have mana");
        }
    }

    public override string GetDescription()
    {
       return "Recover " + amount + " mana.";
    }
    public override bool canBeUsed()
    {
        MC = PlayerUi.Instance.target.gameObject.GetComponent<MagicController>();
        if (MC != null)
        {
            if (MC.CurrMana == MC.MaxMana)
                return false;
            else
            {
                return true;
            }
        }
        return false;
    }
}
