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
    [TabGroup("Spawning")]
    public bool spawnEnemys;
    [TabGroup("Spawning")]
    [SerializeField] Vector3[] playerSpawnPoints;
    public delegate void PvPEnable();
    public static event PvPEnable PvPon;
    public delegate void spawn();
    public static event spawn spawnMobs;
    [TabGroup("Game Time")]
    public float GameTimeLeft = 0f;
    [TabGroup("Game Time")]
    [SerializeField] int gameTime;
    bool timerOn = false;
    bool[] picked = new bool[2];

    private void Awake()
    {
        Instance = this;
        //PhotonNetwork.OfflineMode = true;
    }

    void Start()
    {
        SpawnPlayers();
        //MusicClass.Instance.StopMusic();
        if (PhotonNetwork.IsMasterClient && spawnEnemys)
        {
            spawnMobs();
        }
        GameTimeLeft = gameTime;
        timerOn = true;
    }
    private void SpawnPlayers()
    {
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, RandomSpawn(), Quaternion.identity);
    }


    Vector3 RandomSpawn()
    {
        for (int i = 0; i < playerSpawnPoints.Length; i++)
        {
            int selected = Random.Range(0, playerSpawnPoints.Length);
            Debug.Log(selected);
            Vector3 selectedSpawn;
            if (!picked[selected])
            {
                picked[selected] = true;
                selectedSpawn = playerSpawnPoints[selected];
                return selectedSpawn;
            }
            else
            {
                RandomSpawn();
            }
        }
        return Vector3.zero;
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
