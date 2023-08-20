using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Casino : MonoBehaviour
{
    [TabGroup("Game"), SerializeField]
    private int costPerPlay = 100; // Currency cost for each play
    [TabGroup("Game"), SerializeField]
    private float noRewardChance = 0.2f; // Chance of getting no reward (20%)
    [TabGroup("Game"), SerializeField]
    private float breakChance = 0.1f; // Chance of getting no reward (10%)
    [TabGroup("Game"), SerializeField]
    private WeightedRandomList<int> goldRewards = new WeightedRandomList<int>();
    [TabGroup("Setup"), SerializeField]
    private InputAction Play;
    [TabGroup("Setup"), SerializeField]
    private bool canBreak = true;
    [TabGroup("Setup"), SerializeField]
    private GameObject PopUp;
    [TabGroup("Setup"), SerializeField]
    private TextMeshProUGUI playerGold;
    [TabGroup("Setup"), SerializeField]
    private Animator armAnimator;
    [TabGroup("Setup"), SerializeField]
    private Animator reelAnimator;
    [TabGroup("Decoration"), SerializeField]
    private TextMeshPro PayoutText;
    [TabGroup("Decoration"), SerializeField]
    GameObject[] brokenChanges;
    [TabGroup("Decoration"), SerializeField]
    private Material brokenMaterial;
    [TabGroup("Audio"), SerializeField]
    private WeightedRandomList<SFX> lowWinSounds = new WeightedRandomList<SFX>();
    [TabGroup("Audio"), SerializeField]
    private WeightedRandomList<SFX> meduimWinSounds = new WeightedRandomList<SFX>();
    [TabGroup("Audio"), SerializeField]
    private WeightedRandomList<SFX> highWinSounds = new WeightedRandomList<SFX>();
    [TabGroup("Audio"), SerializeField]
    private SFX jackpot;
    [TabGroup("Audio"), SerializeField]
    private SFX spinningSFX;
    [TabGroup("Audio"), SerializeField]
    private SFX lose;
    [TabGroup("Audio"), SerializeField]
    private SFX brokenSFX;


    private bool broken = false;
    private bool decided = false;
    private bool spinning = false;
    private bool inRange = false;
    private InventoryUi Player;


    private void Awake()
    {
        Play.performed += ctx => PlaySlotMachine();
        PayoutText.text = "0G";
    }
    private void Update()
    {
        if (spinning && !decided)
        {
            PayoutText.color = Color.white;
            PayoutText.text = Random.Range(0, 1200).ToString() + "G";
        }
    }
    private IEnumerator SpinCycle()
    {
        if (spinning)
            yield break;
        spinning = true;
        armAnimator.Play("Arm Drop");
        reelAnimator.Play("Reel Spin");
       
        spinningSFX.PlaySFX();
        yield return new WaitForSecondsRealtime(2.5f);
        decided = true;
        spinningSFX.StopSFX();
        if (Random.value < noRewardChance)
        {

            PayoutText.color = Color.red;
            PayoutText.text = "0G";
            lose.PlaySFX();
            updatePlayerGold();
            breakCheck(Random.Range(.01f, .05f));
        }
        else
        {
            
            int goldReward = goldRewards.GetRandom();
            Player.UpdateGold(goldReward);
            if (goldReward < 100)
            {
                SFX rewardSound = lowWinSounds.GetRandom();
                rewardSound.PlaySFX();
            }
            else if (goldReward < 660)
            {
                SFX rewardSound = meduimWinSounds.GetRandom();
                rewardSound.PlaySFX();
            }
            else if (goldReward < 999)
            {
                SFX rewardSound = highWinSounds.GetRandom();
                rewardSound.PlaySFX();
            }
            else if (goldReward == 1000)
                jackpot.PlaySFX();

            if (goldReward < 100)
                PayoutText.color = Color.red;
            else if (goldReward == 100)
                PayoutText.color = Color.white;
            else if (goldReward > 100 && goldReward < 1000)
                PayoutText.color = Color.green;
            else if (goldReward == 1000)
                PayoutText.color = Color.yellow;
            PayoutText.text = goldReward.ToString() + "G";
            updatePlayerGold();
            breakCheck(Random.Range(.05f, .15f));
        }
        yield return new WaitForSecondsRealtime(1f);
        spinning = false;
        decided = false;
        if (!inRange)
            Player = null;
    }
    public void PlaySlotMachine()
    {     
        if (Player.HasEnoughGold(costPerPlay) && !spinning)
        {
            Player.UpdateGold(costPerPlay * -1);
            updatePlayerGold();
            StartCoroutine(SpinCycle());
        }
        else if (!Player.HasEnoughGold(costPerPlay))
        {

            PayoutText.color = Color.red;
            PayoutText.text = "Not enough gold!";
        }
    }

    private void breakCheck(float incress)
    {
        if (canBreak)
        {
            if (Random.value < breakChance)
            {
                Play.Disable();
                Play.performed -= ctx => PlaySlotMachine();
                Player = null;
                broken = true;
                foreach (GameObject gameObject in brokenChanges)
                {
                    Renderer render = gameObject.GetComponent<Renderer>();
                    render.material = brokenMaterial;
                }
                brokenSFX.PlaySFX();
                PayoutText.color = Color.red;
                PayoutText.text = "Out of Order";
                PopUp.SetActive(false);
            }
            else
            {
                breakChance += incress;
            }
        }
    }
    private void updatePlayerGold()
    {
        playerGold.text = "Current Gold " + Player.CheckGold().ToString() + "G";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !broken)
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            Character character = player.character;
            Player = character.GetComponent<InventoryUi>();
            updatePlayerGold();
            PopUp.SetActive(true);
            inRange = true;
            Play.Enable();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !broken)
        {
            inRange = false;
            if (!spinning)
                Player = null;
            PopUp.SetActive(false);
            Play.Disable();
        }
    }
}
