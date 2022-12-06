using UnityEngine;
public class QuickSlot : ItemSlot
{
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    public override bool CanReceiveItem(Item item)
    {
        if (item == null)
        {
            return true;
        }
        UsableItem usableItem = item as UsableItem;
        return usableItem != null && usableItem.IsConsumable;

    }
}

