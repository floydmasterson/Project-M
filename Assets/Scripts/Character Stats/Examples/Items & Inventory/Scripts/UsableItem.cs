using Sirenix.OdinInspector;
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
   
   public bool UseableCheck()
    {
        foreach (UseableItemEffect effect in Effects)
        {
           if (effect.canBeUsed())
                return true;
        }
        return false;
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        foreach (UseableItemEffect effect in Effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        sb.Append("Sell Value: " + GetSellValue() + "G");
        return sb.ToString();
    }
}
