using UnityEngine;
using Photon.Pun;

public class enemyProjectile : MonoBehaviourPun
{
    public int speed;
    Enemys origin;
    bool poof;
    [SerializeField] ParticleSystem ps;

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
            target.TakeDamge(Mathf.RoundToInt(origin.Power / Mathf.Pow(3, (target.Defense / origin.Power))), origin);
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
        if (ps != null)
            ps.Play();
       Destroy(gameObject, .5f);
    }
}
