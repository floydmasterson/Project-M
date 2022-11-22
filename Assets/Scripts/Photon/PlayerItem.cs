using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerName;
    Image backgroundImage;
    public Color highlightColor;
    public GameObject Left;
    public GameObject Right;
    public Image playerAvatar;
    public Sprite[] avatars;
    Player player;

    ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();

    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);

    }

    public void ApplyLocalChanges()
    {
        backgroundImage.color = highlightColor;
        Left.SetActive(true);
        Right.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        if ((int)playerProp["playerAvatar"] == 0)
        {
            playerProp["playerAvatar"] = avatars.Length -1;
        }
        else
        {
            playerProp["playerAvatar"] = (int)playerProp["playerAvatar"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProp);
    }

    public void OnClickRightArrow()
    {
        if ((int)playerProp["playerAvatar"] == avatars.Length - 1)
        {
            playerProp["playerAvatar"] = 0;
        }
        else
        {
            playerProp["playerAvatar"] = (int)playerProp["playerAvatar"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProp);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }

    }
    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProp["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
       
        }
        else
        {
            playerProp["playerAvatar"] = 0;
        }
    }
}
