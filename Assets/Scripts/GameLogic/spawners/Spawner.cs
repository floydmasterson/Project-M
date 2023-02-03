using Photon.Pun;
using UnityEngine;

public abstract class Spawner : MonoBehaviourPun
{
    [HideInInspector]
    public int spawnChance;
    #region Events

    public void OnEnable()
    {
        GameManger.Instance.spawnMobs += Spawn;
    }
    public void OnDisable()
    {
        GameManger.Instance.spawnMobs -= Spawn;
    }
    #endregion
    public abstract void Spawn();
    public abstract void deleteSpawner();
}
