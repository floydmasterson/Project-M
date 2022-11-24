using UnityEngine;


public abstract class PassiveSO : ScriptableObject
{
    public abstract void Passive();
    public abstract void ApplyPassive();
    public abstract void RemovePassive();
    public abstract string GetDescription();
}
