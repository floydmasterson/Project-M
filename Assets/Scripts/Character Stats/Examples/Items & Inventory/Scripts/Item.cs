using Sirenix.OdinInspector;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public enum Tier
    {
        T0,
        T1,
        T2,
        T3,
    }
    public string ID { get { return id; } }
    [PreviewField(60), HideLabel]
    [TableColumnWidth(300, resizable: false)]
    [HorizontalGroup("Item", 60)]
    public Sprite Icon;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public string ItemName;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public int itemId;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    [EnumToggleButtons]
    public Tier ItemTier;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    private int value;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public int BuyPrice;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public int SellValue;
    [VerticalGroup("Item/Right"), LabelWidth(105)]
    [Range(1, 999)]
    public int MaximumStacks = 1;
    [VerticalGroup("Item/Right"), LabelWidth(90)]
    [ShowIf("@MaximumStacks > 1")]
    public bool IsConsumable;
    [VerticalGroup("Item/Right"), LabelWidth(90)]
    [ShowIf("@IsConsumable")]
    public bool IsFood;
    [PropertyOrder(8)]
    [TableColumnWidth(160, resizable: true)]
    [TextArea]
    public string Notes;
    [HideIf("@1 == 1")]
    [SerializeField] string id;




    protected static readonly StringBuilder sb = new StringBuilder();

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
        value = FindCost(this);
        SellValue = GetSellValue();
        BuyPrice = GetBuyPrice();
    }
#endif

    public virtual Item GetCopy()
    {
        return this;
    }

    public virtual void Destroy()
    {

    }
    public virtual string GetTier()
    {
        return ItemTier.ToString();
    }
    public virtual string GetItemType()
    {
        return "";
    }

    public virtual string GetDescription(bool goldType)
    {
        return "";
    }
    public virtual int GetSellValue()
    {
        return value - (int)(value * .40f);
    }
    public virtual int GetBuyPrice()
    {
        return value + (int)(value * .60f);
    }

    public virtual int FindCost(Item Item)
    {
        float value = 0;
        if (ItemName == "Gold Coin")
            return 1;
        if (ItemTier == Tier.T0)
            return Mathf.RoundToInt(value);
        if (IsConsumable)
        {
            if (!IsFood)
            {
                switch (ItemTier)
                {

                    case Tier.T1:
                        value += 10;
                        break;
                    case Tier.T2:
                        value += 25;
                        break;
                    case Tier.T3:
                        value += 50;
                        break;
                }

            value += value * .1f;
            }
            else
            {
                switch (ItemTier)
                {

                    case Tier.T1:
                        value += 35;
                        break;
                    case Tier.T2:
                        value += 55;
                        break;
                    case Tier.T3:
                        value += 75;
                        break;
                }

                value += value * .15f;
            }
        }
        else if (!IsConsumable)
        {
            switch (ItemTier)
            {
                case Tier.T1:
                    value += 100;
                    break;
                case Tier.T2:
                    value += 250;
                    break;
                case Tier.T3:
                    value += 400;
                    break;
            }
            if (this is EquippableItem)
            {
                EquippableItem item = Item as EquippableItem;
                value += item.AgilityBonus * 10;
                value += item.VitalityBonus * 10;
                value += item.IntelligenceBonus * 15;
                value += item.StrengthBonus * 15;

                value += item.AgilityPercentBonus * 100;
                value += item.VitalityPercentBonus * 100;
                value += item.IntelligencePercentBonus * 200;
                value += item.StrengthPercentBonus * 200;

                foreach (PassiveSO passive in item.Passives)
                {
                    if (passive != null)
                    {
                        value += 20;
                    }

                }
            }


            switch (ItemTier)
            {
                case Tier.T1:
                    value += value / 5;
                    break;
                case Tier.T2:
                    value += value / 3;
                    break;
                case Tier.T3:
                    value += value / 2;
                    break;
            }
        }

        return Mathf.RoundToInt(value);
    }

}
