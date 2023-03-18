using Kryz.CharacterStats;
using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MeeleController : MonoBehaviourPun, IAttack
{

    private float currentRageFallOffTimer;
    private float currentRageDecayTimer;

    private EffectVisualController EffectVisualContol;

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
    [TabGroup("Setup")]
    [SerializeField] Item Aura;
    [TabGroup("Setup")]
    public Sprite rageAuraImage;
    [TabGroup("Rage")]
    [SerializeField] private bool damaged;
    [TabGroup("Rage")]
    [SerializeField] private int currentDefenseMod;
    [TabGroup("Rage")]
    [SerializeField] public float lifeStealAmount = 0f;
    [TabGroup("Rage"), SerializeField]
    private float auraRange;
    [TabGroup("Rage"), SerializeField]
    private float auraCooldown;
    [TabGroup("Rage"), SerializeField]
    private bool auraOnCooldown = false;
    [TabGroup("Audio"), SerializeField]
    SFX attackSwish;
    private float timeBetweenHeal;
    private int RageHealAmount;
    private float healDelay;
    private float currentAuraTimer;

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
    private IEnumerator AttackSync()
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
                player.TakeDamge(DamageCaculator.MeleeDamage(manger.character, DamageCaculator.Reciver.Player, null, player));
                didDmg = true;
            }
            if (Etarget != null)
            {
                Etarget.TakeDamge(DamageCaculator.MeleeDamage(manger.character, DamageCaculator.Reciver.Mob, Etarget, null)); ;
                didDmg = true;
            }
            if (didDmg == true)
                GainRage(1);
        }
    }
    private IEnumerator RageAuraDebuff(PlayerManger player, Enemys enemy, int stacks)
    {
        float slowAmount = 0;
        float damageReduction = 0;
        float duration = 0;
        switch (stacks)
        {
            case 0:
                break;
            case 1:
                slowAmount = .05f;
                damageReduction = .05f;
                duration = 1f;
                break;
            case 2:
                slowAmount = .10f;
                damageReduction = .08f;
                duration = 1.5f;
                break;
            case 3:
                slowAmount = .15f;
                damageReduction = .1f;
                duration = 2f;
                break;
            case 4:
                slowAmount = .2f;
                damageReduction = .15f;
                duration = 2.5f;
                break;
            case 5:
                slowAmount = .25f;
                damageReduction = .2f;
                duration = 3f;
                break;
            default:
                slowAmount = .25f;
                damageReduction = .2f;
                duration = 3.5f;
                break;
        }
        if (player != null)
        {
            EffectVisualController EVC = player.gameObject.GetComponent<EffectVisualController>();
            EVC.EnableEffect(EffectVisualController.Effects.Slowed);
            player.character.Strength.AddModifier(new StatModifier(damageReduction, StatModType.PercentMult, Aura));
            player.character.Intelligence.AddModifier(new StatModifier(damageReduction, StatModType.PercentMult, Aura));
            player.character.Agility.AddModifier(new StatModifier(slowAmount, StatModType.PercentMult, Aura));
            player.character.statPanel.UpdateStatValues();
            yield return new WaitForSecondsRealtime(duration);
            EVC.DisableEffect(EffectVisualController.Effects.Slowed);
            player.character.Strength.RemoveAllModifiersFromSource(Aura);
            player.character.Intelligence.RemoveAllModifiersFromSource(Aura);
            player.character.Agility.RemoveAllModifiersFromSource(Aura);
            player.character.statPanel.UpdateStatValues();
        }
        else if (enemy != null)
        {
            EffectVisualController EVC = enemy.gameObject.GetComponent<EffectVisualController>();
            EVC.EnableEffect(EffectVisualController.Effects.Slowed);
            NavMeshAgent enemyAgent = enemy.gameObject.GetComponent<NavMeshAgent>();
            float enemyStartSpeed = enemyAgent.speed;
            int enemyStartingPower = enemy.Power;
            enemyAgent.speed = enemyAgent.speed - (enemyAgent.speed * slowAmount);
            enemy.Power = Mathf.RoundToInt(enemy.Power - (enemy.Power * damageReduction));
            yield return new WaitForSecondsRealtime(duration);
            EVC.DisableEffect(EffectVisualController.Effects.Slowed);
            enemyAgent.speed = enemyStartSpeed;
            enemy.Power = enemyStartingPower;
        }

    }

    private void Start()
    {
        EffectVisualContol = GetComponent<EffectVisualController>();
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
        if (currentAuraTimer > 0)
        {
            auraOnCooldown = true;
            currentAuraTimer -= Time.deltaTime;
        }
        else if (currentAuraTimer < 0)
        {
            auraOnCooldown = false;
            currentAuraTimer = 0;
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
        manger.character.Strength.RemoveAllModifiersFromSource(Rage);
        manger.character.Agility.RemoveAllModifiersFromSource(Rage);
        PlayerUi.Instance.target.DefenseMod -= currentDefenseMod;
        currentDefenseMod = 0;
        manger.CheckDefense();
        manger.character.statPanel.UpdateStatValues();
        if (stackCount > 0)
        {
            if (stackCount > 1)
            {

                manger.character.Strength.AddModifier(new StatModifier(StrengthMod, StatModType.PercentMult, Rage));
                manger.character.Agility.AddModifier(new StatModifier(AgilityMod, StatModType.PercentMult, Rage));
            }
            manger.DefenseMod += DefenseMod;
            currentDefenseMod = DefenseMod;
            manger.character.statPanel.UpdateStatValues();
            manger.CheckDefense();
        }
    }
    public void RageAura(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine)
            if (CurrentRage >= 1 && auraOnCooldown == false)
            {
                EffectVisualContol.EnableEffect(EffectVisualController.Effects.Rage_Aura);
                Collider[] hitenemines = Physics.OverlapSphere(transform.position, auraRange, manger.enemyLayers);
                for (int i = 0; i < hitenemines.Length; i++)
                {
                    Transform target = hitenemines[i].transform;
                    PlayerManger player = target.GetComponent<PlayerManger>();
                    Enemys Etarget = target.GetComponent<Enemys>();

                    if (player != null && player != PlayerUi.Instance.target)
                    {
                        player.StartCoroutine(RageAuraDebuff(player, null, CurrentRage));
                    }
                    if (Etarget != null)
                    {
                        Etarget.StartCoroutine(RageAuraDebuff(null, Etarget, CurrentRage));
                    }

                }
                CurrentRage = 0;
                auraOnCooldown = true;
                currentAuraTimer = auraCooldown;
                PlayerUi.Instance.SecondaryCooldownGFX(auraCooldown);
            }
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
        bool didDmg = false;
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        if (hitEnemins.Length != 0)
        {
            Transform target = hitEnemins[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                int damage = DamageCaculator.MeleeDamage(manger.character, DamageCaculator.Reciver.Player, null, player);
                player.TakeDamge(damage);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));
                didDmg = true;

            }
            if (Etarget != null)
            {
                int damage = DamageCaculator.MeleeDamage(manger.character, (int)DamageCaculator.Reciver.Mob, Etarget, null);
                Etarget.TakeDamge(damage);
                manger.Heal(Mathf.RoundToInt(damage * lifeStealAmount));
                didDmg = true;
            }
        }
        if (didDmg == true)
            GainRage(1);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, auraRange);
    }

}