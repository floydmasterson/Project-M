using Kryz.CharacterStats.Examples;
using UnityEngine;

public abstract class UseableItemEffect : ScriptableObject
{
    public abstract void ExecuteEffect(UsableItem parentItem, Character character);
    public abstract string GetDescription();
}
