using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/ Max Mana Gain")]
public class MaxManaItemEffect : UseableItemEffect
{
    MagicController MC;
    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (MC != null)
            MC.CurrMana = MC.MaxMana;
        base.ExecuteEffect(parentItem, character);
    }

    public override string GetDescription()
    {
        return "Fully recover all mana.";
    }
    public override bool canBeUsed()
    {
        MC = PlayerUi.Instance.target.gameObject.GetComponent<MagicController>();
        if (MC != null)
        {
            if (MC.CurrMana == MC.MaxMana)
                return false;
            else
            {
                return true;
            }
        }
        return false;
    }
}
