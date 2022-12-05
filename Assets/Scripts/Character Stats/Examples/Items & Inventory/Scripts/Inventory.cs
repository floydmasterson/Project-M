using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : ItemContainer
{
    [TabGroup("Starting Inventory")]
    [PropertyOrder(-1)]
    [TableList(AlwaysExpanded = true, NumberOfItemsPerPage = 30, ShowPaging = true), HideLabel]
    [SerializeField] public Item[] startingItems;
    [TabGroup("Setup")]
    [SerializeField] protected Transform itemsParent;

    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);


    }

    protected override void Start()
    {
        base.Start();
        SetStartingItems();
    }

    private void SetStartingItems()
    {

        Clear();
        foreach (Item item in startingItems)
        {
            if (item != null)
                item.name = item.name.Replace("(Clone", "").Trim();
            if (item != null)
                AddItem(item.GetCopy());
        }

    }
}
