using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProjectMItemCompair : OdinEditorWindow
{
    #region Items


    [TitleGroup("Items")]
    [HorizontalGroup("Items/Split")]
    [VerticalGroup("Items/Split/Left")]
    [SerializeField, PreviewField(20), HideLabel, Title("@item1?.ItemName", titleAlignment: TitleAlignments.Centered), BoxGroup("Items/Split/Left/Item 1", showLabel: false)]
    private Item item1;
    [VerticalGroup("Items/Split/Right")]
    [SerializeField, PreviewField(20), HideLabel, Title("@item2?.ItemName", titleAlignment: TitleAlignments.Centered), BoxGroup("Items/Split/Right/Item 2", showLabel: false)]
    private Item item2;
    [SerializeField, HideLabel, PreviewField(120), BoxGroup("Items/Split/Left/Item 1"), ReadOnly]
    private Sprite item1Icon;
    [SerializeField, HideLabel, PreviewField(120), BoxGroup("Items/Split/Right/Item 2"), ReadOnly]
    private Sprite item2Icon;
    #endregion
    #region Item 1 stats
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Strength Bonus"), GUIColor("@getStatColor(Item1S)")]
    private int Item1S;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Intelligence Bonus"), GUIColor("@getStatColor(Item1I)")]
    private int Item1I;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Agility Bonus"), GUIColor("@getStatColor(Item1A)")]
    private int Item1A;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Vitality Bonus"), GUIColor("@getStatColor(Item1V)")]
    private int Item1V;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Strength % Bonus"), GUIColor("@getStatColor(Item1SP)")]
    private float Item1SP;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Intelligence % Bonus"), GUIColor("@getStatColor(Item1IP)")]
    private float Item1IP;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Agility % Bonus"), GUIColor("@getStatColor(Item1AP)")]
    private float Item1AP;
    [SerializeField, BoxGroup("Items/Split/Left/Item 1"), LabelText("Vitality % Bonus"), GUIColor("@getStatColor(Item1VP)")]
    private float Item1VP;
    #endregion
    #region Item 2 stats
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Strength Bonus"), GUIColor("@getStatColor(Item2S)")]
    private int Item2S;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Intelligence Bonus"), GUIColor("@getStatColor(Item2I)")]
    private int Item2I;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Agility Bonus"), GUIColor("@getStatColor(Item2A)")]
    private int Item2A;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Vitality Bonus"), GUIColor("@getStatColor(Item2V)")]
    private int Item2V;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Strength % Bonus"), GUIColor("@getStatColor(Item2SP)")]
    private float Item2SP;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Intelligence % Bonus"), GUIColor("@getStatColor(Item2IP)")]
    private float Item2IP;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Agility % Bonus"), GUIColor("@getStatColor(Item2AP)")]
    private float Item2AP;
    [SerializeField, BoxGroup("Items/Split/Right/Item 2"), LabelText("Vitality % Bonus"), GUIColor("@getStatColor(Item2VP)")]
    private float Item2VP;
    #endregion
    #region StatCheck
    [TitleGroup("Stat Check")]
    [HorizontalGroup("Stat Check/Split")]
    [VerticalGroup("Stat Check/Split/Left")]
    #region item 1 vs 2
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Strength Difference"), GUIColor("@getColor(SDif1)")]
    private int SDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Intelligence Difference"), GUIColor("@getColor(IDif1)")]
    private int IDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Agility Difference"), GUIColor("@getColor(ADif1)")]
    private int ADif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Vitality Difference"), GUIColor("@getColor(VDif1)")]
    private int VDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Strength % Difference"), GUIColor("@getColor(SPDif1)")]
    private float SPDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Intelligence % Difference"), GUIColor("@getColor(IPDif1)")]
    private float IPDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Agility % Difference"), GUIColor("@getColor(APDif1)")]
    private float APDif1;
    [SerializeField, BoxGroup("Stat Check/Split/Left/Item 1", showLabel: false), LabelText("Vitality % Difference"), GUIColor("@getColor(VPDif1)")]
    private float VPDif1;
    #endregion
    #region item 2 vs 1
    [VerticalGroup("Stat Check/Split/Right")]
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Strength Difference"), GUIColor("@getColor(SDif2)")]
    private int SDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Intelligence Difference"), GUIColor("@getColor(IDif2)")]
    private int IDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Agility Difference"), GUIColor("@getColor(ADif2)")]
    private int ADif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Vitality Difference"), GUIColor("@getColor(VDif2)")]
    private int VDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Strength % Difference"), GUIColor("@getColor(SPDif2)")]
    private float SPDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Intelligence % Difference"), GUIColor("@getColor(IPDif2)")]
    private float IPDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Agility % Difference"), GUIColor("@getColor(APDif2)")]
    private float APDif2;
    [SerializeField, BoxGroup("Stat Check/Split/Right/Item 2", showLabel: false), LabelText("Vitality % Difference"), GUIColor("@getColor(VPDif1)")]
    private float VPDif2;
    #endregion
    #endregion
    #region Spells and Passives
    [TitleGroup("Spells and Passives")]
    [HorizontalGroup("Spells and Passives/Split")]
    [VerticalGroup("Spells and Passives/Split/Left")]
    [BoxGroup("Spells and Passives/Split/Left/Item 1", showLabel: false), InlineEditor, SerializeField, HideLabel, HideIf("@item1Spell == null"), ListDrawerSettings(Expanded = true, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, IsReadOnly = true)]
    private SpellScriptableObject item1Spell;
    [VerticalGroup("Spells and Passives/Split/Right")]
    [BoxGroup("Spells and Passives/Split/Right/Item 2", showLabel: false), InlineEditor, SerializeField, HideLabel, HideIf("@item2Spell == null"), ListDrawerSettings(Expanded = true, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, IsReadOnly = true)]
    private SpellScriptableObject item2Spell;
    [BoxGroup("Spells and Passives/Split/Left/Item 1", showLabel: false), SerializeField, HideLabel, ListDrawerSettings(Expanded = true, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, IsReadOnly = true)]
    private PassiveSO[] Item1passives;
    [BoxGroup("Spells and Passives/Split/Right/Item 2", showLabel: false), SerializeField, HideLabel, ListDrawerSettings(Expanded = true, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, IsReadOnly = true)]
    private PassiveSO[] Item2passives;
    #endregion
    #region Econ
    [TitleGroup("Economy")]
    [HorizontalGroup("Economy/Split")]
    [VerticalGroup("Economy/Split/Left")]
    [BoxGroup("Economy/Split/Left/Item 1", showLabel: false), SerializeField, LabelText("Buy Price"), GUIColor("@getColor(item1Buy)")]
    private int item1Buy;
    [BoxGroup("Economy/Split/Left/Item 1", showLabel: false), SerializeField, LabelText("Sell Value"), GUIColor("@getColor(item1Sell)")]
    private int item1Sell;
    [VerticalGroup("Economy/Split/Right")]
    [BoxGroup("Economy/Split/Right/Item 2", showLabel: false), SerializeField, LabelText("Buy Price"), GUIColor("@getColor(item2Buy)")]
    private int item2Buy;
    [BoxGroup("Economy/Split/Right/Item 2", showLabel: false), SerializeField, LabelText("Sell Value"), GUIColor("@getColor(item2Sell)")]
    private int item2Sell;
    [BoxGroup("Economy/Split/Left/Item 1", showLabel: false), SerializeField, LabelText("Buy Difference"), GUIColor("@getColor(item1Buy - item2Buy)")]
    private int item1Buydiff;
    [BoxGroup("Economy/Split/Left/Item 1", showLabel: false), SerializeField, LabelText("Sell Difference"), GUIColor("@getColor(item1Sell - item2Sell)")]
    private int item1Selldiff;
    [BoxGroup("Economy/Split/Right/Item 2", showLabel: false), SerializeField, LabelText("Buy Difference"), GUIColor("@getColor(item2Buy - item1Buy)")]
    private int item2Buydiff;
    [BoxGroup("Economy/Split/Right/Item 2", showLabel: false), SerializeField, LabelText("Sell Difference"), GUIColor("@getColor(item2Sell - item1Sell)")]
    private int item2Selldiff;
    #endregion


    [MenuItem("Tools/Project M Item Comparison Tool")]
    private static void OpenWindow()
    {
        var window = GetWindow<ProjectMItemCompair>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }
    private void OnValidate()
    {
        if (item1 != null)
        {
            item1Icon = item1.Icon;
        }
        else if (item1 == null)
        {
            item1Icon = null;

        }
        if (item2 != null)
            item2Icon = item2.Icon;
        else if (item2 == null)
        {
            item2Icon = null;

        }
        ZeroOut(1);
        ZeroOut(2);
        CompairItems();


    }
    [Button("Refresh Compair", DisplayParameters = false), BoxGroup("Controls", showLabel: false), PropertyOrder(-1), Title("Controls", titleAlignment: TitleAlignments.Centered)]
    private void CompairItems()
    {
        if (item1 is EquippableItem)
        {
            EquippableItem item = item1 as EquippableItem;
            if (item.BoundSpell != null)
                item1Spell = item.BoundSpell.spellToCast;
            if (!item.Passives.IsNullOrEmpty())
                Item1passives = item.Passives.ToArray();
            statCheck(item, 1);
        }

        if (item2 is EquippableItem)
        {
            EquippableItem item = item2 as EquippableItem;
            if (item.BoundSpell != null)
                item2Spell = item.BoundSpell.spellToCast;

            if (!item.Passives.IsNullOrEmpty())
                Item2passives = item.Passives.ToArray();

            statCheck(item, 2);
        }
        EconCheck(1);
        EconCheck(2);
        Compair1to2();
        Compair2to1();
    }
    [Button("Clear", DisplayParameters = false), BoxGroup("Controls", showLabel: false), PropertyOrder(-1)]
    private void Clear()
    {
        item1 = null;
        item1Icon = null;
        Item1S = 0;
        Item1I = 0;
        Item1A = 0;
        Item1V = 0;
        Item1SP = 0;
        Item1IP = 0;
        Item1AP = 0;
        Item1VP = 0;
        SDif1 = 0;
        IDif1 = 0;
        ADif1 = 0;
        VDif1 = 0;
        SPDif1 = 0;
        IPDif1 = 0;
        APDif1 = 0;
        VPDif1 = 0;
        item1Spell = null;
        Item1passives = null;
        item1Buy = 0;
        item1Sell = 0;
        item1Buydiff = 0;
        item1Selldiff = 0;
        item2 = null;
        item2Icon = null;
        Item2S = 0;
        Item2I = 0;
        Item2A = 0;
        Item2V = 0;
        Item2SP = 0;
        Item2IP = 0;
        Item2AP = 0;
        Item2VP = 0;
        SDif2 = 0;
        IDif2 = 0;
        ADif2 = 0;
        VDif2 = 0;
        SPDif2 = 0;
        IPDif2 = 0;
        APDif2 = 0;
        VPDif2 = 0;
        item2Spell = null;
        Item2passives = null;
        item2Buy = 0;
        item2Sell = 0;
        item2Buydiff = 0;
        item2Selldiff = 0;
    }
    private void statCheck(EquippableItem item, int itemNumber)
    {
        switch (itemNumber)
        {
            case 1:
                {
                    Item1S = item.StrengthBonus;
                    Item1I = item.IntelligenceBonus;
                    Item1A = item.AgilityBonus;
                    Item1V = item.VitalityBonus;
                    Item1SP = item.StrengthPercentBonus;
                    Item1IP = item.IntelligencePercentBonus;
                    Item1AP = item.AgilityPercentBonus;
                    Item1VP = item.VitalityPercentBonus;
                }
                break;
            case 2:
                {
                    Item2S = item.StrengthBonus;
                    Item2I = item.IntelligenceBonus;
                    Item2A = item.AgilityBonus;
                    Item2V = item.VitalityBonus;
                    Item2SP = item.StrengthPercentBonus;
                    Item2IP = item.IntelligencePercentBonus;
                    Item2AP = item.AgilityPercentBonus;
                    Item2VP = item.VitalityPercentBonus;
                }
                break;
        }
    }
    private void Compair1to2()
    {
        SDif1 = Item1S - Item2S;
        IDif1 = Item1I - Item2I;
        ADif1 = Item1A - Item2A;
        VDif1 = Item1V - Item2V;
        SPDif1 = Item1SP - Item2SP;
        IPDif1 = Item1IP - Item2IP;
        APDif1 = Item1AP - Item2AP;
        VPDif1 = Item1VP - Item2VP;
        if (item1 != null && item2 != null)
        {
            item1Buydiff = item1.BuyPrice - item2.BuyPrice;
            item1Selldiff = item1.SellValue - item2.SellValue;

        }
    }
    private void Compair2to1()
    {
        SDif2 = Item2S - Item1S;
        IDif2 = Item2I - Item1I;
        ADif2 = Item2A - Item1A;
        VDif2 = Item2V - Item1V;
        SPDif2 = Item2SP - Item1SP;
        IPDif2 = Item2IP - Item1IP;
        APDif2 = Item2AP - Item1AP;
        VPDif2 = Item2VP - Item1VP;
        if (item1 != null && item2 != null)
        {
            item2Buydiff = item2.BuyPrice - item1.BuyPrice;
            item2Selldiff = item2.SellValue - item1.SellValue;
        }
    }
    private void ZeroOut(int itemNumber)
    {
        switch (itemNumber)
        {
            case 1:
                {
                    Item1S = 0;
                    Item1I = 0;
                    Item1A = 0;
                    Item1V = 0;
                    Item1SP = 0;
                    Item1IP = 0;
                    Item1AP = 0;
                    Item1VP = 0;
                    SDif1 = 0;
                    IDif1 = 0;
                    ADif1 = 0;
                    VDif1 = 0;
                    SPDif1 = 0;
                    IPDif1 = 0;
                    APDif1 = 0;
                    VPDif1 = 0;
                    item1Spell = null;
                    Item1passives = null;
                    item1Buy = 0;
                    item1Sell = 0;
                    item1Buydiff = 0;
                    item1Selldiff = 0;
                }
                break;
            case 2:
                {
                    Item2S = 0;
                    Item2I = 0;
                    Item2A = 0;
                    Item2V = 0;
                    Item2SP = 0;
                    Item2IP = 0;
                    Item2AP = 0;
                    Item2VP = 0;
                    SDif2 = 0;
                    IDif2 = 0;
                    ADif2 = 0;
                    VDif2 = 0;
                    SPDif2 = 0;
                    IPDif2 = 0;
                    APDif2 = 0;
                    VPDif2 = 0;
                    item2Spell = null;
                    Item2passives = null;
                    item2Buy = 0;
                    item2Sell = 0;
                    item2Buydiff = 0;
                    item2Selldiff = 0;
                }
                break;
        }


    }
    private void EconCheck(int itemNumber)
    {
        switch (itemNumber)
        {
            case 1:
                {
                    if (item1 != null)
                    {
                        item1Buy = item1.BuyPrice;
                        item1Sell = item1.SellValue;

                    }
                }
                break;
            case 2:
                {
                    if (item2 != null)
                    {
                        item2Buy = item2.BuyPrice;
                        item2Sell = item2.SellValue;
                    }
                }
                break;

        }
    }
    private static Color getColor(float diffrence)
    {
        if (diffrence == 0)
            return new Color(0, 0, 0);
        else if (diffrence > 0)
            return new Color(0, 1, 0);
        else if (diffrence < 0)
            return new Color(1, 0, 0);
        return new Color(0, 0, 0);

    }
    private static Color getStatColor(float stat)
    {
        if (stat == 0)
            return new Color(0, 0, 0);
        else if (stat > 0)
            return new Color(0, 1, 0);
        else if (stat < 0)
            return new Color(1, 0, 0);
        return new Color(0, 0, 0);
    }
}
