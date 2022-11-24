using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Heal")]
public class HealItemEffect : UseableItemEffect
{
    public int HealAmount;
    PlayerManger player;

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        player.Heal(HealAmount);
    }

    public override string GetDescription()
    {
        return "Heals for " + HealAmount + " health.";
    }
    public override bool canBeUsed()
    {
        player = PlayerUi.Instance.target;
        if (player.CurrentHealth == player.MaxHealth)
            return false;
        else
            return true;
    }
}
