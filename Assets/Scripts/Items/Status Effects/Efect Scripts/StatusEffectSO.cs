using System.Collections;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public abstract IEnumerator DecayTimerE(int time);
    public abstract IEnumerator ApplyE();
    public abstract IEnumerator DecayTimerP(int time);
    public abstract IEnumerator ApplyP();
    public abstract void StatusEffectE();
    public abstract void StatusEffectP();
    public abstract void ApplyEffectE(Enemys enenmy);
    public abstract void ApplyEffectP(PlayerManger enemy);
    public abstract StatusEffectSO GetCopy();
}
