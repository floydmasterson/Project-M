using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ChestSpawner : Spawner
{
    [SerializeField] WeightedRandomList<GameObject> chestTable;


    public override void Spawn()
    {
        spawnChance = Random.Range(1, 10);
        if (spawnChance <= 6)
        {
            GameObject chestToSpawn = chestTable.GetRandom();
            PhotonNetwork.Instantiate(chestToSpawn.name, gameObject.transform.position, gameObject.transform.rotation);
        }
        this.photonView.RPC("deleteSpawner", RpcTarget.All);

    }

    [PunRPC]
    public override void deleteSpawner()
    {
        Destroy(gameObject);
    }
    

}
