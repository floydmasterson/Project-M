using UnityEngine;

public class enemyProjectile : MonoBehaviour
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
        PlayerManger target = other.GetComponent<PlayerManger>();
        if (other.CompareTag("Player"))
        {
            Poof();
            target.TakeDamge(Mathf.RoundToInt(origin.Power / Mathf.Pow(2f, (target.Defense / origin.Power)))); ;
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
        Destroy(this.gameObject, .5f);
    }
}
