using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Projectile : MonoBehaviourPun
{
    private enum DamageType
    {
        Magic,
        Ranged,
    }
    [EnumToggleButtons, SerializeField]
    private DamageType damageType;

    [TabGroup("Setup"), Required]
    public ProjectileSO ProjectileToUse;
    [TabGroup("Setup"), SerializeField]
    ParticleSystem ps;
    [TabGroup("Setup"), SerializeField]
    public List<StatusEffectSO> statusEffects = new List<StatusEffectSO>();

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

    [TabGroup("Explosive"), SerializeField]
    private bool willExpload;
    [TabGroup("Explosive"), SerializeField]
    private int exploasionDmg;
    [TabGroup("Explosive"), SerializeField]
    private float exploasionRange;

    SphereCollider sphereCollider;
    Transform target;
    bool poof = false;
    bool hit = false;
    private Character character;
    private PlayerManger manger;

    private IEnumerator LifeTime()
    {
        yield return new WaitForSecondsRealtime(ProjectileToUse.lifeTime);
        Poof();

    }
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = ProjectileToUse.spellRadius;
        if (GameManger.Instance.GameTimeLeft <= 0 && PlayerUi.Instance.target.pvp == true)
            EnemyMask |= LayerMask.GetMask("enenmyMask") | LayerMask.GetMask("target");
        StartCoroutine(LifeTime());
    }
    private void FixedUpdate()
    {
        if (homing == false && poof == false)
        {
            if (ProjectileToUse.speed > 0) transform.Translate(Vector3.forward * ProjectileToUse.speed * Time.fixedDeltaTime);
        }
        else if (homing == true && poof == false)
        {
            if (target == null)
            {
                if (ProjectileToUse.speed > 0)
                    transform.Translate(Vector3.forward * ProjectileToUse.speed * Time.fixedDeltaTime);
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
            if (photonView.IsMine)
            {
                if (other.CompareTag("enemy"))
                {
                    hit = true;
                    Enemys enemy = other.GetComponent<Enemys>();
                    if (damageType == DamageType.Magic)
                        enemy.TakeDamge(DamageCaculator.MagicDamage(character, (int)DamageCaculator.Reciver.Mob, enemy, null, ProjectileToUse.dmgAmt));
                    if (damageType == DamageType.Ranged)
                        enemy.TakeDamge(DamageCaculator.RangedDamage(character, (int)DamageCaculator.Reciver.Mob, enemy, null, ProjectileToUse.dmgAmt));
                    if (statusEffects != null)
                    {
                        foreach (StatusEffectSO effect in statusEffects)
                        {
                            StatusEffectSO effectCopy = effect.GetCopy();
                            if (effectCopy != null)
                                effectCopy.ApplyEffect(enemy);
                        }
                    }
                    Poof();
                }
                if (other.CompareTag("Player") && PlayerUi.Instance.target.pvp == true)
                {
                    hit = true;
                    PlayerManger enemy = other.GetComponent<PlayerManger>();
                    if (enemy.IsLocal == false)
                    {
                        if (damageType == DamageType.Magic)
                            enemy.TakeDamge(DamageCaculator.MagicDamage(character, DamageCaculator.Reciver.Player, null, enemy, ProjectileToUse.dmgAmt));
                        if (damageType == DamageType.Ranged)
                            enemy.TakeDamge(DamageCaculator.RangedDamage(character, DamageCaculator.Reciver.Player, null, enemy, ProjectileToUse.dmgAmt));
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

                if (other.gameObject.layer == Obstruction)
                {
                    Poof();
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
        if (willExpload)
        {
            Collider[] hitenemines = Physics.OverlapSphere(transform.position, exploasionRange, manger.enemyLayers);
            for (int i = 0; i < hitenemines.Length; i++)
            {
                Transform target = hitenemines[i].transform;
                PlayerManger player = target.GetComponent<PlayerManger>();
                Enemys Etarget = target.GetComponent<Enemys>();

                if (player != null && player != PlayerUi.Instance.target)
                {
                    player.TakeDamge(exploasionDmg);
                    foreach (StatusEffectSO effect in statusEffects)
                    {
                        StatusEffectSO eCopy = effect.GetCopy();
                        if (eCopy != null)
                            eCopy.ApplyEffect(player);
                    }
                }
                if (Etarget != null)
                {
                    Etarget.TakeDamge(exploasionDmg);
                    foreach (StatusEffectSO effect in statusEffects)
                    {
                        StatusEffectSO eCopy = effect.GetCopy();
                        if (eCopy != null)
                            eCopy.ApplyEffect(Etarget);
                    }
                }
            }

        }

        Destroy(gameObject, .5F);
    }
    private void OnDrawGizmosSelected()
    {
        if (homing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(origin.position, homingRange);
        }
        if (willExpload)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gameObject.transform.position, exploasionRange);
        }

    }
    public void Setup(Character Sentcharacter, PlayerManger SentPlayer, StatusEffectSO effectsToAdd)
    {
        character = Sentcharacter;
        manger = SentPlayer;
        if (effectsToAdd != null)
        {
            statusEffects.Add(effectsToAdd);
        }
    }
}
