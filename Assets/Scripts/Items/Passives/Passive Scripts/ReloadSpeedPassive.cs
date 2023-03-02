using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Reload Speed Passive")]
public class ReloadSpeedPassive : PassiveSO
{
    public float reloadAmount;
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

        if (PassiveLvl > 0 && reloadAmount > 0)
        {
            sb.Append("Swift Reload " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Decrease bow reload time by " + reloadAmount +"s.");
            return sb.ToString();
        }
        else if(PassiveLvl == 0 && reloadAmount > 0)
        {
            sb.Append("Swift Reload: Decrease bow reload time by " + reloadAmount + "s.");
            return sb.ToString();
        }
        if (PassiveLvl > 0 && reloadAmount < 0)
        {
            sb.Append("Heavy Arrows " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Increase bow reload time by " + reloadAmount + "s.");
            return sb.ToString();
        }
        else if (PassiveLvl == 0 && reloadAmount < 0)
        {
            sb.Append("Heavy Arrows: increase bow reload time by " + reloadAmount + "s.");
            return sb.ToString();
        }
        else
            return sb.ToString();
    }

    public override void Passive()
    {
        if (AC != null)
            AC.timeBetweenReload -= reloadAmount;
    }

    public override void RemovePassive()
    {
        if (AC != null)
            AC.timeBetweenReload += reloadAmount;
    }
}
