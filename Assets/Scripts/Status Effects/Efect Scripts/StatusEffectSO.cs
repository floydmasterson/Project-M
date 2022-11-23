using System.Collections;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public abstract IEnumerator DecayTimer(int time, Enemys enemys);
    public abstract IEnumerator Apply();
    public abstract void StatusEffect();
    public abstract void ApplyEffect(Enemys enenmy);
}
