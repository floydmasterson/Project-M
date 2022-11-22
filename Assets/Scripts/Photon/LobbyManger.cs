using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManger : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInput;
    public GameObject lobby;
    public GameObject room;
    public TextMeshProUGUI roomName;
    public static LobbyManger Instnace;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;
    public float timeBUpdate = 1.5f;
    float nextUpdate;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;


    private void Start()
    {
        PhotonNetwork.JoinLobby();
        Instnace = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (roomInput.text.Length >= 1)
            {
                PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions() { MaxPlayers = 4 });
            }
        }
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
      
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("Game");
        playButton.SetActive(false);
    }

    public void OnClickCreate()
    {
        if (roomName.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true});
        }
    }

    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        room.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        if (Time.time >= nextUpdate)
        {
            UpdateRoomList(roomList);
            nextUpdate = Time.time + timeBUpdate;
        }

       
    }

    public void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
           Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }

    }

    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        lobby.SetActive(true);
        room.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();
        
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayerItem);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerEnteredRoom(otherPlayer);
        UpdatePlayerList();
    }

    
}
