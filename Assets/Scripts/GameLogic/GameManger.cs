using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviourPunCallbacks
{
    public static GameManger Instance { get; private set; }
    [TabGroup("Prefabs")]
    public GameObject[] playerPrefab;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector3;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector1;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector5;
    [TabGroup("Spawning"), SerializeField]
    private bool spawnEnemys;
    [TabGroup("Spawning"), SerializeField]
    private bool spawnPlayersTogther;
    [TabGroup("Game Time")]
    public float GameTimeLeft = 0f;
    [TabGroup("Game Time"), SerializeField]
    public int gameTime;
    [TabGroup("Audio"), SerializeField]
    private SFX dungonAmbient;
    [SerializeField]
    private List<RespawnPoint> playerSpawnPoints = new List<RespawnPoint>();

    public event Action TimerOver;
    public event Action spawnMobs;
    private bool timerOn = false;


    private void Awake()
    {
        Instance = this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        VotingSystem.Instance.timeSkip += T2Load;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        VotingSystem.Instance.timeSkip -= T2Load;
    }
    private void T2Load()
    {
        timerOn = false;
        GameTimeLeft = 0;
        Sector1.SetActive(false);
        Sector5.SetActive(false);
        Sector3.SetActive(true);
        if (spawnEnemys == true && PhotonNetwork.IsMasterClient)
            spawnMobs?.Invoke();
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
                    int selctedSpawn = UnityEngine.Random.Range(0, playerSpawnPoints.Count);
                    photonView.RPC("InstantiationPlayer", player, selctedSpawn);
                    photonView.RPC("Removepoint", RpcTarget.AllBuffered, selctedSpawn); 
                }
                else if (spawnPlayersTogther)
                {
                    int selctedSpawn = 0;
                    photonView.RPC("InstantiationPlayer", player, selctedSpawn);
                    photonView.RPC("Removepoint", RpcTarget.AllBuffered, selctedSpawn); 
                }
            }
            if (spawnEnemys == true)
                spawnMobs?.Invoke();

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

                Debug.Log("time is up");
                timerOn = false;
                TimerOver?.Invoke();
                GameTimeLeft = 0;
                T2Load();
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
