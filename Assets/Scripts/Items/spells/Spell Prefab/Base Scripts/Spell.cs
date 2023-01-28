using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Spell : MonoBehaviourPun
{
    [TabGroup("Spell"), Required]
    public SpellScriptableObject spellToCast;
    [TabGroup("Spell"), SerializeField]
    ParticleSystem ps;
    [TabGroup("Spell"), SerializeField]
    StatusEffectSO[] statusEffects;

    [TabGroup("Homing")]
    public bool homing;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    private LayerMask EnemyMask;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    Rigidbody rb;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    Transform origin;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    float homingRange;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    float rotForce;
    [TabGroup("Homing"), HideIf("@!homing"), SerializeField]
    float force;

    SphereCollider sphereCollider;
    Transform target;
    bool poof = false;
    bool hit = false;

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
        if (GameManger.Instance.GameTimeLeft <= 0 && PlayerUi.Instance.target.pvp == true)
            EnemyMask |= LayerMask.GetMask("enenmyMask") | LayerMask.GetMask("target");
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
            if (target == null)
            {
                if (spellToCast.speed > 0)
                    transform.Translate(Vector3.forward * spellToCast.speed * Time.fixedDeltaTime);
                Collider[] hitEnemins = Physics.OverlapSphere(origin.position, homingRange, EnemyMask);
                foreach (Collider enemy in hitEnemins)
                {
                    Enemys mob = enemy.gameObject.GetComponent<Enemys>();
                    PlayerManger player = enemy.gameObject.GetComponent<PlayerManger>();
                    if (mob != null)
                        target = mob.transform;
                    if (player != null && player != PlayerUi.Instance.target)
                        target = player.transform;
                    break;
                }
            }
            else if (target != null)
            {
                Vector3 direction = target.position - rb.position;
                direction.Normalize();
                Vector3 rotAmount = Vector3.Cross(transform.forward, direction);
                rb.angularVelocity = rotAmount * rotForce;
                rb.velocity = transform.forward * force;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int Obstruction = LayerMask.NameToLayer("Obstruction");
        if (hit == false)
        {
            if (other.gameObject.layer == Obstruction)
            {
                Poof();
            }
            if (photonView.IsMine)
            {
                if (other.CompareTag("enemy"))
                {
                    hit = true;
                    Enemys enemy = other.GetComponent<Enemys>();
                    enemy.TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(3f, enemy.Defense / Character.Instance.Intelligence.Value))));
                    if (statusEffects != null)
                    {
                        foreach (StatusEffectSO effect in statusEffects)
                        {
                            StatusEffectSO effectCopy = effect.GetCopy();
                            if (effectCopy != null)
                                effectCopy.ApplyEffect(enemy);
                        }
                    }
                    else
                    {
                        Debug.Log("spell status empy");
                    }
                    Poof();
                }
                if (other.CompareTag("Player") && PlayerUi.Instance.target.pvp == true)
                {
                    hit = true;
                    PlayerManger enemy = other.GetComponent<PlayerManger>();
                    if (enemy.IsLocal == false)
                    {
                        enemy.TakeDamge(spellToCast.dmgAmt + (Mathf.RoundToInt(Character.Instance.Intelligence.Value / Mathf.Pow(3, enemy.Defense / Character.Instance.Intelligence.Value))), PlayerUi.Instance.target);
                        if (statusEffects != null)
                        {
                            foreach (StatusEffectSO effect in statusEffects)
                            {
                                StatusEffectSO eCopy = effect.GetCopy();
                                if (eCopy != null)
                                    eCopy.ApplyEffect(enemy);
                            }
                        }
                        Poof();
                    }
                }
            }
        }
    }
    private void Poof()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        poof = true;
        if (ps != null)
            ps.Play();
        Destroy(gameObject, .5F);
    }
    private void OnDrawGizmosSelected()
    {
        if (origin == null)
            return;
        Gizmos.DrawSphere(origin.position, homingRange);
    }
}
