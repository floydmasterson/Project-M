using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Roll Distance Passive")]
public class RollDistancePassive : PassiveSO
{
    public int pushAmount;
    public int PassiveLvl;
    StringBuilder sb = new StringBuilder();

    public override void Passive()
    {
        PlayerUi.Instance.target.pushAmt += pushAmount;
    }
    public override void ApplyPassive()
    {
        Passive();
    }
    public override void RemovePassive()
    {
        PlayerUi.Instance.target.pushAmt -= pushAmount;
    }
    public override string GetDescription()
    {
        sb.Length = 0;

        if (PassiveLvl > 0)
        {
            sb.Append("Wind Step " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Increase roll amount by " + pushAmount + ".");
            return sb.ToString();
        }
        else
        {
            sb.Append("Wind Step: Increase roll amount by " + pushAmount + ".");
            return sb.ToString();
        }
    }
}

