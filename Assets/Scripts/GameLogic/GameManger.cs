using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManger : MonoBehaviourPunCallbacks
{
    public static GameManger Instance;
    [TabGroup("Prefabs")]
    public GameObject[] playerPrefab;
    public static GameObject LocalPlayerInstance;
    [TabGroup("Spawning")]
    public bool spawnEnemys;
    [TabGroup("Spawning")]
    [SerializeField] List<Vector3> playerSpawnPoints = new List<Vector3>();
    public delegate void PvPEnable();
    public static event PvPEnable PvPon;
    public delegate void spawn();
    public static event spawn spawnMobs;
    [TabGroup("Game Time")]
    public float GameTimeLeft = 0f;
    [TabGroup("Game Time")]
    [SerializeField] int gameTime;
    public bool timerOn = false;
    bool[] picked = new bool[2];



    private void Awake()
    {
        Instance = this;
        //PhotonNetwork.OfflineMode = true;
    }

    void Start()
    {
        MusicClass.Instance.StopMusic();
        if (PhotonNetwork.IsMasterClient)
        {
            int index = 0;
            
            if (spawnEnemys == true)
                spawnMobs();
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (index >= 2)
                    index = 0;
                photonView.RPC("InstantiationPlayer", player, index);
                index++;

            }
        }
        GameTimeLeft = gameTime;
        timerOn = true;
    }
    [PunRPC]
    void InstantiationPlayer(int index)
    {
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, playerSpawnPoints[index], Quaternion.identity);
        
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
