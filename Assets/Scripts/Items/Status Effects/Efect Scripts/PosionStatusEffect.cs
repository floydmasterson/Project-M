using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Status Effects/Posion Effect")]
public class PosionStatusEffect : StatusEffectSO
{
    Enemys Etarget;
    PlayerManger Ptarget;
    EffectVisualController EffectVFX;
    bool stop = false;
    public int tickDamage;
    public float timeBetweenTicks;
    public int debuffDuration;

    #region Enemy
    public override IEnumerator DecayTimerE(int time)
    {
        time = debuffDuration;
        Debug.Log("starting decay and effect");
        Etarget.StartCoroutine(ApplyE());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator ApplyE()
    {
        if (stop == false)
        {
            
            if (!Etarget.isDead)
            {
                StatusEffectE();
                yield return new WaitForSeconds(timeBetweenTicks);
                Etarget.StartCoroutine(ApplyE());
            }
            else
            {
                stop = true;
                Etarget.StartCoroutine(ApplyE());
            }
        }
        else
        {
            Debug.Log("effect over");
            Etarget.StopCoroutine(ApplyE());
            EffectVFX.DisableEffect(1);
            EffectVFX.posioned = false;
            stop = false;
        }
    }
    public override void StatusEffectE()
    {
        Etarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffectE(Enemys enenmy)
    {
        Etarget = enenmy;
        EffectVFX = Etarget.GetComponentInChildren<EffectVisualController>(); ;
        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(1);
            EffectVFX.posioned = true;
            Etarget.StartCoroutine(DecayTimerE(debuffDuration));
        }
        else
        {
            Debug.Log("already posioned");
        }
    }
    #endregion
    #region Player
    public override IEnumerator DecayTimerP(int time)
    {
        time = debuffDuration;
        Debug.Log("starting decay and effect");
        Ptarget.StartCoroutine(ApplyP());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator ApplyP()
    {
        if (stop == false)
        {

            if (Ptarget.isAlive)
            {
                StatusEffectP();
                yield return new WaitForSeconds(timeBetweenTicks);
                Ptarget.StartCoroutine(ApplyP());
            }
            else
            {
                stop = true;
                Ptarget.StartCoroutine(ApplyP());
            }
        }
        else
        {
            Debug.Log("effect over");
            Ptarget.StopCoroutine(ApplyE());
            EffectVFX.DisableEffect(1);
            EffectVFX.posioned = false;
            stop = false;
        }
    }
    public override void StatusEffectP()
    {
        Ptarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffectP(PlayerManger enemy)
    {
        Ptarget = enemy;
        EffectVFX = Ptarget.GetComponentInChildren<EffectVisualController>();
        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(1);
            EffectVFX.posioned = true;
            Ptarget.StartCoroutine(DecayTimerP(debuffDuration));
        }
        else
        {
            Debug.Log("already posioned");
        }
    }
    #endregion
    private void Reset()
    {
        stop = false;
    }
    public override StatusEffectSO GetCopy()
    {
        return Instantiate(this);
    }

}