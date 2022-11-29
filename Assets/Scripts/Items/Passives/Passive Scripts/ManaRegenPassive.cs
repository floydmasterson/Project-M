using System.Text;
using UnityEngine;

    [CreateAssetMenu(menuName = "Passives/Mana Regen Passive")]
public class ManaRegenPassive : PassiveSO
{
    public float RegenAmount;
    public int PassiveLvl;
    private MagicController MC;
    private StringBuilder sb = new StringBuilder();
    public override void Passive()
    {
        if (MC != null)
        {
            MC.ManaRegenRate += RegenAmount;
        }
    }
    public override void ApplyPassive()
    {
        MC = PlayerUi.Instance.target.GetComponent<MagicController>();

        if (MC != null)
            Passive();

    }
    public override void RemovePassive()
    {
        if (MC != null)
            MC.ManaRegenRate -= RegenAmount;
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        if(PassiveLvl > 0)
        {
            sb.Append("Mana Flow " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Gain an additonal " + RegenAmount + " mana a second");
            return sb.ToString();
        }
        else
        {

            sb.Append( "Mana Flow: Gain an additonal " + RegenAmount + " mana a second");
            return sb.ToString();
        }
        
    }


}
