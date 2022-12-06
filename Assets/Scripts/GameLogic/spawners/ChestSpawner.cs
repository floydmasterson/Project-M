using Photon.Pun;
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
            PhotonNetwork.Instantiate(chestToSpawn.name, gameObject.transform.position, chestToSpawn.transform.rotation);
        }
        Destroy(gameObject);

    }
}
