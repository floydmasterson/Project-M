
using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Mana Up Passive")]
public class ManaUpPassive : PassiveSO
{
    public int manaAmount;
    public int PassiveLvl;
    MagicController MC;
    StringBuilder sb = new StringBuilder();

    public override void Passive()
    {
        if (MC != null)
            MC.MaxMana += manaAmount;
    }
    public override void ApplyPassive()
    {
        MC = PlayerUi.Instance.target.GetComponent<MagicController>();
        if (MC != null)
            Passive();
    }
    public override void RemovePassive()
    {
        if (MC != null)
            MC.MaxMana -= manaAmount;
    }
    public override string GetDescription()
    {
        sb.Length = 0;

        if (PassiveLvl > 0)
        {
            sb.Append("Mana Well " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": This item contains a mana reserve increasing max mana by " + manaAmount);
            return sb.ToString();
        }
        else
        {
            sb.Append("Mana Well: This item contains a mana reserve increasing max mana by " + manaAmount);
            return sb.ToString();
        }
    }
}



