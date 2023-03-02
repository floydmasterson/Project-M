using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Draw Speed Passive")]
public class BowDrawSpeedPassive : PassiveSO
{
    public float drawAmount;
    public int PassiveLvl;
    ArrowController AC;
    StringBuilder sb = new StringBuilder();
    public override void ApplyPassive()
    {
        AC = PlayerUi.Instance.target.GetComponent<ArrowController>();
        if (AC != null)
            Passive();
    }

    public override string GetDescription()
    {
        sb.Length = 0;

     

        if (PassiveLvl > 0 && drawAmount > 0)
        {
            sb.Append("Nimble Fingers " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Decrease bow draw time by " + drawAmount + "s.");
            return sb.ToString();
        }
        else if (PassiveLvl == 0 && drawAmount > 0)
        {
            sb.Append("Nimble Fingers: Decrease bow draw time by " + drawAmount + "s.");
            return sb.ToString();
        }
        if (PassiveLvl > 0 && drawAmount < 0)
        {
            sb.Append("Weighted String " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Increase bow draw time by " + drawAmount + "s.");
            return sb.ToString();
        }
        else if (PassiveLvl == 0 && drawAmount < 0)
        {
            sb.Append("Weighted String: Increase bow draw time by " + drawAmount + "s.");
            return sb.ToString();
        }
        else
            return sb.ToString();
    }

    public override void Passive()
    {
        if (AC != null)
            AC.DrawTime -= drawAmount;
    }

    public override void RemovePassive()
    {
        if (AC != null)
            AC.DrawTime += drawAmount;
    }
}
