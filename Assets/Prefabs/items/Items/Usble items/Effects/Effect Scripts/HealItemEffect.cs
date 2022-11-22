using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Heal")]
public class HealItemEffect : UseableItemEffect
{
    public int HealAmount;
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        PlayerUi.Instance.target.CurrentHealth += HealAmount;
    }

    public override string GetDescription()
    {
        return "Heals for " + HealAmount + " health.";
    }
}
