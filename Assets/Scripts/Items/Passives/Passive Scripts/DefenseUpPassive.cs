using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Defense Up Passive")]
public class DefenseUpPassive : PassiveSO
{
    public int defenseAmount;
    public int PassiveLvl;
    StringBuilder sb = new StringBuilder();
    public override void Passive()
    {
        PlayerUi.Instance.target.Defense += defenseAmount;
    }

    public override void ApplyPassive()
    {
        Passive();
    }


    public override void RemovePassive()
    {
        PlayerUi.Instance.target.Defense -= defenseAmount;
    }
    public override string GetDescription()
    {
        sb.Length = 0;

        if (PassiveLvl > 0)
        {
            sb.Append("Rigid Structure " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Increase overall defense by " + defenseAmount + ".");
            return sb.ToString();
        }
        else
        {
            sb.Append("Rigid Structure: Increase overall defense by " + defenseAmount + ".");
            return sb.ToString();
        }
    }

}
