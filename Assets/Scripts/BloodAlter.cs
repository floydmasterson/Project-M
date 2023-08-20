using Kryz.CharacterStats;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class BloodAlter : MonoBehaviourPun
{
    private enum AlterType
    {
        Hidden_Room,
        Spawned
    }
    [SerializeField, EnumToggleButtons]
    private AlterType alterType;

    [SerializeField, HideIf("@alterType != AlterType.Hidden_Room"), TabGroup("Rewards")]
    private WeightedRandomList<GameObject> rewards = new WeightedRandomList<GameObject>();
    [SerializeField, HideIf("@alterType != AlterType.Hidden_Room"), TabGroup("Rewards")]
    private GameObject[] rewardSpawnPoint;

    [SerializeField, TabGroup("Stats")] private bool used = false;
    [SerializeField, TabGroup("Stats")] private int healthCost;
    [SerializeField, TabGroup("Stats")] private float StregthBonus;
    [SerializeField, TabGroup("Stats")] private float AgilityPercentBonus;
    [SerializeField, TabGroup("Stats")] private float IntelligenceBonus;

    [SerializeField, TabGroup("Decoration")] private GameObject fire;
    [SerializeField, TabGroup("Decoration")] private Light skullLamp;
    [SerializeField, TabGroup("Decoration")] private GameObject openTrap;
    [SerializeField, TabGroup("Decoration")] private GameObject closedTrap;
    [SerializeField, TabGroup("Decoration")] private GameObject LiveTree;
    [SerializeField, TabGroup("Decoration")] private GameObject DeadTree;
    [SerializeField, TabGroup("Decoration")] private Item Alter;
    [SerializeField, TabGroup("Decoration")] GameObject PopUp;
    [SerializeField, TabGroup("Decoration")] ParticleSystem RuneCircle;
    [SerializeField, TabGroup("Decoration")] ParticleSystem TreePuff;
    [SerializeField, TabGroup("Decoration")] SFX sacrafice;
    private int uses;

    private void Awake()
    {
        if (alterType == AlterType.Hidden_Room)
        {
            uses = Random.Range(1, 5);
        }
    }
    public void Sacrafice(object controller, PlayerManger player)
    {
        if (alterType == AlterType.Spawned)
        {
            if (controller is MeeleController)
            {
                player._sacrificedHealth += healthCost;
                player.character.Strength.AddModifier(new StatModifier(StregthBonus, StatModType.Flat, Alter));
            }
            else if (controller is ArrowController)
            {
                player._sacrificedHealth += healthCost;
                player.character.Agility.AddModifier(new StatModifier(AgilityPercentBonus, StatModType.PercentMult, Alter));
            }
            else if (controller is MagicController)
            {
                player._sacrificedHealth += healthCost;
                player.character.Intelligence.AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, Alter));
            }
            player.CheckMaxHealth();
            player.Alter = null;
            Disable();
        }
        else if (alterType == AlterType.Hidden_Room)
        {
            int damage = Random.Range(5, 30);
            player.TakeDamge(damage);
            uses--;
            spawnReward();
            if (uses == 0)
            {
                player.Alter = null;
                Disable();
            }
        }
    }

    private void spawnReward()
    {
        int rewardAmount = Random.Range(0, 2);
        GameObject reward;
        if (rewardAmount > 0)
        {
            if(rewardAmount == 1)
            {
                reward = rewards.GetRandom();
                PhotonNetwork.Instantiate(reward.name, rewardSpawnPoint[0].transform.position, gameObject.transform.rotation);
            }
            else if (rewardAmount == 2)
            {
                reward = rewards.GetRandom();
                PhotonNetwork.Instantiate(reward.name, rewardSpawnPoint[0].transform.position, gameObject.transform.rotation);
                reward = rewards.GetRandom();
                PhotonNetwork.Instantiate(reward.name, rewardSpawnPoint[1].transform.position, gameObject.transform.rotation);
            }    
        }
    }

    private void Disable()
    {
        used = true;
     
        fire.SetActive(true);
        skullLamp.enabled = true;
        openTrap.SetActive(false);
        closedTrap.SetActive(true);
        LiveTree.SetActive(false);
        TreePuff.Play();
        DeadTree.SetActive(true);
        sacrafice.PlaySFX();
        RuneCircle.Play();
        PopUp.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !used)
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            player.Alter = this;
            PopUp.SetActive(true);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !used)
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            player.Alter = null;
            PopUp.SetActive(false);

        }
    }

}
