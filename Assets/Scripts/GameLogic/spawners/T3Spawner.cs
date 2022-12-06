using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T3Spawner : Spawner
{
    [SerializeField] GameObject[] enemys = new GameObject[1];
    public override void Spawn()
    {
            int randomEnemy = Random.Range(0, enemys.Length);
            GameObject enemy = PhotonNetwork.Instantiate(enemys[randomEnemy].name, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
