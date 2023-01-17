using Photon.Pun;
using UnityEngine;
using System.Collections;

public class enemyProjectile : MonoBehaviourPun
{
    [SerializeField] int speed;
    [SerializeField] float lifeTime;
    [SerializeField] ParticleSystem particle;
    Enemys origin;
    bool poof;
    bool hit = false;

    private IEnumerator LifeTime()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        Poof();
    }
    private void Awake()
    {
        StartCoroutine(LifeTime());
    }
    private void FixedUpdate()
    {
        if (poof == false)
            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        int Obstruction = LayerMask.NameToLayer("Obstruction");

        if (other.gameObject.layer == Obstruction)
        {
            Poof();
        }
        if (other.CompareTag("Player"))
        {
            PlayerManger target = other.GetComponent<PlayerManger>();
            Poof();
            if (!hit)
                target.TakeDamge(Mathf.RoundToInt(origin.Power / Mathf.Pow(2, (target.Defense / origin.Power))), origin);
            hit = true;
            return;
        }
    }
    public void setOrigin(Enemys enemy)
    {
        origin = enemy;
    }

    private void Poof()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        poof = true;
        if (GetComponent<ParticleSystem>() != null)
            GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, .5f);
    }
}
