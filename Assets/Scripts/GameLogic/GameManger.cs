using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManger : MonoBehaviourPun
{
    public static GameManger Instance;
    [TabGroup("Prefabs")]
    public GameObject[] playerPrefab;
    public static GameObject LocalPlayerInstance;
    GameObject enemyPrefab;
    public static GameObject LocalenemyInstance;
    [TabGroup("Prefabs")]
    [SerializeField] GameObject[] enemys = new GameObject[2];
    [TabGroup("Spawners")]
    [SerializeField] Transform[] spawners = new Transform[3];
    int randomPoint, randomEnemy;
    [TabGroup("Spawning")]
    public bool spawnEnemys = false;
    [TabGroup("Spawning")]
    [SerializeField] int enemyAmount;
    public delegate void PvPEnable();
    public static event PvPEnable PvPon;
    [TabGroup("Spawning")]
    public float GameTimeLeft = 0f;
    [TabGroup("Spawning")]
    [SerializeField] int gameTime;
    bool timerOn = false;

    private void Awake()
    {
        Instance = this;
        //PhotonNetwork.OfflineMode = true;
    }

    void Start()
    {
        //MusicClass.Instance.StopMusic();
        SpawnPlayer();
        if (PhotonNetwork.IsMasterClient && spawnEnemys)
        {
            SpawnEnemys(enemyAmount);
        }
        GameTimeLeft = gameTime;
        timerOn = true;
    }
    private void SpawnPlayer()
    {
        Vector3 playerspawn = new Vector3(25f, 3f, -45f);
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        GameObject player = PhotonNetwork.Instantiate(playerToSpawn.name, playerspawn, Quaternion.identity);
    }
    private void SpawnEnemys(int count)
    {
        for (int i = 0; i < count; i++)
        {
            randomPoint = Random.Range(0, spawners.Length);
            randomEnemy = Random.Range(0, enemys.Length);
            GameObject enemy = PhotonNetwork.Instantiate(enemys[randomEnemy].name, spawners[randomPoint].position, Quaternion.identity);
            enemy.transform.position = new Vector3(enemy.transform.position.x + (Random.Range(-5, 5)), enemy.transform.position.y, enemy.transform.position.z + Random.Range(-5, 5));
        }
    }
    private void Update()
    {
        if (timerOn)
        {
            if (GameTimeLeft > 0)
            {
                GameTimeLeft -= Time.deltaTime;
            }
            else
            {
                Debug.Log("time is up");
                GameTimeLeft = 0;
                timerOn = false;
                PvPon();
                //despawn enemys 
                //despawn chests?
            }
        }
    }



}
