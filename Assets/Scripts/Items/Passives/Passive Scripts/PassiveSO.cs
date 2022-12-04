using UnityEngine;
using Sirenix.OdinInspector;


public abstract class PassiveSO : ScriptableObject
{
    public abstract void Passive();
    public abstract void ApplyPassive();
    public abstract void RemovePassive();
    public abstract string GetDescription();
    [Button("Apply Passive")]
    public void ApplyPassiveButton()
    {
        ApplyPassive();
    }
    [Button("Remove Passive")]
    public void RemovePassiveButton()
    {
        RemovePassive();
    }
}
