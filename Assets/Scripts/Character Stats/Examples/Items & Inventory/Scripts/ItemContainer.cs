using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour, IItemContainer
{
    [TabGroup("Setup")]
    public List<ItemSlot> ItemSlots;

    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnRightClickEvent;
    public event Action<BaseItemSlot> OnBeginDragEvent;
    public event Action<BaseItemSlot> OnEndDragEvent;    
    public event Action<BaseItemSlot> OnDragEvent;
    public event Action<BaseItemSlot> OnDropEvent;

    protected virtual void OnValidate()
    {
    }

    protected virtual void Start()
    {
        GetComponentsInChildren(includeInactive: true, result: ItemSlots);
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            ItemSlots[i].OnPointerEnterEvent += slot => EventHelper(slot, OnPointerEnterEvent);
            ItemSlots[i].OnPointerExitEvent += slot => EventHelper(slot, OnPointerExitEvent);
            ItemSlots[i].OnRightClickEvent += slot => EventHelper(slot, OnRightClickEvent);
            ItemSlots[i].OnBeginDragEvent += slot => EventHelper(slot, OnBeginDragEvent);
            ItemSlots[i].OnEndDragEvent += slot => EventHelper(slot, OnEndDragEvent);
            ItemSlots[i].OnDragEvent += slot => EventHelper(slot, OnDragEvent);
            ItemSlots[i].OnDropEvent += slot => EventHelper(slot, OnDropEvent);
        }
    }

    private void EventHelper(BaseItemSlot itemSlot, Action<BaseItemSlot> action)
    {
        if (action != null)
            action(itemSlot);
    }

    public virtual bool CanAddItem(Item item, int amount = 1)
    {
        int freeSpaces = 0;

        foreach (ItemSlot itemSlot in ItemSlots)
        {
            if (itemSlot.Item == null || itemSlot.Item.ID == item.ID)
            {
                freeSpaces += item.MaximumStacks - itemSlot.Amount;
            }
        }
        return freeSpaces >= amount;
    }

    public virtual bool AddItem(Item item, int startSlot = 1, int amount = 1)
    {

        for (int i = startSlot; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].CanAddStack(item, amount))
            {
                ItemSlots[i].Item = item;
                ItemSlots[i].Amount += amount;
                return true;
            }
        }
        for (int i = startSlot; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item == null)
            {
                ItemSlots[i].Item = item;
                ItemSlots[i].Amount = amount;
                return true;
            }
        }
        return false;
    }

    public virtual bool RemoveItem(Item item, int amount = 1)
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            if (ItemSlots[i].Item == item)
            {        
                ItemSlots[i].Amount -= amount;
                return true;
            }
        }
        return false;
    }

    public virtual Item RemoveItem(string itemID)
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            Item item = ItemSlots[i].Item;
            if (item != null && item.ID == itemID)
            {
                ItemSlots[i].Amount--;
                return item;
            }
        }
        return null;
    }

    public virtual int ItemCount(string itemID)
    {
        int number = 0;

        for (int i = 0; i < ItemSlots.Count; i++)
        {
            Item item = ItemSlots[i].Item;
            if (item != null && item.ID == itemID)
            {
                number += ItemSlots[i].Amount;
            }
        }
        return number;
    }

    public void Clear()
    {
        for (int i = 0; i < ItemSlots.Count; i++)
        {
            ItemSlots[i].Item = null;
            ItemSlots[i].Amount = 0;
        }
    }
}
