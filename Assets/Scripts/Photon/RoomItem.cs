using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;

    public void SetRoomName(string _roomName)
        {
        roomName.text = _roomName; 
        }

    public void OnClickItem()
    {
        LobbyManger.Instnace.JoinRoom(roomName.text);
    }
}
