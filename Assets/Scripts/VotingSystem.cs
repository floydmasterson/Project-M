using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VotingSystem : MonoBehaviourPunCallbacks
{
    private int requiredVotes;
    private int votesReceived = 0;
    private Dictionary<int, bool> voteTracker = new Dictionary<int, bool>();

    private Canvas popup;
    [SerializeField] private ShopPortal shopPortal;
    [SerializeField]
    private InputAction SkipButton;
    [SerializeField]
    private Collider col;
    [SerializeField]
    private GameObject Light;
    private int PlayerId;
    [SerializeField]
    private timeToText display;

    private void Awake()
    {
        PlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        popup = GetComponentInChildren<Canvas>(true);
        SkipButton.performed += ctx => CastVote(PlayerId);

    }
    private void Update()
    {
        if (PhotonNetwork.InRoom && requiredVotes != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            requiredVotes = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
    private void Start()
    {
        if (GameManger.Instance != null)
            Destroy(gameObject, GameManger.Instance.gameTime + 60);
    }
    private void CastVote(int playerId)
    {
        photonView.RPC("SyncVote", RpcTarget.All, playerId);
        Light.SetActive(true);
        gameObject.SetActive(false);
        SkipButton.performed -= ctx => CastVote(PlayerId);
    }
    [PunRPC]
    private void SyncVote(int playerId)
    {
        if (!voteTracker.ContainsKey(playerId))
        {
            voteTracker[playerId] = true;
            votesReceived++;
            display.updateVote(votesReceived);

            if (votesReceived >= requiredVotes)
            {
                object[] eventData = new object[] { "Vote Replay Event Data" }; // Event data to send
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                PhotonNetwork.RaiseEvent(EventCodes.VoteToSkip, eventData, raiseEventOptions, SendOptions.SendReliable);

                PhotonNetwork.SendAllOutgoingCommands();
                Debug.LogWarning("Shop time was skipped");
                display.gameObject.SetActive(false);
                shopPortal.StartCoroutine(shopPortal.OpenShopPortal());
                voteTracker.Clear();
                votesReceived = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !voteTracker.ContainsKey(PlayerId) && other.GetComponent<PhotonView>().IsMine)
        {
            popup.gameObject.SetActive(true);
            SkipButton.Enable();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && voteTracker.ContainsKey(PlayerId) && other.GetComponent<PhotonView>().IsMine)
        {
            popup.gameObject.SetActive(false);
            SkipButton.Disable();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popup.gameObject.SetActive(false);
            SkipButton.Disable();
        }
    }


}



