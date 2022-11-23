using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Status Effects/Posion")]
public class PosionStatusEffect : StatusEffectSO
{
    Enemys target;
    bool stop = false;
    public int tickDamage;
    public float timeBetweenTicks;
    public int debuffDuration;
    bool isApplyEx = false;
    public override IEnumerator DecayTimer(int time, Enemys enemys)
    {
        Debug.Log("starting decay and effect");
        target.StartCoroutine(Apply());
        yield return new WaitForSecondsRealtime(debuffDuration);
        stop = true;

    }
    public override IEnumerator Apply()
    {
        if (isApplyEx == true)
        {
            Debug.Log("break");
            yield break;
        }
        isApplyEx = true;
        if (stop == false)
        {
            Debug.Log("tick");
            StatusEffect();
            yield return new WaitForSeconds(timeBetweenTicks);
            target.StartCoroutine(Apply());
            isApplyEx = false;
        }
        else
        {
            Debug.Log("effect over");
            target.StopCoroutine(Apply());
            isApplyEx = false;
        }
    }
    public override void StatusEffect()
    {
        target.TakeDamge(tickDamage);
    }

    public override void ApplyEffect(Enemys enenmy)
    {
        Debug.Log("apply");
        target = enenmy;
        Debug.Log(target);
        target.StartCoroutine(DecayTimer(debuffDuration, target));
    }

}
