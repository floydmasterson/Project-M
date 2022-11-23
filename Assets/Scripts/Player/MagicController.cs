using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MagicController : MonoBehaviour, IAttack
{
    public static MagicController Instance;
    private PlayerManger manger;
    private PhotonView photonView;
    //  public static event AttackWorks
    [SerializeField] public Spell selectedSpell;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float _maxMana = 100f;
    [SerializeField] private float _currMana;
    [SerializeField] private float _timeToWaitForRecharge = 1.5f;
    [SerializeField] private float _BasemanaRegenRate = 2f;
    private float _additonalRegenRate;
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
        set { _currMana = value; }
    }
    private float currentManaRechargeTimer;
    [SerializeField] private float _castCooldown = .25f;
    private float currentCastTimer;

    [SerializeField] private bool isCasting = false;

    private IEnumerator AttackSpell()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock());
        yield return new WaitForSecondsRealtime(.6f);
        CastSpell();
    }
    private IEnumerator AttackMelee()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock());
        yield return new WaitForSecondsRealtime(.1f);
    }
    private void Awake()
    {
        Instance = this;
        photonView = PhotonView.Get(this);
        manger = gameObject.GetComponent<PlayerManger>();
        _currMana = _maxMana;
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
                manger.canAttack = true;
            }
            else
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
        PhotonNetwork.Instantiate(selectedSpell.name, castPoint.position, castPoint.rotation);
    }
    void MeeleAttack()
    {
        StartCoroutine(AttackMelee());
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        foreach (Collider enemy in hitEnemins)
        {
            enemy.GetComponent<Enemys>().TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (enemy.GetComponent<Enemys>().Defense / Character.Instance.Strength.Value)))); ;
            if (!enemy.GetComponent<Enemys>().isDead)
                enemy.GetComponent<Enemys>().Target = manger.transform;
        }
    }

}
