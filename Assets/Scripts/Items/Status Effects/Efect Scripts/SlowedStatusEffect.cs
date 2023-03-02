using Kryz.CharacterStats;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName = "Status Effects/Slow Effect")]
public class SlowedStatusEffect : StatusEffectSO
{
    private Enemys Etarget;
    private PlayerManger Ptarget;
    private EffectVisualController EffectVFX;
    private bool stop = false;
    [SerializeField]
   private Item Slowed;
    [SerializeField]
    private float debuffDuration;
    [SerializeField]
    private float slowAmount;
    private float enemyStartSpeed;
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

        if (EffectVFX.slowed == false)
        {
            EffectVFX.EnableEffect(EffectVisualController.Effects.Slowed);
            EffectVFX.slowed = true;
            if (Etarget != null)
                Etarget.StartCoroutine(DecayTimer(debuffDuration, null, Etarget));
            else if (Ptarget != null)
                Ptarget.StartCoroutine(DecayTimer(debuffDuration, Ptarget, null));
        }


    }

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
                yield return new WaitForSeconds(debuffDuration);
                stop = true;
                Etarget.StartCoroutine(EffectApplication());
            }

            if (Ptarget != null && Ptarget.isAlive && Etarget == null)
            {
                StatusEffect();
                yield return new WaitForSeconds(debuffDuration);
                stop = true;
                Ptarget.StartCoroutine(EffectApplication());
            }

        }
        else
        {
            if (Ptarget != null && Etarget == null)
            {
                EffectVFX.DisableEffect(EffectVisualController.Effects.Slowed);
                EffectVFX.slowed = false;
                stop = false;
                Ptarget.StopCoroutine(EffectApplication());
            }
            else if (Ptarget == null && Etarget != null)
            {
                EffectVFX.DisableEffect(EffectVisualController.Effects.Slowed);
                EffectVFX.slowed = false;
                stop = false;
                Etarget.StopCoroutine(EffectApplication());
            }
        }
    }

    public override StatusEffectSO GetCopy()
    {
        return Instantiate(this);
    }

    public override void StatusEffect()
    {
        if (Ptarget != null)
            Ptarget.character.Agility.AddModifier(new StatModifier(slowAmount, StatModType.PercentMult, Slowed));
        if (Etarget != null)
        {
            NavMeshAgent enemyAgent = Etarget.gameObject.GetComponent<NavMeshAgent>();
            enemyStartSpeed = enemyAgent.speed;
            enemyAgent.speed = enemyAgent.speed - (enemyAgent.speed * slowAmount);
        }

    }
}
