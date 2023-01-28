using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif


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
    [EnumToggleButtons]
    public Tier ItemTier;
    [VerticalGroup("Item/Right"), LabelWidth(65)]
    public int cost;
    [VerticalGroup("Item/Right"), LabelWidth(105)]
    [Range(1, 999)]
    public int MaximumStacks = 1;
    [VerticalGroup("Item/Right"), LabelWidth(90)]
    [ShowIf("@MaximumStacks > 1")]
    public bool IsConsumable;
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
        cost = FindCost(this);
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

    public virtual string GetDescription()
    {
        return "";
    }
    public virtual int GetSellValue()
    {
        return cost - (int)(cost * .2f);
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
            value += value * (int).1f;
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
                    value += 500;
                    break;
            }
            if (this is EquippableItem)
            {
                EquippableItem item = Item as EquippableItem;
                value += item.AgilityBonus * 15;
                value += item.VitalityBonus * 15;
                value += item.IntelligenceBonus * 20;
                value += item.StrengthBonus * 20;

                value += item.AgilityPercentBonus * 100;
                value += item.VitalityPercentBonus * 100;
                value += item.IntelligencePercentBonus * 200;
                value += item.StrengthPercentBonus * 200;

                foreach (PassiveSO passive in item.Passives)
                {
                    if (passive != null)
                    {
                        value += 25;
                    }

                }
            }


            switch (ItemTier)
            {
                case Tier.T1:
                    value += value * .2f;
                    break;
                case Tier.T2:
                    value += value * .3f;
                    break;
                case Tier.T3:
                    value += value * .5f;
                    break;
            }
        }

        return Mathf.RoundToInt(value);
    }

}
