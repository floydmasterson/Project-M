using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [HideInInspector]
    public int spawnChance;
    #region Events
    public void OnEnable()
    {
        GameManger.spawnMobs += Spawn;
    }
    public void OnDisable()
    {
        GameManger.spawnMobs -= Spawn;
    }
    #endregion
    public abstract void Spawn();
    
}
