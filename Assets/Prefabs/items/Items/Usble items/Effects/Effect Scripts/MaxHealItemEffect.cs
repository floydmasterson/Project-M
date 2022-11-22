using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Full Heal")]
public class MaxHealItemEffect : UseableItemEffect
{
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        PlayerUi.Instance.target.CurrentHealth += PlayerUi.Instance.target.MaxHealth;
    }

    public override string GetDescription()
    {
        return "Heals to full health.";
    }
}
