using UnityEngine;
using Photon.Pun;

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
        StartCoroutine(manger.MoveLock());
        Collider[] hitEnemins = Physics.OverlapSphere(manger.attackPoint.position, manger.attackRange, manger.enemyLayers);
        foreach (Collider enemy in hitEnemins)
        {
            enemy.GetComponent<Enemys>().TakeDamge(Mathf.RoundToInt(Character.Instance.Strength.Value / Mathf.Pow(2f,(enemy.GetComponent<Enemys>().Defense/Character.Instance.Strength.Value))));
            if (!enemy.GetComponent<Enemys>().isDead)
                enemy.GetComponent<Enemys>().Target = manger.transform;
        }
     

    }
}

