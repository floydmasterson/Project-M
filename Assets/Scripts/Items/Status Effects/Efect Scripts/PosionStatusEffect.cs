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
    public override IEnumerator DecayTimerEnemy(int time)
    {
        time = debuffDuration;
        Etarget.StartCoroutine(ApplyEnemy());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator ApplyEnemy()
    {
        if (stop == false)
        {
            
            if (!Etarget.isDead)
            {
                StatusEffectEnemy();
                yield return new WaitForSeconds(timeBetweenTicks);
                Etarget.StartCoroutine(ApplyEnemy());
            }
            else
            {
                stop = true;
                Etarget.StartCoroutine(ApplyEnemy());
            }
        }
        else
        {
            Etarget.StopCoroutine(ApplyEnemy());
            EffectVFX.DisableEffect(1);
            EffectVFX.posioned = false;
            stop = false;
        }
    }
    public override void StatusEffectEnemy()
    {
        Etarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffectEnemy(Enemys enenmy)
    {
        Etarget = enenmy;
        EffectVFX = Etarget.GetComponentInChildren<EffectVisualController>(); ;
        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(1);
            EffectVFX.posioned = true;
            Etarget.StartCoroutine(DecayTimerEnemy(debuffDuration));
        }
        else
        {
            Debug.Log("already posioned");
        }
    }
    #endregion
    #region Player
    public override IEnumerator DecayTimerPlayer(int time)
    {
        time = debuffDuration;
        Ptarget.StartCoroutine(ApplyPlayer());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator ApplyPlayer()
    {
        if (stop == false)
        {

            if (Ptarget.isAlive)
            {
                StatusEffectPlayer();
                yield return new WaitForSeconds(timeBetweenTicks);
                Ptarget.StartCoroutine(ApplyPlayer());
            }
            else
            {
                stop = true;
                Ptarget.StartCoroutine(ApplyPlayer());
            }
        }
        else
        {
            Debug.Log("effect over");
            Ptarget.StopCoroutine(ApplyEnemy());
            EffectVFX.DisableEffect(1);
            EffectVFX.posioned = false;
            stop = false;
        }
    }
    public override void StatusEffectPlayer()
    {
        Ptarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffectPlayer(PlayerManger enemy)
    {
        Ptarget = enemy;
        EffectVFX = Ptarget.GetComponentInChildren<EffectVisualController>();
        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(1);
            EffectVFX.posioned = true;
            Ptarget.StartCoroutine(DecayTimerPlayer(debuffDuration));
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