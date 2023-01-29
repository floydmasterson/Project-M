using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Usable Item")]
public class UsableItem : Item
{
    [InlineEditor(Expanded = true, ObjectFieldMode = InlineEditorObjectFieldModes.Boxed)]
    [TableColumnWidth(190, resizable: false)]
    public List<UseableItemEffect> Effects;
    

    public virtual void Use(Character character)
    {
        foreach (UseableItemEffect effect in Effects)
        {
            effect.ExecuteEffect(this, character);
        }
    }

    public override string GetItemType()
    {
        return IsConsumable ? "Consumable" : "Usable";
    }
   
   public bool UseableCheck(PlayerUi player)
    {
        foreach (UseableItemEffect effect in Effects)
        {
           if (effect.canBeUsed() && !player.Sick)
                return true;
        }
        return false;
    }

    public override string GetDescription(bool sell)
    {
        sb.Length = 0;
        foreach (UseableItemEffect effect in Effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        if (sell)
            sb.Append("Sell Value: " + GetSellValue() + "G");
        else if (!sell)
            sb.Append("Buy Price: " + GetBuyValue() + "G");
        return sb.ToString();
    }
}
