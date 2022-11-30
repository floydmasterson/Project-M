using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Spell : MonoBehaviour
{
    public SpellScriptableObject spellToCast;
    [SerializeField] ParticleSystem ps;
    [SerializeField] StatusEffectSO[] statusEffects;
    [Space]
    [Tooltip("If unchecked all below is not needed")]
    public bool homing;
    [SerializeField] private LayerMask EnemyMask;
    public Rigidbody rb;
    [SerializeField]  Transform origin;
    [SerializeField]  float homingRange;
    [SerializeField] float rotForce;
    [SerializeField] float force;
    SphereCollider sphereCollider;
  Transform target;
    bool poof = false;

    private IEnumerator LifeTime()
    {
        yield return new WaitForSecondsRealtime(spellToCast.lifeTime);
        Poof();
    }
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = spellToCast.spellRadius;

        StartCoroutine(LifeTime());
    }
    private void FixedUpdate()
    {
        if (homing == false && poof == false)
        {
            if (spellToCast.speed > 0) transform.Translate(Vector3.forward * spellToCast.speed * Time.fixedDeltaTime);
        }
        else if (homing == true && poof == false)
        {
            Collider[] hitEnemins = Physics.OverlapSphere(origin.position, homingRange, EnemyMask);
            foreach (Collider enemy in hitEnemins)
            {
                target = enemy.transform;
                break;
            }
            if (target != null)
            {
                Vector3 direction = target.position - rb.position;
                direction.Normalize();
                Vector3 rotAmount = Vector3.Cross(transform.forward, direction);
                rb.angularVelocity = rotAmount * rotForce;
                rb.velocity = transform.forward * force;
            }
            else
            {
                if (spellToCast.speed > 0) transform.Translate(Vector3.forward * spellToCast.speed * Time.fixedDeltaTime);
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {

        int Obstruction = LayerMask.NameToLayer("Obstruction");

        if (other.gameObject.layer == Obstruction)
        {
            Poof();
        }
        if (other.CompareTag("enemy"))
        {
            Enemys enemy = other.GetComponent<Enemys>();
            enemy.TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(2f, enemy.Defense / Character.Instance.Intelligence.Value))));
            if (!enemy.isDead)
            {
                enemy.Target = PlayerUi.Instance.target.transform;
            }
            if (statusEffects != null)
            {
                foreach (StatusEffectSO effect in statusEffects)
                {
                    StatusEffectSO eCopy = effect.GetCopy();
                    if (eCopy != null)
                        eCopy.ApplyEffectE(enemy);
                }
            }
            else
            {
                Debug.Log("spell status empy");
            }
            Poof();
        }
        if (other.CompareTag("Player"))
        {
            PlayerManger enemy = other.GetComponent<PlayerManger>();
            enemy.TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(2f, enemy.Defense / Character.Instance.Intelligence.Value))));
         
            if (statusEffects != null)
            {
                foreach (StatusEffectSO effect in statusEffects)
                {
                    StatusEffectSO eCopy = effect.GetCopy();
                    if (eCopy != null)
                        eCopy.ApplyEffectP(enemy);
                }
            }
            else
            {
                Debug.Log("spell status empy");
            }
            Poof();
        }
    }
    private void Poof()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        poof = true;
        if (ps != null)
            ps.Play();
        Destroy(this.gameObject, .5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (origin == null)
            return;
        Gizmos.DrawSphere(origin.position, homingRange);
    }

}
