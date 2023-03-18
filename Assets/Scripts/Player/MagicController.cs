using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class MagicController : MonoBehaviourPun, IAttack
{

    private float currentManaRechargeTimer;
    private float currentCastTimer;
    private float currentPulseTimer;
    private PlayerManger manger;
    private bool pulseOnCooldown;

    [TabGroup("Spell and Mana")]
    public Projectile selectedSpell;
    [TabGroup("Spell and Mana")]
    [SerializeField] private bool isCasting = false;
    [TabGroup("Spell and Mana")]
    [SerializeField] float _maxMana = 100f;
    [TabGroup("Spell and Mana"), ProgressBar(0, "_maxMana", 0, 0, 1)]
    [SerializeField] float _currMana;
    [TabGroup("Spell and Mana")]
    [SerializeField] float _manaPulseCost = 20f;




    [TabGroup("Setup")]
    [SerializeField] Transform castPoint;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForRecharge = 1.5f;
    [TabGroup("Setup")]
    [SerializeField] float _BasemanaRegenRate = 2f;
    [TabGroup("Setup")]
    [SerializeField] private float _castCooldown = .25f;
    [TabGroup("Setup")]
    [SerializeField] float _pushForce = 10.0f;
    [TabGroup("Setup")]
    [SerializeField] float pulseCooldown = 10f;
    [TabGroup("Setup")]
    [SerializeField] float pulseRange = 10f;
    [TabGroup("Setup")]
    public Sprite manaPulseImage;

    EffectVisualController EffectVisualContol;

    [TabGroup("Audio"), Required, SerializeField]
    SFX spellcast;
    [TabGroup("Audio"), Required, SerializeField]
    SFX manaPulse;
    public float ManaRegenRate
    {
        get { return _BasemanaRegenRate; }
        set { _BasemanaRegenRate = value; }
    }
    public float MaxMana
    {
        get { return _maxMana; }
        set
        {
            _maxMana = value;
            if (_currMana > _maxMana)
            {
                CurrMana = MaxMana;
            }
        }
    }
    public float CurrMana
    {
        get { return _currMana; }
        set
        {
            _currMana = value;
            if (_currMana > _maxMana)
            {
                _currMana = _maxMana;
            }
        }
    }
    public float PushForce
    {
        get { return _pushForce; }

        set { _pushForce = value; }
    }
    public float ManaPulseCost
    {
        get
        {
            return _manaPulseCost;
        }
        set
        {
            _manaPulseCost = value;
        }
    }

    private IEnumerator AttackSpell()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        yield return new WaitForSecondsRealtime(.6f);
        CastSpell();
    }
    private IEnumerator AttackMelee()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        yield return new WaitForSecondsRealtime(.1f);
    }
    private IEnumerator ManaPulsePush(Transform target, Vector3 direction, float speed)
    {
        float startime = Time.time;
        Vector3 start_pos = target.position; //Starting position.
        Vector3 end_pos = target.position + direction; //Ending position.

        while (start_pos != end_pos && ((Time.time - startime) * speed) < 1f)
        {
            float move = Mathf.Lerp(0, 1, (Time.time - startime) * speed);

            target.position += direction * move;

            yield return null;
        }
    }

    private void Awake()
    {
        EffectVisualContol = GetComponent<EffectVisualController>();
        manger = GetComponent<PlayerManger>();
        _currMana = _maxMana;
    }
    private void OnEnable()
    {
        PlayerManger.OnDeath += onDeath;
    }
    private void OnDisable()
    {
        PlayerManger.OnDeath -= onDeath;
    }
    private void Update()
    {
        if (isCasting)
        {
            currentCastTimer = Time.deltaTime;
            if (currentCastTimer > _castCooldown) isCasting = false;
        }
        if (_currMana < _maxMana && !isCasting)
        {
            currentManaRechargeTimer += Time.deltaTime;
            if (currentManaRechargeTimer >= _timeToWaitForRecharge)
            {
                _currMana += _BasemanaRegenRate * Time.deltaTime;
                if (_currMana >= _maxMana)
                    _currMana = _maxMana;
            }
        }
        if (currentPulseTimer > 0)
        {
            pulseOnCooldown = true;
            currentPulseTimer -= Time.deltaTime;
        }
        else if (currentPulseTimer < 0)
        {
            pulseOnCooldown = false;
            currentPulseTimer = 0;
        }
    }
    public void Attack()
    {
        if (selectedSpell != null)
        {
            bool hasEnoughMana = _currMana - selectedSpell.ProjectileToUse.ManaCost >= 0f;
            if (!isCasting && hasEnoughMana)
            {

                isCasting = true;
                currentCastTimer = 0;
                _currMana -= selectedSpell.ProjectileToUse.ManaCost;
                StartCoroutine(AttackSpell());
                currentManaRechargeTimer = 0;
                isCasting = false;

            }
            else if (!isCasting)
            {
                MeeleAttack();
            }
        }
        else
        {
            MeeleAttack();
        }
    }
    void CastSpell()
    {
        photonView.RPC("spellCastSFX", RpcTarget.All);
        GameObject spell = PhotonNetwork.Instantiate(selectedSpell.name, castPoint.position, castPoint.rotation);
        spell.GetComponent<Projectile>().Setup(manger.character, manger, null);
    }
    void MeeleAttack()
    {
        StartCoroutine(AttackMelee());
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        if (hitEnemins.Length != 0)
        {
            Transform target = hitEnemins[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Enemys Etarget = target.GetComponent<Enemys>();
            if (player != null && player != PlayerUi.Instance.target)
            {
                player.TakeDamge(DamageCaculator.MeleeDamage(manger.character, DamageCaculator.Reciver.Player, null, player));
            }
            if (Etarget != null)
            {
                player.TakeDamge(DamageCaculator.MeleeDamage(manger.character, DamageCaculator.Reciver.Mob, Etarget, null));
            }
        }
    }
    public void ManaPulse(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine)
        {
            bool hasEnoughMana = _currMana - ManaPulseCost >= 0f;
            if (hasEnoughMana && !isCasting && !pulseOnCooldown)
            {

                manger.StartCoroutine(manger.IFrames(.8f));
                EffectVisualContol.EnableEffect(EffectVisualController.Effects.Mana_Pulse);
                isCasting = true;
                currentPulseTimer = pulseCooldown;
                currentCastTimer = 0;
                _currMana -= ManaPulseCost;
                photonView.RPC("ManaPulseSFX", RpcTarget.All);
                Collider[] hitenemines = Physics.OverlapSphere(transform.position, pulseRange, manger.enemyLayers);
                for (int i = 0; i < hitenemines.Length; i++)
                {
                    Transform target = hitenemines[i].transform;
                    PlayerManger player = target.GetComponent<PlayerManger>();
                    Enemys Etarget = target.GetComponent<Enemys>();
                    if (player != null && player != PlayerUi.Instance.target)
                    {
                        player.TakeDamge(DamageCaculator.MagicDamage(manger.character, DamageCaculator.Reciver.Player, Etarget, null));
                    }
                    if (Etarget != null)
                    {
                        Etarget.TakeDamge(DamageCaculator.MagicDamage(manger.character, (int)DamageCaculator.Reciver.Mob, Etarget, null)); ;
                    }
                    Vector3 pushDirection = (target.transform.position - transform.position).normalized;
                    StartCoroutine(ManaPulsePush(target, pushDirection, PushForce));
                }

                currentManaRechargeTimer = 0;
                isCasting = false;
                PlayerUi.Instance.SecondaryCooldownGFX(pulseCooldown);
            }
        }
    }
    private void onDeath(PlayerManger player)
    {
        CurrMana = MaxMana;
    }
    [PunRPC]
    private void spellCastSFX()
    {
        spellcast.PlaySFX();
    }
    [PunRPC]
    private void ManaPulseSFX()
    {
        manaPulse.PlaySFX();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pulseRange);
    }
}

