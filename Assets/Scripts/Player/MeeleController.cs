using Kryz.CharacterStats;
using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class MeeleController : MonoBehaviourPun, IAttack
{
 
    private float currentRageFallOffTimer;
    private float currentRageDecayTimer;
    private PlayerManger manger;

    [TabGroup("Rage")]
    [SerializeField] int _maxRage = 1;
    [TabGroup("Rage")]
    [ProgressBar(0,"_maxRage", 1, 0, 0, Segmented = true)]
    [SerializeField] int _currRage;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForFalloff = 10f;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForDecay = 3f;
    [TabGroup("Setup")]
    [SerializeField] Item Rage;
    [TabGroup("Rage")]
    [SerializeField] bool raging = false;
    [TabGroup("Rage")]
    [SerializeField] private bool damaged;
    [TabGroup("Rage")]
    [SerializeField] public float lifeStealAmount = .25f;
    public int MaxRage
    {
        get { return _maxRage; }
        set
        {
            _maxRage = (int)value;
            if (_currRage > _maxRage)
            {
                CurrentRage = MaxRage;
            }
        }
    }
    public int CurrentRage
    {
        get { return _currRage; }
        set
        {
            _currRage = (int)value;
            if (_currRage > _maxRage)
            {
                _currRage = _maxRage;
            }
        }
    }
    IEnumerator RageMode(float StrengthMod, float AgilityMod, int DefenseMod)
    {
        if (raging)
            yield break;
        currentRageDecayTimer = 0;
        damaged = false;
        raging = true;
        int startingRage = CurrentRage;
        Color rageColor = new Color32(170, 38, 64, 255);
        Character.Instance.Strength.AddModifier(new StatModifier(StrengthMod, StatModType.PercentMult, Rage));
        Character.Instance.Agility.AddModifier(new StatModifier(AgilityMod, StatModType.PercentMult, Rage));
        manger.DefenseMod += DefenseMod;
        manger.CheckDefense();
        Character.Instance.statPanel.UpdateStatValues();
        PlayerUi.Instance.playerRageSlider.GetComponentInChildren<Image>().color = rageColor;
        for (int i = 0; i < startingRage; i++)
        {
            CurrentRage--;
            if (CurrentRage > 0)
                yield return new WaitForSecondsRealtime(2);
        }
        PlayerUi.Instance.playerRageSlider.GetComponentInChildren<Image>().color = Color.white;
        manger.LifeSteal = false;
        Character.Instance.Strength.RemoveAllModifiersFromSource(Rage);
        Character.Instance.Agility.RemoveAllModifiersFromSource(Rage);
        PlayerUi.Instance.target.DefenseMod -= DefenseMod;
        Character.Instance.statPanel.UpdateStatValues();
        raging = false;
    }
    private void Awake()
    {
        manger = PlayerUi.Instance.target;
        CurrentRage = 0;
    }
    private void OnEnable()
    {
        PlayerManger.OnDamaged += GainRage;
        PlayerManger.OnDeath += onDeath;
    }

    private void OnDisable()
    {
        PlayerManger.OnDamaged -= GainRage;
        PlayerManger.OnDeath += onDeath;
    }

    private void Update()
    {

        if (CurrentRage > 0 && Input.GetKeyDown(KeyCode.Q) && raging == false && PlayerUi.Instance.target.isAlive)
        {
            if (CurrentRage == 1)
            {
                manger.Heal(10);
                CurrentRage = 0;
            }
            if (CurrentRage == 2)
            {
                manger.Heal(20);
                StartCoroutine(RageMode(0.05f, 0.05f, 3));
            }
            if (CurrentRage == 3)
            {
                manger.Heal(30);
                StartCoroutine(RageMode(0.08f, 0.08f, 5));
            }
            if (CurrentRage == 4)
            {
                manger.Heal(40);
                manger.LifeSteal = true;
                StartCoroutine(RageMode(0.1f, 0.1f, 8));
            }
            if (CurrentRage >= 5)
            {
                if (CurrentRage == 5)
                    manger.Heal(50);
                if (CurrentRage > 5)
                    manger.Heal(50 + (10 * (CurrentRage - 5)));
                manger.LifeSteal = true;
                StartCoroutine(RageMode(0.15f, 0.2f, 10));

            }
        }
        if (damaged == true && raging == false)
        {
            currentRageFallOffTimer += Time.deltaTime;
            if (currentRageFallOffTimer >= _timeToWaitForFalloff)
            {
                damaged = false;
            }
        }
        else if (damaged == false && CurrentRage > 0 && raging == false)
        {
            currentRageDecayTimer += Time.deltaTime;
            if (currentRageDecayTimer >= _timeToWaitForDecay)
            {
                CurrentRage--;
                currentRageDecayTimer = 0;

            }
        }
    }
    private void GainRage(int recivedDamaged)
    {
        //CurrentRage += recivedDamaged / 4;
        if (raging == false)
        {
            CurrentRage++;
            damaged = true;
            currentRageFallOffTimer = 0;
        }

    }
    public void Attack()
    {
        if (manger == null)
            manger = PlayerUi.Instance.target;
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        //StartCoroutine(manger.MoveLock(.8f));
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        if (hitEnemins.Length != 0)
        {
            Transform target = hitEnemins[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                player.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (player.Defense / Character.Instance.Strength.Value))), manger); ;
            }
            if (Etarget != null)
            {
                Etarget.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value)))); ;
            }
        }
    }
    public void AttackLifeSteal()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock(.8f));
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        if (hitEnemins.Length != 0)
        {
            Transform target = hitEnemins[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                int damage = Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (player.Defense / Character.Instance.Strength.Value)));
                player.TakeDamge(damage, manger);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));

            }
            if (Etarget != null)
            {
                int damage = Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value)));
                Etarget.TakeDamge(damage);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));
            }
        }
    }

    private void onDeath(PlayerManger player)
    {
        CurrentRage = 0;
    }
}

