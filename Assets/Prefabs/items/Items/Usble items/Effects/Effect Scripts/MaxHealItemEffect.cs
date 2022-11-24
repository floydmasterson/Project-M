using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Full Heal")]
public class MaxHealItemEffect : UseableItemEffect
{
    PlayerManger player;
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        player = PlayerUi.Instance.target;
        player.CurrentHealth += player.MaxHealth;
    }

    public override string GetDescription()
    {
        return "Heals to full health.";
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
