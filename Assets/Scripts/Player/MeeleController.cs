using Photon.Pun;
using System;
using UnityEngine;

public class MeeleController : MonoBehaviourPun, IAttack
{
    private PlayerManger manger;
    [SerializeField] float _maxRage = 100f;
    [SerializeField] float _currRage;
  
    public float MaxRage
    {
        get { return _maxRage; }
        set
        {
            _maxRage = value;
            if (_currRage > _maxRage)
            {
                CurrentRage = MaxRage;
            }
        }
    }
    public float CurrentRage
    {
        get { return _currRage; }
        set
        {
            _currRage = value;
            if (_currRage > _maxRage)
            {
                _currRage = _maxRage;
            }
        }
    }
    private void Awake()
    {
        manger = PlayerUi.Instance.target;
        CurrentRage = 0;
    }
    private void OnEnable()
    {
        PlayerManger.OnDamaged += GainRage;
    }


    private void OnDisable()
    {
        PlayerManger.OnDamaged -= GainRage;
    }

   private void Update()
    {
        if(CurrentRage > 10 && Input.GetKeyDown(KeyCode.Q))
        {
            CurrentRage -= 10;
            manger.Heal(40);
        }
    }
    private void GainRage(int recivedDamaged)
    {
        CurrentRage += recivedDamaged / 4;
    }
    public void Attack()
    {
        if (manger == null)
            manger = PlayerUi.Instance.target;
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
                manger.Heal(Mathf.RoundToInt(damage / 3));

            }
            if (Etarget != null)
            {
                int damage = Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value)));
                Etarget.TakeDamge(damage);
                manger.Heal(Mathf.RoundToInt(damage / 3));
            }
        }
    }
}

