using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviourPunCallbacks
{
    public static GameManger Instance;
    [TabGroup("Prefabs")]
    public GameObject[] playerPrefab;
    [TabGroup("Prefabs")]
    public GameObject Sector3;
    [TabGroup("Prefabs")]
    public GameObject Sector1;
    [TabGroup("Prefabs")]
    public GameObject Sector5;
    public static GameObject LocalPlayerInstance;
    [TabGroup("Spawning")]
    public bool spawnEnemys;
    [TabGroup("Spawning")]
    public bool spawnPlayersTogther;
    [TabGroup("Spawning")]
    [SerializeField] List<RespawnPoint> playerSpawnPoints = new List<RespawnPoint>();
    public delegate void PvPEnable();
    public static event PvPEnable PvPon;
    public delegate void spawn();
    public static event spawn spawnMobs;
    [TabGroup("Game Time")]
    public float GameTimeLeft = 0f;
    [TabGroup("Game Time")]
    [SerializeField] public int gameTime;
    public bool timerOn = false;
    bool[] picked = new bool[2];
    [TabGroup("Audio")]
    public SFX dungonAmbient;



    private void Awake()
    {
        Instance = this;
    }
    private void TimesUp()
    {
        Debug.Log("time is up");
        GameTimeLeft = 0;
        timerOn = false;
        PvPon();
        Sector1.SetActive(false);
        Sector5.SetActive(false);
        Sector3.SetActive(true);
        if (spawnEnemys == true && spawnMobs != null)
            spawnMobs();
    }

    void Start()
    {
        if (MusicClass.Instance != null)
            MusicClass.Instance.StopMusic();
        dungonAmbient.PlaySFX();
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!spawnPlayersTogther)
                {
                    int selctedSpawn = Random.Range(0, playerSpawnPoints.Count);
                    photonView.RPC("InstantiationPlayer", player, selctedSpawn);
                    photonView.RPC("Removepoint", RpcTarget.All, selctedSpawn);
                }
                else if(spawnPlayersTogther)
                {
                    int selctedSpawn = 0;
                    photonView.RPC("InstantiationPlayer", player, selctedSpawn);
                    photonView.RPC("Removepoint", RpcTarget.All, selctedSpawn);
                }
            }
            if (spawnEnemys == true && spawnMobs != null)
                spawnMobs();
            SetGameTime(gameTime);
        }
    }
    [PunRPC]
    void InstantiationPlayer(int spawnPoint)
    {
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, playerSpawnPoints[spawnPoint].spawnPoint.transform.position, Quaternion.identity);
    }
    [PunRPC]
    void Removepoint(int spawnPoint)
    {
        playerSpawnPoints.Remove(playerSpawnPoints[spawnPoint]);
    }


    private void Update()
    {
        if (timerOn)
        {
            if (GameTimeLeft > 0)
            {
                GameTimeLeft -= Time.deltaTime;
            }
            else if (GameTimeLeft < 0)
            {
                TimesUp();
            }
        }
    }

    public void SetGameTime(float time)
    {
        photonView.RPC("SetGameTimeRPC", RpcTarget.AllBuffered, time);
    }
    [PunRPC]
    public void SetGameTimeRPC(float time)
    {
        GameTimeLeft = time;
        if (time > 0)
            timerOn = true;
    }
}
