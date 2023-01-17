using System.Collections;
using System.Text;
using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Regen Passive")]
public class RegenPassive : PassiveSO
{
    public int regenAmount;
    public float regenRate;
    private bool remove;
    public int PassiveLvl;
    private StringBuilder sb = new StringBuilder();
    PlayerManger player;

    private void OnEnable()
    {
        PlayerManger.OnDeath += healthRestart;
    }
    public IEnumerator HealOT()
    {
        if (remove)
        {
            player.StopCoroutine(HealOT());
            player.CurrentHealth = player.CurrentHealth - regenAmount;
            remove = false;
        }
        else
        {
            Passive();
            yield return new WaitForSecondsRealtime(regenRate);
            player.StartCoroutine(HealOT());

        }
    }

    public IEnumerator HealRestart()
    {
        RemovePassive();
        yield return new WaitForSecondsRealtime(4);
        ApplyPassive();
    }
    void healthRestart(PlayerManger player)
    {
       player.StopCoroutine(HealOT());
        if (player == this.player)
            player.StartCoroutine(HealRestart());
    }
    public override void Passive()
    {
        player.Heal(regenAmount);
    }
    public override void ApplyPassive()
    {
        player = PlayerUi.Instance.target;
        player.StartCoroutine(HealOT());
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
