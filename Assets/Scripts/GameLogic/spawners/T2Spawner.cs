using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T2Spawner : Spawner
{
    [SerializeField] GameObject[] enemys = new GameObject[1];
    public override void Spawn()
    {
        spawnChance = Random.Range(1, 10);

        if (spawnChance <= 9)
        {
            int randomEnemy = Random.Range(0, enemys.Length);
            GameObject enemy = PhotonNetwork.Instantiate(enemys[randomEnemy].name, gameObject.transform.position, Quaternion.identity);
        }

        this.photonView.RPC("deleteSpawner", RpcTarget.All);
    }
    [PunRPC]
    public override void deleteSpawner()
    {
        Destroy(gameObject);
    }
}
