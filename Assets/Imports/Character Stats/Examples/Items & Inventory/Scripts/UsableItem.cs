using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Usable Item")]
public class UsableItem : Item
{
    public bool IsConsumable;

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

    public override string GetDescription()
    {
        sb.Length = 0;
        foreach (UseableItemEffect effect in Effects)
        {
            sb.AppendLine(effect.GetDescription());
        }
        return sb.ToString();
    }
}
