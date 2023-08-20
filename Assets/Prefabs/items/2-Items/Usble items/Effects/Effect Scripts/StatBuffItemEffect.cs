using Kryz.CharacterStats;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Stat Buff")]
public class StatBuffItemEffect : UseableItemEffect
{
    public int StrengthBuff;
    public float AgilityBuff;
    public int IntelligenceBuff;
    public int VitalityBuff;
    [HideIf("@!hasDuration")]
    public float Duration;
    public bool hasDuration;

    StringBuilder sb = new StringBuilder();
    public IEnumerator RemoveBuff(Character character, StatModifier statModifier, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        if (StrengthBuff != 0)
        {
            character.Strength.RemoveModifier(statModifier);

        }
        if (AgilityBuff != 0)
        {
            character.Agility.RemoveModifier(statModifier);

        }
        if (IntelligenceBuff != 0)
        {
            character.Intelligence.RemoveModifier(statModifier);

        }
        if (VitalityBuff != 0)
        {
            character.Vitality.RemoveModifier(statModifier);

        }
        character.statPanel.UpdateStatValues();
        Debug.Log("buff over");
    }
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        StatModifier statModifierA = new StatModifier(AgilityBuff, StatModType.Flat, parentItem);
        StatModifier statModifierS = new StatModifier(StrengthBuff, StatModType.Flat, parentItem);
        StatModifier statModifierI = new StatModifier(IntelligenceBuff, StatModType.Flat, parentItem);
        StatModifier statModifierV = new StatModifier(VitalityBuff, StatModType.Flat, parentItem);



        if (StrengthBuff != 0)
        {
            character.Strength.AddModifier(statModifierS);
            if (hasDuration)
                character.StartCoroutine(RemoveBuff(character, statModifierS, Duration));

        }
        if (AgilityBuff != 0)
        {
            character.Agility.AddModifier(statModifierA);
            if (hasDuration)
                character.StartCoroutine(RemoveBuff(character, statModifierA, Duration));

        }
        if (IntelligenceBuff != 0)
        {
            character.Intelligence.AddModifier(statModifierI);
            if (hasDuration)
                character.StartCoroutine(RemoveBuff(character, statModifierI, Duration));

        }
        if (VitalityBuff != 0)
        {
            character.Vitality.AddModifier(statModifierV);
            if (hasDuration)
                character.StartCoroutine(RemoveBuff(character, statModifierV, Duration));

        }
        character.statPanel.UpdateStatValues();
        base.ExecuteEffect(parentItem, character);

    }

    public override string GetDescription()
    {
        sb.Length = 0;
        if (hasDuration)
            sb.Append("Gain the following for " + Duration + " seconds. ");
        else
            sb.Append("Gain the following");
        sb.AppendLine();
        AddStat(StrengthBuff, "Strength");
        AddStat(AgilityBuff, "Agility");
        AddStat(IntelligenceBuff, "Intelligence");
        AddStat(VitalityBuff, "Vitality");
        return sb.ToString();
    }

    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (value > 0)
            {
                sb.Append("+");
            }
            else
            {
                sb.Append("-");
            }
            sb.Append(value);
            sb.Append(" ");
            sb.Append(statName);
            sb.Append(" ");
        }
    }

    public override bool canBeUsed()
    {
        return true;
    }
}
