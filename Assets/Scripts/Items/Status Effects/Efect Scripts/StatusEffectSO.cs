using System.Collections;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public abstract IEnumerator DecayTimer(float time,PlayerManger player, Enemys enemy);
    public abstract IEnumerator EffectApplication();
    public abstract void StatusEffect();
    public abstract void ApplyEffect(object enemy);
    public abstract StatusEffectSO GetCopy();
}
