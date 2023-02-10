using Kryz.CharacterStats;
using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class MeeleController : MonoBehaviourPun, IAttack
{

    private float currentRageFallOffTimer;
    private float currentRageDecayTimer;
    private PlayerManger manger;

    [TabGroup("Rage")]
    [SerializeField] int _maxRage = 1;
    [TabGroup("Rage")]
    [ProgressBar(0, "_maxRage", 1, 0, 0, Segmented = true)]
    [SerializeField] int _currRage;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForFalloff = 10f;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForDecay = 2f;
    [TabGroup("Setup")]
    [SerializeField] Item Rage;
    [TabGroup("Rage")]
#pragma warning disable IDE0052 // Remove unread private members
    [SerializeField] bool raging = false;
#pragma warning restore IDE0052 // Remove unread private members
    [TabGroup("Rage")]
    [SerializeField] private bool damaged;
    [TabGroup("Rage")]
    [SerializeField] private int currentDefenseMod;
    [TabGroup("Rage")]
    [SerializeField] public float lifeStealAmount = .25f;
    [TabGroup("Audio"), SerializeField]
    SFX attackSwish;
    private float timeBetweenHeal;
    private int RageHealAmount;
    private float healDelay;

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
    IEnumerator AttackSync()
    {
        yield return new WaitForSecondsRealtime(0.08f);
        photonView.RPC("swingSFX", RpcTarget.All);
        yield return new WaitForSecondsRealtime(0.15f);
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        for (int i = 0; i < hitEnemins.Length; i++)
        {
            bool didDmg = false;
            Transform target = hitEnemins[i].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                player.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (player.Defense / Character.Instance.Strength.Value))), manger); ;
                didDmg = true;
            }
            if (Etarget != null)
            {
                Etarget.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value)))); ;
                didDmg = true;
            }
            if (didDmg == true)
                GainRage(1);
        }
    }
    private void Start()
    {
        manger = GetComponent<PlayerManger>();
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
        if (Time.time >= timeBetweenHeal && CurrentRage > 0)
        {
            manger.Heal(RageHealAmount);
            timeBetweenHeal = Time.time + healDelay;
        }

        if (damaged == true)
        {

            currentRageFallOffTimer += Time.deltaTime;
            if (currentRageFallOffTimer >= _timeToWaitForFalloff)
            {
                damaged = false;
            }
        }
        else if (damaged == false && CurrentRage > 0)
        {
            currentRageDecayTimer += Time.deltaTime;
            if (currentRageDecayTimer >= _timeToWaitForDecay)
            {
                CurrentRage--;
                currentRageDecayTimer = 0;
                RagePassive(CurrentRage);

            }
        }
    }
    void RagePassive(int stackCount)
    {
        float StrengthMod = 0;
        float AgilityMod = 0;
        int DefenseMod = 0;
        currentRageDecayTimer = 0;
        switch (stackCount)
        {
            case 1:
                {
                    healDelay = 3f;
                    RageHealAmount = 1;
                    DefenseMod = 1;
                }
                break;
            case 2:
                {
                    StrengthMod = .05f;
                    AgilityMod = .08f;
                    DefenseMod = 2;
                    healDelay = 2.5f;
                    RageHealAmount = 1;
                }
                break;
            case 3:
                {
                    StrengthMod = .08f;
                    AgilityMod = .1f;
                    DefenseMod = 3;
                    healDelay = 2f;
                    RageHealAmount = 1;
                }
                break;
            case 4:
                {
                    StrengthMod = .1f;
                    AgilityMod = .2f;
                    DefenseMod = 5;
                    healDelay = 1.7f;
                    RageHealAmount = 1;
                }
                break;
            case 5:
                {
                    StrengthMod = .15f;
                    AgilityMod = .3f;
                    DefenseMod = 7;
                    healDelay = 1.5f;
                    RageHealAmount = 2;
                }
                break;
        }
        Character.Instance.Strength.RemoveAllModifiersFromSource(Rage);
        Character.Instance.Agility.RemoveAllModifiersFromSource(Rage);
        PlayerUi.Instance.target.DefenseMod -= currentDefenseMod;
        currentDefenseMod = 0;
        manger.CheckDefense();
        Character.Instance.statPanel.UpdateStatValues();
        if (stackCount > 0)
        {
            if (stackCount > 1)
            {

                Character.Instance.Strength.AddModifier(new StatModifier(StrengthMod, StatModType.PercentMult, Rage));
                Character.Instance.Agility.AddModifier(new StatModifier(AgilityMod, StatModType.PercentMult, Rage));
            }
            manger.DefenseMod += DefenseMod;
            currentDefenseMod = DefenseMod;
            Character.Instance.statPanel.UpdateStatValues();
            manger.CheckDefense();
        }
        raging = false;
    }
    private void GainRage(int recivedDamaged)
    {
        if (recivedDamaged >= 15)
        {
            CurrentRage += Mathf.RoundToInt(recivedDamaged / 15);
        }
        else if (recivedDamaged < 15)
            CurrentRage++;
        damaged = true;
        currentRageFallOffTimer = 0;
        RagePassive(CurrentRage);
    }
    public void Attack()
    {
        if (manger == null)
            manger = PlayerUi.Instance.target;
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(AttackSync());
    }
    public void AttackLifeSteal()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        if (hitEnemins.Length != 0)
        {
            Transform target = hitEnemins[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                int damage = Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2.6f, (player.Defense / Character.Instance.Strength.Value)));
                player.TakeDamge(damage, manger);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));

            }
            if (Etarget != null)
            {
                int damage = Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2.6f, (Etarget.Defense / Character.Instance.Strength.Value)));
                Etarget.TakeDamge(damage);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));
            }
        }
    }
    private void onDeath(PlayerManger player)
    {
        CurrentRage = 0;
    }
    [PunRPC]
    private void swingSFX()
    {
        attackSwish.PlaySFX();
    }
}