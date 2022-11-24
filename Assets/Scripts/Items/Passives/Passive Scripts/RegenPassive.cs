using System.Collections;
using System.Text;
using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Regen Passive")]
public class RegenPassive : PassiveSO
{
    public float regenAmount;
    public float regenRate;
    private bool remove;
    public int PassiveLvl;
    private StringBuilder sb = new StringBuilder();
    public IEnumerator HealOT()
    {
        if (remove)
        {
            PlayerUi.Instance.target.StopCoroutine(HealOT());
            PlayerUi.Instance.target.CurrentHealth = PlayerUi.Instance.target.CurrentHealth - regenAmount;
            remove = false;

        }
        else
        {
            Passive();
            yield return new WaitForSecondsRealtime(regenRate);
            PlayerUi.Instance.target.StartCoroutine(HealOT());

        }
    }
    public override void Passive()
    {
        PlayerUi.Instance.target.CurrentHealth = PlayerUi.Instance.target.CurrentHealth + regenAmount;
    }
    public override void ApplyPassive()
    {
        PlayerUi.Instance.target.StartCoroutine(HealOT());
    }
    public override void RemovePassive()
    {
        remove = true;
    }

    public override string GetDescription()
    {
        sb.Length = 0;

        if (PassiveLvl > 0)
        {
            sb.Append("Natural Fortitude " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": Regen " + regenAmount + " health every " + regenRate + " seconds.");
            return sb.ToString();
        }
        else
        {
            sb.Append("Natural Fortitude: Regen " + regenAmount + " health every " + regenRate + " seconds.");
            return sb.ToString();
        }
    }
}
