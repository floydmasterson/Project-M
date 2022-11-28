using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManger : MonoBehaviourPun
{
    public GameObject[] playerPrefab;
    public static GameObject LocalPlayerInstance;
    public GameObject enemyPrefab;
    public static GameObject LocalenemyInstance;
    [SerializeField] private GameObject[] enemys = new GameObject[2];
    [SerializeField] private Transform[] spawners = new Transform[3];
    int randomPoint, randomEnemy;
    [Space]
    public bool spawnEnemys = false;


    private void Awake()
    {
        //PhotonNetwork.OfflineMode = true;
    }

    void Start()
    {
        //MusicClass.Instance.StopMusic();
        //spawn player
        Vector3 playerspawn = new Vector3(25f, 3f, -45f);
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        GameObject player = PhotonNetwork.Instantiate(playerToSpawn.name, playerspawn, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient && spawnEnemys)
        {
            Vector3 enemyspawn = new Vector3(10f, 0f, 10f);
            int count = 30;
            for (int i = 0; i < count; i++)
            {
                randomPoint = Random.Range(0, spawners.Length);
                randomEnemy = Random.Range(0, enemys.Length);
                GameObject enemy = PhotonNetwork.Instantiate(enemys[randomEnemy].name, spawners[randomPoint].position, Quaternion.identity);
                enemy.transform.position = new Vector3(enemy.transform.position.x + (Random.Range(-5, 5)), enemy.transform.position.y, enemy.transform.position.z + Random.Range(-5, 5));
            }

        }




    }



}
