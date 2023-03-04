using Photon.Pun;
using TMPro;
using UnityEngine;

public class timeToText : MonoBehaviourPun
{
    [SerializeField] private TextMeshPro timeText;
    [SerializeField] private TextMeshPro voteText;
    float time;
    int players;
    int votes = 0;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
            players = PhotonNetwork.CurrentRoom.PlayerCount;
        updateVote(0);
    }

    private void Awake()
    {
        time = GameManger.Instance.gameTime + 60;

    }


    private void Update()
    {
        if (time > 0)
        {
            updateTimer(time - Time.deltaTime);
            time -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
        if (PhotonNetwork.InRoom && players != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            players = PhotonNetwork.CurrentRoom.PlayerCount;
            updateVote(votes);
        }


    }
    private void updateTimer(float currentTime)
    {
        currentTime += 1;

        float min = Mathf.FloorToInt(currentTime / 60);
        float sec = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format("{0:00} : {1:00}", min, sec);
    }
   public void updateVote(int recivedVotes)
    {
        voteText.text = recivedVotes.ToString() + "/" + players + " Votes";
        votes = recivedVotes;
    }

}
