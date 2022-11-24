using Photon.Pun;
using UnityEngine;

public class MeeleController : MonoBehaviourPun, IAttack
{
    private PlayerManger manger;

    private void Awake()
    {
        manger = gameObject.GetComponent<PlayerManger>();
    }
    public void Attack()
    {
        manger.photonView.RPC("UpdateAttack", RpcTarget.All);
        StartCoroutine(manger.IFrames(.6f));
        StartCoroutine(manger.MoveLock(.8f));
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        foreach (Collider enemy in hitEnemins)
        {
            enemy.GetComponent<Enemys>().TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (enemy.GetComponent<Enemys>().Defense / Character.Instance.Strength.Value))));
            if (!enemy.GetComponent<Enemys>().isDead)
                enemy.GetComponent<Enemys>().Target = manger.transform;
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
            Enemys target = enemy.GetComponent<Enemys>();
            int damage = (Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f, (target.Defense / Character.Instance.Strength.Value))));
           target.TakeDamge(damage);
            manger.Heal(damage / 3);
            Debug.Log(damage);
            Debug.Log(damage/3);
            if (!target.isDead)
                target.Target = manger.transform;
        }
    }
}

