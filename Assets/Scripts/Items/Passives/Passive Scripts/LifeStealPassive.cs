
using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName = "Passives/Life Steal Passive")]
public class LifeStealPassive : PassiveSO
{
    PlayerManger player;
    MeeleController MC;
    StringBuilder sb = new StringBuilder();
    public override void Passive()
    {
        if (MC != null)
            player.LifeSteal = true;
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
            player.LifeSteal = false;
    }


    public override string GetDescription()
    {
        sb.Length = 0;
        sb.Append("Life Drain: Attacks now heal you for 1/3 of its damage");
        return sb.ToString();

    }

}
