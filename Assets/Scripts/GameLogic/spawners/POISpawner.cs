using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class POISpawner : Spawner
{
  
    [SerializeField] WeightedRandomList<GameObject> POIs = new WeightedRandomList<GameObject>();
    public override void Spawn()
    {
        GameObject randomPoi = POIs.GetRandom();
        GameObject enemy = PhotonNetwork.Instantiate(randomPoi.name, gameObject.transform.position, randomPoi.transform.rotation);
        this.photonView.RPC("deleteSpawner", RpcTarget.All);
    }
    [PunRPC]
    public override void deleteSpawner()
    {
        Destroy(gameObject);
    }
}
