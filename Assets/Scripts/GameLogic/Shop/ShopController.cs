using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : ItemContainer
{
    public bool isOpen;
    [TabGroup("Sell Setup"), SerializeField]
    Inventory SellInventory;
    [TabGroup("Buy Setup"), SerializeField]
    Inventory BuyInventory;
    [TabGroup("Sell Setup"), SerializeField]
    Item Gold;
    [TabGroup("Sell Setup"), SerializeField]
    TextMeshProUGUI sellText;
    [TabGroup("Buy Setup"), SerializeField]
    List<Item> itemsForSale = new List<Item>();
    [TabGroup("Buy Setup"), SerializeField]
    GameObject ShopItemPrefab;
    [TabGroup("Buy Setup"), SerializeField]
    Transform scrollArea;
    [TabGroup("Buy Setup"), SerializeField]
    Transform inCart;
    [TabGroup("Buy Setup"), SerializeField]
    TextMeshProUGUI BuyText;
    [TabGroup("Buy Setup"), SerializeField]
    TextMeshProUGUI StartingGoldText;
    [TabGroup("Buy Setup"), SerializeField]
    TextMeshProUGUI RunningGoldText;
    public event Action<Item> RemovedFromCart;
    int playerStartingGold;
    int SellAmount;
    int BuyAmount;

    InventoryUi currentPlayerUI;
    Inventory currentPlayer;
    Character currentCharacter;


    public void Open(Character character)
    {
        SellInventory.Clear();
        UpdateSellAmount();
        currentCharacter = character;
        currentPlayer = currentCharacter.Inventory;
        currentPlayerUI = currentCharacter.GetComponent<InventoryUi>();
        currentCharacter.OpenItemContainer(SellInventory);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        #region Sell Events On
        SellInventory.OnRightClickEvent += slot => UpdateSellAmount();
        SellInventory.OnBeginDragEvent += slot => UpdateSellAmount();
        SellInventory.OnEndDragEvent += slot => UpdateSellAmount();
        SellInventory.OnDragEvent += slot => UpdateSellAmount();
        SellInventory.OnDropEvent += slot => UpdateSellAmount();
        currentPlayer.OnDropEvent += slot => UpdateSellAmount();
        currentPlayer.OnEndDragEvent += slot => UpdateSellAmount();
        currentPlayer.OnRightClickEvent += slot => UpdateSellAmount();
        #endregion

        isOpen = true;
    }
    public void Close(Character character)
    {
        if (gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
            character.CloseItemContainer(SellInventory);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        #region Sell Events Off
        SellInventory.OnRightClickEvent -= slot => UpdateSellAmount();
        SellInventory.OnBeginDragEvent -= slot => UpdateSellAmount();
        SellInventory.OnEndDragEvent -= slot => UpdateSellAmount();
        SellInventory.OnDragEvent -= slot => UpdateSellAmount();
        SellInventory.OnDropEvent -= slot => UpdateSellAmount();
        currentPlayer.OnDropEvent -= slot => UpdateSellAmount();
        currentPlayer.OnEndDragEvent -= slot => UpdateSellAmount();
        currentPlayer.OnRightClickEvent -= slot => UpdateSellAmount();
        #endregion
        EmptyCheck(SellInventory);
        currentPlayer = null;
        currentPlayerUI = null;
        currentCharacter = null;
        isOpen = false;
    }
    public void swapMenu()
    {
        if (gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            currentCharacter.CloseItemContainer(SellInventory);
            #region Sell Events Off
            SellInventory.OnRightClickEvent -= slot => UpdateSellAmount();
            SellInventory.OnBeginDragEvent -= slot => UpdateSellAmount();
            SellInventory.OnEndDragEvent -= slot => UpdateSellAmount();
            SellInventory.OnDragEvent -= slot => UpdateSellAmount();
            SellInventory.OnDropEvent -= slot => UpdateSellAmount();
            currentPlayer.OnDropEvent -= slot => UpdateSellAmount();
            currentPlayer.OnEndDragEvent -= slot => UpdateSellAmount();
            currentPlayer.OnRightClickEvent -= slot => UpdateSellAmount();
            #endregion
            #region Buy Event On
            foreach (ItemSlot slot in BuyInventory.ItemSlots)
            {
                slot.OnLeftClickEvent += RemoveFromCart;
            }
            foreach (Item item in itemsForSale)
            {
                GameObject shopItemPrefab = Instantiate(ShopItemPrefab, Vector3.zero, Quaternion.identity);
                shopItemPrefab.transform.SetParent(scrollArea, false);
                ShopItem ShopItem = shopItemPrefab.GetComponent<ShopItem>();
                ShopItem.Setup(this, item, inCart, scrollArea);
            }
            #endregion
            playerStartingGold = currentPlayerUI.CheckGold();
            RunningGoldText.text = "Current Gold: " + playerStartingGold.ToString() + "G";
            StartingGoldText.text = "Total Gold: " + playerStartingGold.ToString() + "G";
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (!gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            currentCharacter.OpenItemContainer(SellInventory);
            #region Sell Events On
            SellInventory.OnRightClickEvent += slot => UpdateSellAmount();
            SellInventory.OnBeginDragEvent += slot => UpdateSellAmount();
            SellInventory.OnEndDragEvent += slot => UpdateSellAmount();
            SellInventory.OnDragEvent += slot => UpdateSellAmount();
            SellInventory.OnDropEvent += slot => UpdateSellAmount();
            currentPlayer.OnDropEvent += slot => UpdateSellAmount();
            currentPlayer.OnEndDragEvent += slot => UpdateSellAmount();
            currentPlayer.OnRightClickEvent += slot => UpdateSellAmount();
            #endregion
            #region Buy Event Off
            foreach (ItemSlot slot in BuyInventory.ItemSlots)
            {
                slot.OnLeftClickEvent -= RemoveFromCart;
            }
            for (int i = 0; i < scrollArea.childCount; i++)
            {
                Destroy(scrollArea.GetChild(i).gameObject);

            }
            #endregion
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    void EmptyCheck(Inventory invToCheck)
    {
        for (int i = 0; i < invToCheck.ItemSlots.Count; i++)
        {
            if (invToCheck.ItemSlots[i].Item != null && invToCheck.ItemSlots[i].Amount == 1)
                currentPlayer.AddItem(invToCheck.ItemSlots[i].Item.GetCopy());

            else if (invToCheck.ItemSlots[i].Item != null && invToCheck.ItemSlots[i].Amount > 1)
                currentPlayer.AddItem(invToCheck.ItemSlots[i].Item.GetCopy(), amount: invToCheck.ItemSlots[i].Amount);
        }
    }
    public void Sell()
    {
        int sellAmount = 0;
        for (int i = 0; i < SellInventory.ItemSlots.Count; i++)
        {
            if (SellInventory.ItemSlots[i].Item != null && SellInventory.ItemSlots[i].Amount == 1)
            {
                sellAmount += SellCost(SellInventory.ItemSlots[i].Item);
                SellInventory.ItemSlots[i].Item = null;
                SellInventory.ItemSlots[i].Amount = 0;
            }
            else if (SellInventory.ItemSlots[i].Item != null && SellInventory.ItemSlots[i].Amount > 1)
            {
                sellAmount += SellCost(SellInventory.ItemSlots[i].Item) * SellInventory.ItemSlots[i].Amount;
                SellInventory.ItemSlots[i].Item = null;
                SellInventory.ItemSlots[i].Amount = 0;
            }
        }
        currentPlayerUI.UpdateGold(sellAmount);
        UpdateSellAmount();
    }
    public void Buy()
    {
        if (currentPlayerUI.HasEnoughGold(BuyAmount))
        {
            currentPlayerUI.UpdateGold(-BuyAmount);
            BuyAmount = 0;
            for (int i = 0; i < BuyInventory.ItemSlots.Count; i++)
            {
                if (BuyInventory.ItemSlots[i].Item != null && BuyInventory.ItemSlots[i].Amount == 1)
                {
                    currentPlayer.AddItem(BuyInventory.ItemSlots[i].Item.GetCopy());
                    itemsForSale.Remove(BuyInventory.ItemSlots[i].Item);
                    BuyInventory.ItemSlots[i].Item = null;
                    BuyInventory.ItemSlots[i].Amount = 0;
                }
                else if (BuyInventory.ItemSlots[i].Item != null && BuyInventory.ItemSlots[i].Amount > 1)
                {
                    currentPlayer.AddItem(BuyInventory.ItemSlots[i].Item.GetCopy(), amount: BuyInventory.ItemSlots[i].Amount);
                    itemsForSale.Remove(BuyInventory.ItemSlots[i].Item);
                    BuyInventory.ItemSlots[i].Item = null;
                    BuyInventory.ItemSlots[i].Amount = 0;
                }
            }
            BuyText.text = "Buy Price: " + BuyAmount.ToString() + "G";
            RunningGoldText.text = "Current Gold: " + currentPlayerUI.CheckGold().ToString() + "G";
            StartingGoldText.text = "Total Gold: " + currentPlayerUI.CheckGold().ToString() + "G";
        }
    }
    int SellCost(Item item)
    {
        int sellAmount = 0;
        sellAmount += (item.cost - (int)(item.cost * .2f));
        return sellAmount;
    }
    public int BuyCost(Item item)
    {
        int sellAmount = 0;
        sellAmount += (item.cost + (int)(item.cost * .1f));
        return sellAmount;
    }
    public void AddToCart(Item item)
    {
        BuyInventory.AddItem(item, startSlot: 0);
        UpdateBuyAmount();
    }
    void RemoveFromCart(BaseItemSlot slot)
    {
        RemovedFromCart(slot.Item);
        BuyInventory.RemoveItem(slot.Item);
        UpdateBuyAmount();
    }
    void UpdateSellAmount()
    {
        SellAmount = 0;
        for (int i = 0; i < SellInventory.ItemSlots.Count; i++)
        {
            if (SellInventory.ItemSlots[i].Item != null && SellInventory.ItemSlots[i].Amount == 1 && SellInventory.ItemSlots[i].Item != Gold)
                SellAmount += SellCost(SellInventory.ItemSlots[i].Item);
            else if (SellInventory.ItemSlots[i].Item != null && SellInventory.ItemSlots[i].Amount > 1 && SellInventory.ItemSlots[i].Item != Gold)
                SellAmount += SellCost(SellInventory.ItemSlots[i].Item) * SellInventory.ItemSlots[i].Amount;
        }
        sellText.text = "Sell Value: " + SellAmount.ToString() + "G";
    }
    void UpdateBuyAmount()
    {
        int leftOverGold = 0;
        BuyAmount = 0;
        for (int i = 0; i < BuyInventory.ItemSlots.Count; i++)
        {
            if (BuyInventory.ItemSlots[i].Item != null && BuyInventory.ItemSlots[i].Amount == 1)
                BuyAmount += BuyCost(BuyInventory.ItemSlots[i].Item);
            else if (BuyInventory.ItemSlots[i].Item != null && BuyInventory.ItemSlots[i].Amount > 1)
                BuyAmount += BuyCost(BuyInventory.ItemSlots[i].Item) * BuyInventory.ItemSlots[i].Amount;
        }
        leftOverGold = currentPlayerUI.CheckGold() - BuyAmount;
        RunningGoldText.text = "Current Gold: " + leftOverGold.ToString() + "G";
        BuyText.text = "Buy Price: " + BuyAmount.ToString() + "G";
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
