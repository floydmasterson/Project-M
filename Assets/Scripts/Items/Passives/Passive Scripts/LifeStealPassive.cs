
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
            MC.lifeStealAmount += .10f;
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
            MC.lifeStealAmount -= .10f;
    }


    public override string GetDescription()
    {
        sb.Length = 0;
        sb.Append("Life Drain: Increase rage mode life steal by 10%.");
        return sb.ToString();

    }

}
