using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MagicController : MonoBehaviour, IAttack
{

    private float currentManaRechargeTimer;
    private float currentCastTimer;
    private PlayerManger manger;
    public Spell selectedSpell;
    [SerializeField] Transform castPoint;
    [SerializeField] float _maxMana = 100f;
    [SerializeField] float _currMana;
    [SerializeField] float _timeToWaitForRecharge = 1.5f;
    [SerializeField] float _BasemanaRegenRate = 2f;
    [SerializeField] private float _castCooldown = .25f;
    [SerializeField] private bool isCasting = false;

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
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock(.9f));
        yield return new WaitForSecondsRealtime(.6f);
        CastSpell();
    }
    private IEnumerator AttackMelee()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock(.9f));
        yield return new WaitForSecondsRealtime(.1f);
    }
    private void Awake()
    {

        manger = PlayerUi.Instance.target;
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
            if (player != null && PlayerUi.Instance.target)
            {
                player.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (player.Defense / Character.Instance.Strength.Value))), manger); ;
            }
            if (Etarget != null)
            {
                Etarget.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value)))); ;
            }
        }
    }
}
