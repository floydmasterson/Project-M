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
    Character character;

    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);
    }
    private void Awake()
    {
        character = GetComponentInParent<Character>();
        SetStartingItems();     
    }

    protected override void Start()
    {
        base.Start();
    }
    private void SetStartingItems()
    {
        foreach (Item item in startingItems)
        {
            if (item != null)
            {
                item.name = item.name.Replace("(Clone", "").Trim();
                AddItem(item.GetCopy(), 0);
            }
        }
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item is EquippableItem)
                character.InventoryRightClick(ItemSlots[i]);
        }       

    }

}
