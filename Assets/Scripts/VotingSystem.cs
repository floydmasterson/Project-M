using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VotingSystem : MonoBehaviourPunCallbacks
{
    public static VotingSystem Instance { get; private set; }
    private int requiredVotes = PhotonNetwork.CountOfPlayers;
    private int votesReceived = 0;
    private Dictionary<int, bool> voteTracker = new Dictionary<int, bool>();
    public event Action timeSkip;
    public event Action<int> voteCast;

    private Canvas popup;
    [SerializeField] private ShopPortal shopPortal;
    [SerializeField]
    private InputAction SkipButton;
    [SerializeField]
    private Collider col;
    [SerializeField]
    private GameObject Light;
    private int PlayerId;

    private void Awake()
    {
        Instance = this;
        PlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        popup = GetComponentInChildren<Canvas>(true);
        SkipButton.performed += ctx => CastVote(PlayerId);
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
            voteCast?.Invoke(votesReceived);

            if (votesReceived >= requiredVotes)
            {
                shopPortal.OpenShopPortal();
                timeSkip?.Invoke();
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
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popup.gameObject.SetActive(false);
            SkipButton.Disable();
        }
    }


}
