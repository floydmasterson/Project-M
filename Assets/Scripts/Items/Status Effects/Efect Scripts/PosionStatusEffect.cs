using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Status Effects/Posion Effect")]
public class PosionStatusEffect : StatusEffectSO
{
    Enemys target;
    GameObject posionParticle;
    bool stop = false;
    public int tickDamage;
    public float timeBetweenTicks;
    public int debuffDuration;
    public override IEnumerator DecayTimer(int time)
    {
        time = debuffDuration;
        Debug.Log("starting decay and effect");
        target.StartCoroutine(Apply());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator Apply()
    {
        if (stop == false)
        {
            Debug.Log("tick");
            if (!target.isDead)
            {
                StatusEffect();
                yield return new WaitForSeconds(timeBetweenTicks);
                target.StartCoroutine(Apply());
            }
            else
            {
                stop = true;
                posionParticle.SetActive(false);
            }
        }
        else
        {
            Debug.Log("effect over");
            target.StopCoroutine(Apply());
            posionParticle.SetActive(false);
            target.posioned = false;
            stop = false;
        }
    }
    private void Reset()
    {
        stop = false;
    }
    public override void StatusEffect()
    {
        target.TakeDamge(tickDamage);
    }
    public override void ApplyEffect(Enemys enenmy)
    {
        target = enenmy;
        posionParticle = target.gameObject.transform.GetChild(0).gameObject;
        if (target.posioned == false)
        {
            posionParticle.SetActive(true);
            target.posioned = true;
            target.StartCoroutine(DecayTimer(debuffDuration));
        }
        else
        {
            Debug.Log("already posioned");
        }
    }
    public override StatusEffectSO GetCopy()
    {
        return Instantiate(this);
    }
}