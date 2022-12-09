using Photon.Pun;
using UnityEngine;

public class MeeleController : MonoBehaviourPun, IAttack
{
    private PlayerManger manger;

    private void Awake()
    {
        manger = PlayerUi.Instance.target;
    }
    public void Attack()
    {
        if(manger == null)
            manger = PlayerUi.Instance.target;
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock(.8f));
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        foreach (Collider enemy in hitEnemins)
        {
            Enemys Etarget = enemy.GetComponent<Enemys>();
            PlayerManger Ptarget = enemy.GetComponent<PlayerManger>();
            if (Ptarget != null)
            {
                Ptarget.TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Ptarget.Defense / Character.Instance.Strength.Value)))); ;
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
        foreach (Collider enemy in hitEnemins)
        {
            Enemys Etarget = enemy.GetComponent<Enemys>();
            PlayerManger Ptarget = enemy.GetComponent<PlayerManger>();
            if (Ptarget != null)
            {
                int damage = (Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Ptarget.Defense / Character.Instance.Strength.Value))));
                Ptarget.TakeDamge(damage);
                manger.Heal(damage / 3);
            }
            if (Etarget != null)
            {
                int damage = (Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (Etarget.Defense / Character.Instance.Strength.Value))));
                Etarget.TakeDamge(damage);
                manger.Heal(damage / 3);
               
            }
        }
    }
}

