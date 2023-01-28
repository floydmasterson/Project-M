using System;
using UnityEngine;

public abstract class UseableItemEffect : ScriptableObject
{
    public static event Action<float> PotionSick;
    public float SickTime;
    public virtual void ExecuteEffect(UsableItem parentItem, Character character)
    {
        PotionSicknessCooldown(SickTime);
    }
    public abstract string GetDescription();
    public abstract bool canBeUsed();
    public void PotionSicknessCooldown(float time)
    {
        PotionSick(time);
    }
}
