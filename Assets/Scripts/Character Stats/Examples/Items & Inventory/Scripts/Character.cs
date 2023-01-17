using Kryz.CharacterStats;
using Kryz.CharacterStats.Examples;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public static Character Instance;

    [TabGroup("Stats")]
    public CharacterStat Strength;
    [TabGroup("Stats")]
    public CharacterStat Agility;
    [TabGroup("Stats")]
    public CharacterStat Intelligence;
    [TabGroup("Stats")]
    public CharacterStat Vitality;

    [TabGroup("Inventory")]
    [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
    public Inventory Inventory;
    [TabGroup("Setup")]
    public EquipmentPanel EquipmentPanel;

    [TabGroup("Setup")]
    [SerializeField] public StatPanel statPanel;
    [TabGroup("Setup")]
    [SerializeField] ItemTooltip itemTooltip;
    [TabGroup("Setup")]
    [SerializeField] Image draggableItem;
    [TabGroup("Quick Slot")]
    public Image currentQuickItem;
    [TabGroup("Quick Slot")]
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
            if (usableItem.UseableCheck())
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
            //draggableItem.transform.position = GamepadCursor.Instance.playerInput.currentControlScheme == "Keyboard" ? Input.mousePosition : GamepadCursor.Instance.cursorTransfom.position;
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
        {
            if (dropEquipItem != null) dropEquipItem.Unequip(this);
            if (dragEquipItem != null) dragEquipItem.Equip(this);
        }
        if (dragItemSlot is EquipmentSlot)
        {
            if (dragEquipItem != null) dragEquipItem.Unequip(this);
            if (dropEquipItem != null) dropEquipItem.Equip(this);
        }
        if (dropItemSlot is QuickSlot)
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
    public void Equip(EquippableItem item)
    {
        if (Inventory.RemoveItem(item))
        {
            EquippableItem previousItem;
            if (EquipmentPanel.AddItem(item, out previousItem))
            {
                if (previousItem != null)
                {
                    Inventory.AddItem(previousItem, 1);
                    previousItem.Unequip(this);
                }
                item.Equip(this);
            }
            else
            {
                Inventory.AddItem(item, 1);
            }
            statPanel.UpdateStatValues();
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (Inventory.CanAddItem(item) && EquipmentPanel.RemoveItem(item))
        {
            item.Unequip(this);
            statPanel.UpdateStatValues();
            Inventory.AddItem(item, 1);
        }
    }

    private ItemContainer openItemContainer;

    private void TransferToItemContainer(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.Item;
        if (item != null && openItemContainer.CanAddItem(item))
        {
            Inventory.RemoveItem(item);
            openItemContainer.AddItem(item, 0);
        }
    }

    private void TransferToInventory(BaseItemSlot itemSlot)
    {
        Item item = itemSlot.Item;
        if (item != null && Inventory.CanAddItem(item))
        {
            openItemContainer.RemoveItem(item);
            if (item.IsConsumable)
                Inventory.AddItem(item, 0);
            else if (!item.IsConsumable)
                Inventory.AddItem(item, 1);
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
