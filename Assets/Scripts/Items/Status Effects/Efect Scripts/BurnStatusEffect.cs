using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Burning Effect")]
public class BurnStatusEffect : StatusEffectSO
{
    private Enemys Etarget;
    private PlayerManger Ptarget;
    private EffectVisualController EffectVFX;
    private bool stop = false;
    [SerializeField]
    private int tickDamage;
    [SerializeField]
    private float timeBetweenTicks;
    [SerializeField]
    private float debuffDuration;
    [SerializeField]
    private float spreadRange;
    public bool hasSpread;

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
                EffectVFX.DisableEffect(EffectVisualController.Effects.Burned);
                EffectVFX.burning = false;
                stop = false;
                Ptarget.StopCoroutine(EffectApplication());
            }
            else if (Ptarget == null && Etarget != null)
            {
                EffectVFX.DisableEffect(EffectVisualController.Effects.Burned);
                EffectVFX.burning = false;
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
        if (!hasSpread)
            CheckSpread();
    }

    private void CheckSpread()
    {
        Collider[] hitenemines = Physics.OverlapSphere(Etarget.transform.position, spreadRange, PlayerUi.Instance.target.enemyLayers);
        {
            for (int i = 0; i < hitenemines.Length; i++)
            {
                Transform target = hitenemines[i].transform;
                PlayerManger player = target.GetComponent<PlayerManger>();
                Enemys Etarget = target.GetComponent<Enemys>();
                if (player != null && player != PlayerUi.Instance.target)
                {
                    StatusEffectSO eCopy = GetCopy();
                    BurnStatusEffect bCopy = eCopy as BurnStatusEffect;
                    bCopy.hasSpread = true;
                    bCopy.ApplyEffect(player);
                }
                if (Etarget != null)
                {
                    StatusEffectSO eCopy = GetCopy();
                    BurnStatusEffect bCopy = eCopy as BurnStatusEffect;
                    bCopy.hasSpread = true;
                    bCopy.ApplyEffect(Etarget);
                }
            }
        }
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

        if (EffectVFX.burning == false)
        {
            EffectVFX.EnableEffect(EffectVisualController.Effects.Burned);
            EffectVFX.burning = true;
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
