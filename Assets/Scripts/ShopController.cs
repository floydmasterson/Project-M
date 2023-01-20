using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ShopController : ItemContainer
{
    [TabGroup("Setup"), SerializeField]
    Transform itemsParent;
    [TabGroup("Setup"), SerializeField]
    Inventory inventory;
    [TabGroup("Setup"), SerializeField]
    Item Gold;
    [TabGroup("Setup"), SerializeField]
    TextMeshProUGUI sellText;
    public bool isOpen;
    int SellAmount;


    Character currentPlayer;

    private void Awake()
    {
        inventory.OnRightClickEvent += slot => UpdateSellAmount();
        inventory.OnBeginDragEvent += slot => UpdateSellAmount();
        inventory.OnEndDragEvent += slot => UpdateSellAmount();
        inventory.OnDragEvent += slot => UpdateSellAmount();
        inventory.OnDropEvent += slot => UpdateSellAmount();
    }
    protected override void OnValidate()
    {
        if (itemsParent != null)
            itemsParent.GetComponentsInChildren(includeInactive: true, result: ItemSlots);
    }
    public void Open(Character character)
    {
        character.OpenItemContainer(this);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        currentPlayer = character;
        Inventory inventory = currentPlayer.Inventory;

        inventory.OnDropEvent += slot => UpdateSellAmount();
        inventory.OnEndDragEvent += slot => UpdateSellAmount();
        inventory.OnRightClickEvent += slot => UpdateSellAmount();

        isOpen = true;
    }
    public void Close(Character character)
    {
        character.CloseItemContainer(this);
        Inventory inventory = currentPlayer.Inventory;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        inventory.OnDropEvent -= slot => UpdateSellAmount();
        inventory.OnEndDragEvent -= slot => UpdateSellAmount();
        inventory.OnRightClickEvent -= slot => UpdateSellAmount();
        EmptyCheck();
        currentPlayer = null;
        isOpen = false;
    }
    void EmptyCheck()
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item != null && ItemSlots[i].Amount == 1)
                AddItem(ItemSlots[i].Item, 1);
            else if (ItemSlots[i].Item != null && ItemSlots[i].Amount > 1)
                AddItem(ItemSlots[i].Item, ItemSlots[i].Amount);
        }
    }
    public void Sell()
    {
        int sellAmount = 0;
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item != null && ItemSlots[i].Amount == 1)
                sellAmount += SellCost(ItemSlots[i].Item);
            else if (ItemSlots[i].Item != null && ItemSlots[i].Amount > 1)
                sellAmount += SellCost(ItemSlots[i].Item) * ItemSlots[i].Amount;
        }
        for (int i = 0; i < sellAmount; i++)
        {
            currentPlayer.Inventory.AddItem(Gold, 1);
        }
        Clear();
        UpdateSellAmount();
    }
    int SellCost(Item item)
    {
        int sellAmount = 0;
        sellAmount += (item.cost - (int)(item.cost * .2f));
        return sellAmount;
    }

    void UpdateSellAmount()
    {
        Debug.Log("update");
        SellAmount = 0;
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item != null && ItemSlots[i].Amount == 1 && ItemSlots[i].Item != Gold)
                SellAmount += SellCost(ItemSlots[i].Item);
            else if (ItemSlots[i].Item != null && ItemSlots[i].Amount > 1 && ItemSlots[i].Item != Gold)
                SellAmount += SellCost(ItemSlots[i].Item) * ItemSlots[i].Amount;
        }
        sellText.text = "Sell Value: " + SellAmount.ToString() + "G";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            player.shop = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            player.shop = null;
        }
    }
}
