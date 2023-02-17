
using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Life Steal Passive")]
public class LifeStealPassive : PassiveSO
{
    public float LifeStealAmount;
    public int PassiveLvl;
    PlayerManger player;
    MeeleController MC;
    StringBuilder sb = new StringBuilder();
    public override void Passive()
    {
        if (MC != null)
        {
            MC.lifeStealAmount += LifeStealAmount;
            player.LifeSteal = true;
        }
    }
    public override void ApplyPassive()
    {
        player = PlayerUi.Instance.target;
        MC = player.GetComponent<MeeleController>();
        Passive();
    }
    public override void RemovePassive()
    {
        if (MC != null)
        {
            MC.lifeStealAmount -= LifeStealAmount;
            player.LifeSteal = false;
        }
    }


    public override string GetDescription()
    {
        sb.Length = 0;
        if (PassiveLvl > 0)
        {
            sb.Append("Life Drain " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Life steal for " + LifeStealAmount * 100 + "% of melee damage.");
            return sb.ToString();
        }
        else
        {

            sb.Append("Life Drain: Life steal for "+ LifeStealAmount * 100 + "% of melee damage.");
            return sb.ToString();
        }

    }

}
