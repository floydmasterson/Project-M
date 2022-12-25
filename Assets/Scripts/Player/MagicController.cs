using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class MagicController : MonoBehaviourPun, IAttack
{

    private float currentManaRechargeTimer;
    private float currentCastTimer;
    private PlayerManger manger;

    [TabGroup("Spell and Mana")]
    public Spell selectedSpell;
    [TabGroup("Spell and Mana")]
    [SerializeField] private bool isCasting = false;
    [TabGroup("Spell and Mana")]
    [SerializeField] float _maxMana = 100f;
    [TabGroup("Spell and Mana"), ProgressBar(0, "_maxMana", 0, 0, 1)]
    [SerializeField] float _currMana;

    [TabGroup("Setup")]
    [SerializeField] Transform castPoint;
    [TabGroup("Setup")]
    [SerializeField] float _timeToWaitForRecharge = 1.5f;
    [TabGroup("Setup")]
    [SerializeField] float _BasemanaRegenRate = 2f;
    [TabGroup("Setup")]
    [SerializeField] private float _castCooldown = .25f;

    [TabGroup("Audio"), Required, SerializeField]
    SFX spellcast;
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
    private void Awake()
    {

        manger = PlayerUi.Instance.target;
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
    }
    public void Attack()
    {
        if (selectedSpell != null)
        {
            bool hasEnoughMana = _currMana - selectedSpell.spellToCast.ManaCost >= 0f;
            if (!isCasting && hasEnoughMana)
            {

                isCasting = true;
                currentCastTimer = 0;
                _currMana -= selectedSpell.spellToCast.ManaCost;
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
        PhotonNetwork.Instantiate(selectedSpell.name, castPoint.position, castPoint.rotation);
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
                player.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (player.Defense / Character.Instance.Strength.Value))), manger); ;
            }
            if (Etarget != null)
            {
                Etarget.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2, (Etarget.Defense / Character.Instance.Strength.Value)))); ;
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
}
