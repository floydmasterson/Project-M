using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Spell : MonoBehaviour
{
    public SpellScriptableObject spellToCast;
    [SerializeField] StatusEffectSO[] statusEffects;
    private SphereCollider sphereCollider;
    [Space]
    [Tooltip("If unchecked all below is not needed")]
    public bool homing;
    [SerializeField] private LayerMask EnemyMask;
    public Rigidbody rb;
    [SerializeField] private Transform origin;
    [SerializeField] private float homingRange;
    [SerializeField] private float rotForce;
    [SerializeField] private float force;
    private Transform target;


    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = spellToCast.spellRadius;

        Destroy(this.gameObject, spellToCast.lifeTime);
    }

    private void FixedUpdate()
    {
        if (homing == false)
        {
            if (spellToCast.speed > 0) transform.Translate(Vector3.forward * spellToCast.speed * Time.fixedDeltaTime);
        }
        else
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
            Destroy(this.gameObject);
        }
        if (other.CompareTag("enemy"))
        {
            Enemys enemy = other.GetComponent<Enemys>();
            Debug.Log("takedamae");
            enemy.TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(2f, enemy.Defense / Character.Instance.Intelligence.Value))));
            if (!enemy.isDead)
            {
                enemy.Target = PlayerUi.Instance.target.transform;

            }
            if (statusEffects != null)
            {
                Debug.Log("foreach apply"); 
                foreach (StatusEffectSO effect in statusEffects)
                {
                    Debug.Log("spell apply");
                    effect.ApplyEffect(enemy);
                }
            }
            else
            {
                Debug.Log("spell status empy");
            }

            Debug.Log("destory");
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (origin == null)
            return;
        Gizmos.DrawSphere(origin.position, homingRange);
    }

}
