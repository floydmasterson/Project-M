using System.Collections;
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    public abstract IEnumerator DecayTimerEnemy(int time);
    public abstract IEnumerator ApplyEnemy();
    public abstract IEnumerator DecayTimerPlayer(int time);
    public abstract IEnumerator ApplyPlayer();
    public abstract void StatusEffectEnemy();
    public abstract void StatusEffectPlayer();
    public abstract void ApplyEffectEnemy(Enemys enenmy);
    public abstract void ApplyEffectPlayer(PlayerManger enemy);
    public abstract StatusEffectSO GetCopy();
}
