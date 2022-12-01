using Kryz.CharacterStats;
using Kryz.CharacterStats.Examples;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public static Character Instance;

    [Header("Stats")]
    public CharacterStat Strength;
    public CharacterStat Agility;
    public CharacterStat Intelligence;
    public CharacterStat Vitality;

    [Header("Public")]
    public Inventory Inventory;
    public EquipmentPanel EquipmentPanel;

    [Header("Serialize Field")]
    [SerializeField] public StatPanel statPanel;
    [SerializeField] ItemTooltip itemTooltip;
    [SerializeField] Image draggableItem;
    public Image currentQuickItem;
    public int currentQuickItemAmount;

    private BaseItemSlot dragItemSlot;

    private void OnValidate()
    {
        if (itemTooltip == null)
            itemTooltip = FindObjectOfType<ItemTooltip>(true);
    }

    private void Awake()    
    {
        Instance = this;
        statPanel.SetStats(Strength, Agility, Intelligence, Vitality);
       

        // Setup Events:
        // Right Click
        Inventory.OnRightClickEvent += InventoryRightClick;
        EquipmentPanel.OnRightClickEvent += EquipmentPanelRightClick;
        // Pointer Enter
        Inventory.OnPointerEnterEvent += ShowTooltip;
        EquipmentPanel.OnPointerEnterEvent += ShowTooltip;

        // Pointer Exit
        Inventory.OnPointerExitEvent += HideTooltip;
        EquipmentPanel.OnPointerExitEvent += HideTooltip;

        // Begin Drag
        Inventory.OnBeginDragEvent += BeginDrag;
        EquipmentPanel.OnBeginDragEvent += BeginDrag;
        // End Drag
        Inventory.OnEndDragEvent += EndDrag;
        EquipmentPanel.OnEndDragEvent += EndDrag;
        // Drag
        Inventory.OnDragEvent += Drag;
        EquipmentPanel.OnDragEvent += Drag;
        // Drop
        Inventory.OnDropEvent += Drop;
        EquipmentPanel.OnDropEvent += Drop;
        //dropItemArea.OnDropEvent += DropItemOutsideUI;
    }


    private void Start()
    {
        statPanel.UpdateStatValues();
    }


    private void InventoryRightClick(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item is EquippableItem)
        {
            Equip((EquippableItem)itemSlot.Item);
        }
        else if (itemSlot.Item is UsableItem)
        {
            UsableItem usableItem = (UsableItem)itemSlot.Item;
            if(usableItem.UseableCheck())
            {
            usableItem.Use(this);

            if (usableItem.IsConsumable)
            {
                itemSlot.Amount--;
                usableItem.Destroy();
            }
            }
        }
    }

    private void EquipmentPanelRightClick(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item is EquippableItem)
        {
            Unequip((EquippableItem)itemSlot.Item);
        }
    }

    private void ShowTooltip(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            itemTooltip.ShowTooltip(itemSlot.Item);
        }
    }

    private void HideTooltip(BaseItemSlot itemSlot)
    {
        if (itemTooltip.gameObject.activeSelf)
        {
            itemTooltip.HideTooltip();
        }
    }

    private void BeginDrag(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            dragItemSlot = itemSlot;
            draggableItem.sprite = itemSlot.Item.Icon;
            draggableItem.transform.position = Input.mousePosition;
            draggableItem.gameObject.SetActive(true);
        }
    }

    private void Drag(BaseItemSlot itemSlot)
    {
        draggableItem.transform.position = Input.mousePosition;
    }

    private void EndDrag(BaseItemSlot itemSlot)
    {
        dragItemSlot = null;
        draggableItem.gameObject.SetActive(false);
    }

    private void Drop(BaseItemSlot dropItemSlot)
    {
        if (dragItemSlot == null) return;

        if (dropItemSlot.CanAddStack(dragItemSlot.Item))
        {
            AddStacks(dropItemSlot);
        }
        else if (dropItemSlot.CanReceiveItem(dragItemSlot.Item) && dragItemSlot.CanReceiveItem(dropItemSlot.Item))
        {
            SwapItems(dropItemSlot);
        }
    }

    private void AddStacks(BaseItemSlot dropItemSlot)
    {
        int numAddableStacks = dropItemSlot.Item.MaximumStacks - dropItemSlot.Amount;
        int stacksToAdd = Mathf.Min(numAddableStacks, dragItemSlot.Amount);

        dropItemSlot.Amount += stacksToAdd;
        dragItemSlot.Amount -= stacksToAdd;
    }

    private void SwapItems(BaseItemSlot dropItemSlot)
    {
        EquippableItem dragEquipItem = dragItemSlot.Item as EquippableItem;
        EquippableItem dropEquipItem = dropItemSlot.Item as EquippableItem;
        UsableItem dropUseItem = dropItemSlot.Item as UsableItem;
        if (dropItemSlot is EquipmentSlot)
            if (dropItemSlot is EquipmentSlot)
        {
            if (dragEquipItem != null) dragEquipItem.Equip(this);
            if (dropEquipItem != null) dropEquipItem.Unequip(this);
        }
        if (dragItemSlot is EquipmentSlot)
        {
            if (dragEquipItem != null) dragEquipItem.Unequip(this);
            if (dropEquipItem != null) dropEquipItem.Equip(this);
        }
        if(dropItemSlot is QuickSlot)
        {
            if (dropUseItem != null) currentQuickItem.sprite = dropUseItem.Icon;
        }
        statPanel.UpdateStatValues();

        Item draggedItem = dragItemSlot.Item;
        int draggedItemAmount = dragItemSlot.Amount;

        dragItemSlot.Item = dropItemSlot.Item;
        dragItemSlot.Amount = dropItemSlot.Amount;
       

        dropItemSlot.Item = draggedItem;
        dropItemSlot.Amount = draggedItemAmount;
    }

    //private void DropItemOutsideUI()
    //{
    //    if (dragItemSlot == null) return;

    //    reallyDropItemDialog.Show();
    //    BaseItemSlot slot = dragItemSlot;
    //    reallyDropItemDialog.OnYesEvent += () => DestroyItemInSlot(slot);
    //}

    private void DestroyItemInSlot(BaseItemSlot itemSlot)
    {
        // If the item is equiped, unequip first
        if (itemSlot is EquipmentSlot)
        {
            EquippableItem equippableItem = (EquippableItem)itemSlot.Item;
            equippableItem.Unequip(this);
        }

        itemSlot.Item.Destroy();
        itemSlot.Item = null;
    }

    public void Equip(EquippableItem item)
    {
        if (Inventory.RemoveItem(item))
        {
            EquippableItem previousItem;
            if (EquipmentPanel.AddItem(item, out previousItem))
            {
                if (previousItem != null)
                {
                    Inventory.AddItem(previousItem);
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                item.Equip(this);
                statPanel.UpdateStatValues();
            }
            else
            {
                Inventory.AddItem(item);
            }
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (Inventory.CanAddItem(item) && EquipmentPanel.RemoveItem(item))
        {
            item.Unequip(this);
            statPanel.UpdateStatValues();
            Inventory.AddItem(item);
        }
    }

    private ItemContainer openItemContainer;

    private void TransferToItemContainer(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.Item;
        if (item != null && openItemContainer.CanAddItem(item))
        {
            Inventory.RemoveItem(item);
            openItemContainer.AddItem(item);
        }
    }

    private void TransferToInventory(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.Item;
        if (item != null && Inventory.CanAddItem(item))
        {
            openItemContainer.RemoveItem(item);
            Inventory.AddItem(item);
        }
    }

    public void OpenItemContainer(ItemContainer itemContainer)
    {
        openItemContainer = itemContainer;

        Inventory.OnRightClickEvent -= InventoryRightClick;
        Inventory.OnRightClickEvent += TransferToItemContainer;

        itemContainer.OnRightClickEvent += TransferToInventory;

        itemContainer.OnPointerEnterEvent += ShowTooltip;
        itemContainer.OnPointerExitEvent += HideTooltip;
        itemContainer.OnBeginDragEvent += BeginDrag;
        itemContainer.OnEndDragEvent += EndDrag;
        itemContainer.OnDragEvent += Drag;
        itemContainer.OnDropEvent += Drop;
    }

    public void CloseItemContainer(ItemContainer itemContainer)
    {
        openItemContainer = null;

        Inventory.OnRightClickEvent += InventoryRightClick;
        Inventory.OnRightClickEvent -= TransferToItemContainer;

        itemContainer.OnRightClickEvent -= TransferToInventory;

        itemContainer.OnPointerEnterEvent -= ShowTooltip;
        itemContainer.OnPointerExitEvent -= HideTooltip;
        itemContainer.OnBeginDragEvent -= BeginDrag;
        itemContainer.OnEndDragEvent -= EndDrag;
        itemContainer.OnDragEvent -= Drag;
        itemContainer.OnDropEvent -= Drop;
    }


    public void UpdateStatValues()
    {
        statPanel.UpdateStatValues();
    }
}
