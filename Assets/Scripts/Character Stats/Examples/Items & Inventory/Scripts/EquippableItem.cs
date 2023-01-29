using Kryz.CharacterStats;
using Sirenix.OdinInspector;
using UnityEngine;


public enum EquipmentType
{
    [GUIColor(0, 1, 0)]
    Helmet,
    Chest,
    Gloves,
    Boots,
    Belt,
    Melee_Weapon,
    Magic_Weapon,
    Off_Hand,
    Leggings,
    Major_Accessory,
    Minor_Accessory,
    Sholder_Guard,
}

[CreateAssetMenu(menuName = "Items/Equippable Item")]
public class EquippableItem : Item
{
    [TableColumnWidth(190, resizable: false)]
    [FoldoutGroup("Flat Stat Modifier", expanded: true)]
    public int StrengthBonus;
    [FoldoutGroup("Flat Stat Modifier", expanded: true)]
    public int AgilityBonus;
    [FoldoutGroup("Flat Stat Modifier", expanded: true)]
    public int IntelligenceBonus;
    [FoldoutGroup("Flat Stat Modifier", expanded: true)]
    public int VitalityBonus;

    [TableColumnWidth(190, resizable: false)]
    [FoldoutGroup("Percentage Stat Modifier", expanded: true)]
    public float StrengthPercentBonus;
    [FoldoutGroup("Percentage Stat Modifier", expanded: true)]
    public float AgilityPercentBonus;
    [FoldoutGroup("Percentage Stat Modifier", expanded: true)]
    public float IntelligencePercentBonus;
    [FoldoutGroup("Percentage Stat Modifier", expanded: true)]
    public float VitalityPercentBonus;
    [TableColumnWidth(190, resizable: false)]
    public PassiveSO[] Passives;
    [TableColumnWidth(300, resizable: false)]
    [VerticalGroup("Equipment Type")]
    [EnumToggleButtons, HideLabel]
    public EquipmentType EquipmentType;

    [HorizontalGroup("Equipment Type/Right")]
    [ShowIf("EquipmentType", EquipmentType.Magic_Weapon)]
    public Spell BoundSpell;



    public override Item GetCopy()
    {
        return Instantiate(this);
    }

    public override void Destroy()
    {
        Destroy(this);
    }

    public void Equip(Character c)
    {
        if (StrengthBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        if (AgilityBonus != 0)
            c.Agility.AddModifier(new StatModifier(AgilityBonus, StatModType.Flat, this));
        if (IntelligenceBonus != 0)
            c.Intelligence.AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, this));
        if (VitalityBonus != 0)
            c.Vitality.AddModifier(new StatModifier(VitalityBonus, StatModType.Flat, this));

        if (StrengthPercentBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
        if (AgilityPercentBonus != 0)
            c.Agility.AddModifier(new StatModifier(AgilityPercentBonus, StatModType.PercentMult, this));
        if (IntelligencePercentBonus != 0)
            c.Intelligence.AddModifier(new StatModifier(IntelligencePercentBonus, StatModType.PercentMult, this));
        if (VitalityPercentBonus != 0)
            c.Vitality.AddModifier(new StatModifier(VitalityPercentBonus, StatModType.PercentMult, this));
        if (BoundSpell != null)
            PlayerUi.Instance.target.gameObject.GetComponent<MagicController>().selectedSpell = BoundSpell;
        if (Passives != null)
        {
            foreach (PassiveSO passive in Passives)
            {
                if (passive != null)
                {
                    passive.ApplyPassive();
                }
            }
        }
    }
    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Agility.RemoveAllModifiersFromSource(this);
        c.Intelligence.RemoveAllModifiersFromSource(this);
        c.Vitality.RemoveAllModifiersFromSource(this);
        if (BoundSpell != null)
            PlayerUi.Instance.target.gameObject.GetComponent<MagicController>().selectedSpell = null;
        if (Passives != null)
        {


            foreach (PassiveSO passive in Passives)
            {
                if (passive != null)
                {
                    passive.RemovePassive();
                }

            }
        }
        PlayerUi.Instance.target.CheckMaxHealth();
    }

    public override string GetItemType()
    {
        return EquipmentType.ToString();
    }

    public override string GetDescription(bool sell)
    {
        sb.Length = 0;
        AddStat(StrengthBonus, "Strength");
        AddStat(AgilityBonus, "Agility");
        AddStat(IntelligenceBonus, "Intelligence");
        AddStat(VitalityBonus, "Vitality");

        AddStat(StrengthPercentBonus, "Strength", isPercent: true);
        AddStat(AgilityPercentBonus, "Agility", isPercent: true);
        AddStat(IntelligencePercentBonus, "Intelligence", isPercent: true);
        AddStat(VitalityPercentBonus, "Vitality", isPercent: true);
        if (BoundSpell != null)
        {
            sb.AppendLine();
            sb.Append(BoundSpell + "is bound to this weapon").Replace("(Spell)", "").Replace("S-", "");

        }
        if (Passives != null)
        {
            foreach (PassiveSO passive in Passives)
            {
                if (passive != null)
                {
                    sb.AppendLine();
                    sb.Append(passive.GetDescription());
                }

            }
            sb.AppendLine();
            if (sell)
                sb.Append("Sell Value: " + SellValue + "G");
            else if(!sell)
                sb.Append("Buy Price: " + BuyPrice + "G");
        }

        return sb.ToString();
    }

    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (value > 0)
                sb.Append("+");

            if (isPercent)
            {
                sb.Append(value * 100);
                sb.Append("% ");
            }
            else
            {
                sb.Append(value);
                sb.Append(" ");
            }
            sb.Append(statName);
        }
    }
}
