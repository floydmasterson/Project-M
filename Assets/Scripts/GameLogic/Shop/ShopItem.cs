using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [TabGroup("Setup"), SerializeField, Required]
    TextMeshProUGUI Gold;
    [TabGroup("Setup"), SerializeField, Required]
    Button button;
    [TabGroup("Setup"), SerializeField, Required]
    Image ItemIcon;
    [TabGroup("Setup"), SerializeField, Required]
    private BaseItemSlot slot;
    [TabGroup("Setup"), SerializeField, Tooltip("Set at runtime")]
    public Item ItemForSale;
    [TabGroup("Setup"), SerializeField, Tooltip("Set at runtime")]
    public ShopController Shop;
    int amountForSale;
    Transform inCart;
    Transform inShop;

    public void Setup(ShopController shop, Item saleItem, Transform InCart, Transform InShop)
    {
        Shop = shop;
        ItemForSale = saleItem;
        inCart = InCart;
        inShop = InShop;
        ItemIcon.sprite = ItemForSale.Icon;
        if (ItemForSale is UsableItem)
            amountForSale = UnityEngine.Random.Range(2, 6);
        else
            amountForSale = 1;
        slot.Item = saleItem;
        slot.Amount = amountForSale;
        Gold.text = (ItemForSale.BuyPrice).ToString() + "G x" + amountForSale;
        button.onClick.AddListener(addToCart);
        Shop.RemovedFromCart += ReEnable;
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(addToCart);
        Shop.RemovedFromCart -= ReEnable; 
    }

    void addToCart()
    {
        Shop.AddToCart(ItemForSale);
        amountForSale -= 1;
        if (amountForSale == 0)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.SetParent(inCart);
        }
        else
        {
            Gold.text = (ItemForSale.BuyPrice).ToString() + "G x" + amountForSale;
        }
    }

    void ReEnable(Item item)
    {
        if (item.ID == ItemForSale.ID)
        {
            amountForSale += 1;   
            Gold.text = (ItemForSale.BuyPrice).ToString() + "G x" + amountForSale;
            gameObject.transform.SetParent(inShop);
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

  
}
