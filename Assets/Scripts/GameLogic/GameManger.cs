using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManger Instance { get; private set; }
    [TabGroup("Prefabs")]
    public GameObject[] playerPrefab;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector1;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector2;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector3;
    [TabGroup("Prefabs"), SerializeField]
    private GameObject Sector4;
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
    [TabGroup("Spawning"), SerializeField]
    private List<RespawnPoint> playerSpawnPoints = new List<RespawnPoint>();
    [TabGroup("Hidden Rooms"), SerializeField]
    private WeightedRandomList<HiddenRoom> Sector1HiddenRooms = new WeightedRandomList<HiddenRoom>();
    [TabGroup("Hidden Rooms"), SerializeField]
    private WeightedRandomList<HiddenRoom> Sector2HiddenRooms = new WeightedRandomList<HiddenRoom>();
    [TabGroup("Hidden Rooms"), SerializeField]
    private WeightedRandomList<HiddenRoom> Sector3HiddenRooms = new WeightedRandomList<HiddenRoom>();
    [TabGroup("Hidden Rooms"), SerializeField]
    private WeightedRandomList<HiddenRoom> Sector4HiddenRooms = new WeightedRandomList<HiddenRoom>();
    [TabGroup("Hidden Rooms"), SerializeField]
    private WeightedRandomList<HiddenRoom> Sector5HiddenRooms = new WeightedRandomList<HiddenRoom>();
    public event Action spawnMobs;
    private bool timerOn = false;


    private void Awake()
    {
        Instance = this;
        PhotonNetwork.AddCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.VoteToSkip && PhotonNetwork.IsMasterClient)
        {
            timerOn = false;
            SetGameTime(0);
        }
    }
    void Start()
    {
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
            SelectHiddenRooms();
            SetGameTime(gameTime);
        }
    }

    private void SelectHiddenRooms()
    {
        HiddenRoom room1 = Sector1HiddenRooms.GetRandom();
        HiddenRoom room2 = Sector2HiddenRooms.GetRandom();
        HiddenRoom room3 = Sector3HiddenRooms.GetRandom();
        HiddenRoom room4 = Sector4HiddenRooms.GetRandom();
        HiddenRoom room5 = Sector5HiddenRooms.GetRandom();
        room1.Selected();
        if (1 == 2)
        {
        room2.Selected();
        room3.Selected();
        room4.Selected();
        room5.Selected();         
        }
    }

    [PunRPC]
    void InstantiationPlayer(int spawnPoint)
    {
        GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        playerSpawnPoints[spawnPoint].Selcted(playerToSpawn);
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
                GameTimeLeft = 0;
            }
        }
    }
    public void SetGameTime(float time)
    {
        photonView.RPC("SetGameTimeRPC", RpcTarget.AllBuffered, time);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    [PunRPC]
    public void SetGameTimeRPC(float time)
    {
        GameTimeLeft = time;
        if (time > 0)
            timerOn = true;
    }
}
