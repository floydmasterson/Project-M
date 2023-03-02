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
    public float debuffDuration;

    public override IEnumerator DecayTimer(float time, PlayerManger player, Enemys enemy)
    {
        if (enemy != null)
            enemy.StartCoroutine(EffectApplication());
        else if (player != null)
            player.StartCoroutine(EffectApplication());
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
                EffectVFX.DisableEffect(EffectVisualController.Effects.Poisoned);
                EffectVFX.posioned = false;
                stop = false;
                Ptarget.StopCoroutine(EffectApplication());
            }
            else if (Ptarget == null && Etarget != null)
            {
                EffectVFX.DisableEffect(EffectVisualController.Effects.Poisoned);
                EffectVFX.posioned = false;
                stop = false;
                Etarget.StopCoroutine(EffectApplication());
            }
        }
    }
    public override void StatusEffect()
    {
        if (Ptarget != null)
            Ptarget.TakeDamge(tickDamage);
        if (Etarget != null)
            Etarget.TakeDamge(tickDamage);
    }
    public override void ApplyEffect(object enemy)
    {

        if (enemy is Enemys)
        {
            Etarget = enemy as Enemys;
            EffectVFX = Etarget.GetComponentInChildren<EffectVisualController>();
        }
        else if (enemy is PlayerManger)
        {
            Ptarget = enemy as PlayerManger;
            EffectVFX = Ptarget.GetComponentInChildren<EffectVisualController>();
        }

        if (EffectVFX.posioned == false)
        {
            EffectVFX.EnableEffect(EffectVisualController.Effects.Poisoned);
            EffectVFX.posioned = true;
            if (Etarget != null)
                Etarget.StartCoroutine(DecayTimer(debuffDuration, null, Etarget));
            else if (Ptarget != null)
                Ptarget.StartCoroutine(DecayTimer(debuffDuration, Ptarget, null));
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