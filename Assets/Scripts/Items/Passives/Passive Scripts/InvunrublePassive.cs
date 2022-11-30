using System.Collections;
using System.Text;
using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Invulnerable Passive")]
public class InvunrublePassive : PassiveSO
{
    public int timeBetween;
    public int invulnerableTime;
    public int PassiveLvl;
    private bool stop = false;
    private bool isEx = false;
    StringBuilder sb = new StringBuilder();
    private IEnumerator TimedInv()
    {
        if (isEx)
            yield break;
        isEx = true;
        if (stop == false)
        {
            Passive(true);
            PlayerUi.Instance.target.GetComponentInChildren<EffectVisualController>().EnableEffect(0);
            yield return new WaitForSecondsRealtime(invulnerableTime);
            Passive(false);
            PlayerUi.Instance.target.GetComponentInChildren<EffectVisualController>().DisableEffect(0);
            yield return new WaitForSecondsRealtime(timeBetween);
            isEx = false;
            ApplyPassive();
        }
        else
        {
            Passive(false);
            PlayerUi.Instance.target.StopCoroutine(TimedInv());
            isEx = false;
            stop = false;
        }
    }
    public void Passive(bool state)
    {

        PlayerUi.Instance.target.isInvulnerable = state;
    }
    public override void ApplyPassive()
    {
        PlayerUi.Instance.target.StartCoroutine(TimedInv());
    }
    public override void RemovePassive()
    {
        stop = true;
    }
    public override string GetDescription()
    {
        sb.Length = 0;
        if (PassiveLvl > 0)
        {
            sb.Append("Divine Intervention " + RomanNumerals.RomanNumeralGenerator(PassiveLvl) + ": You are invulnerable for " + invulnerableTime + " every " + timeBetween + " seconds.");
            return sb.ToString();
        }
        else
        {
            sb.Append("Divine Intervention: You are invulnerable for " + invulnerableTime + " every " + timeBetween + " seconds.");
            return sb.ToString();
        }
    }
    public override void Passive()
    {
        Debug.Log("Unused method");
    }
    private void Reset()
    {
        stop = false;
        isEx = false;
    }
}
