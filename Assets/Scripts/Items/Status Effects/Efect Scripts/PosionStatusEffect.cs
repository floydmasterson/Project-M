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

    public override IEnumerator DecayTimer(int time, PlayerManger player, Enemys enemy)
    {
        if (Ptarget != null)
            Ptarget.StartCoroutine(EffectApplication());
        else if (Etarget != null)
            Etarget.StartCoroutine(EffectApplication());
        yield return new WaitForSeconds(time);
        stop = true;
    }
    public override IEnumerator EffectApplication()
    {
        if (stop == false)
        {

            if (Etarget != null && !Etarget.isDead && Ptarget == null)
            {
                StatusEffect();
                yield return new WaitForSeconds(timeBetweenTicks);
                Etarget.StartCoroutine(EffectApplication());
            }
            else if (Ptarget == null && Etarget != null)
            {
                stop = true;
                Etarget.StartCoroutine(EffectApplication());
            }

            if (Ptarget != null && Ptarget.isAlive && Etarget == null)
            {
                StatusEffect();
                yield return new WaitForSeconds(timeBetweenTicks);
                Ptarget.StartCoroutine(EffectApplication());
            }
            else if (Ptarget != null && Etarget == null)
            {
                stop = true;
                Ptarget.StartCoroutine(EffectApplication());
            }


        }
        else
        {
            if (Ptarget != null && Etarget == null)
            {
                Debug.Log("effect over");
                EffectVFX.DisableEffect(1);
                EffectVFX.posioned = false;
                stop = false;
                Ptarget.StopCoroutine(EffectApplication());
            }
            else if (Ptarget == null && Etarget != null)
            {
                Debug.Log("effect over");
                EffectVFX.DisableEffect(1);
                EffectVFX.posioned = false;
                stop = false;
                Etarget.StopCoroutine(EffectApplication());
            }
        }
    }
    public override void StatusEffect()
    {
        if (Ptarget != null)
            Ptarget.TakeDamge(tickDamage, null);
        if (Etarget != null)
            Etarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffect(object enemy)
    {

        if (enemy.GetType() == typeof(Enemys))
        {
            Etarget = enemy as Enemys;
            EffectVFX = Etarget.GetComponentInChildren<EffectVisualController>(); ;
        }
        else if (enemy.GetType() == typeof(PlayerManger))
        {
            Ptarget = enemy as PlayerManger;
            EffectVFX = Ptarget.GetComponentInChildren<EffectVisualController>(); ;
        }
        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(1);
            EffectVFX.posioned = true;
            if (Etarget != null)
                Etarget.StartCoroutine(DecayTimer(debuffDuration, null, Etarget));
            else if (Ptarget != null)
                Ptarget.StartCoroutine(DecayTimer(debuffDuration, Ptarget, null));
        }
        else
        {
            Debug.Log("already posioned");
        }
    }
    private void Reset()
    {
        stop = false;
    }
    public override StatusEffectSO GetCopy()
    {
        return Instantiate(this);
    }

}