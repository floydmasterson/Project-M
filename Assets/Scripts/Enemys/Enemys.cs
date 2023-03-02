using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using Time = UnityEngine.Time;

public class Enemys : MonoBehaviourPun
{
    #region Vars
    public enum Tier
    {
        T1,
        T2,
        T3,
    }


    [BoxGroup]
    [Tooltip("0= target dummy 1 = Chase & Melee 2 = Avoid & Ranged 3 = Chase & Boom !4 = hybrid! not set")]
    [Range(0, 4)] public int typeSetting = 0;

    [EnumToggleButtons]
    public Tier EnemyTier;

    //Health
    [TabGroup("Health")]
    [Range(50, 100)] public float Vitality;
    [TabGroup("Health")]
    public float Defense;
    [TabGroup("Health")]
    [ProgressBar(0, "maxHealth", 0, 1, 0)]
    [SerializeField] float currentHealth;
    float maxHealth;
    [TabGroup("Health")]
    public bool isDead = false;
    [TabGroup("Health"), SerializeField]
    GameObject FloatingText;
    [TabGroup("Health"), SerializeField]
    Transform FloatingTextspawn;

    //Fov Detection/Movement
    [TabGroup("Detection")]
    public Transform Target;
    [TabGroup("Movement")]
    [SerializeField][Range(1, 100)] float LSpeed = 1f;
    [TabGroup("Detection")]
    [Range(0, 360)] public float angle;
    [TabGroup("Detection")]
    public float radius;
    [TabGroup("Movement")]
    [ShowIf("@typeSetting == 2")]
    [SerializeField] float runDistance = 0f;
    [TabGroup("Movement")]
    [ShowIf("@typeSetting == 2")]
    public float runRadius = 0f;
    [TabGroup("Movement")]
    [ShowIf("@typeSetting == 2")]
    [SerializeField] int runTime;
    [TabGroup("Detection")]
    public bool canSeePlayer;
    bool isLoseTargetExecuting = false;
    Coroutine LookCoroutine;

    //attacking
    [TabGroup("Attack")]
    [Range(25, 150)] public int Power = 25;
    [TabGroup("Attack")]
    [Required]
    public Transform attackPoint;
    [TabGroup("Attack")]
    [SerializeField][Range(0, 20)] float attackRange;
    [TabGroup("Attack")]
    [SerializeField][Range(1, 5)] float attackCooldown = 2f;
    [TabGroup("Attack")]
    [ShowIf("@typeSetting == 2")]
    [Required]
    [SerializeField] GameObject rangedProjectile;
    [TabGroup("Attack")]
    [ShowIf("@typeSetting == 3")]
    [SerializeField] float timeToBlow;
    [TabGroup("Attack")]
    [ShowIf("@typeSetting == 3")]
    [Required]
    [SerializeField] GameObject explosionGFX;
    bool tryingToBoom;
    bool isTAttackExecuting = false;
    bool isTRangedAttackExecuting = false;
    //masks
    [TabGroup("Detection")]
    [Required]
    [SerializeField] LayerMask targetMask;
    [TabGroup("Detection")]
    [Required]
    [SerializeField] LayerMask obstructionMask;
    //Comps
    private Animator animator;
    private NavMeshAgent agent;
    private Collider col;
    [TabGroup("Loot Drop")]
    [TableList(AlwaysExpanded = true), HideLabel]
    public WeightedRandomList<LootContainerControl> possibleBags;

    //audio 
    [TabGroup("Audio"), SerializeField]
    public WeightedRandomList<SFX> AttackSounds;
    [TabGroup("Audio"), SerializeField]
    public WeightedRandomList<SFX> HurtSounds;
    [TabGroup("Audio"), Required, SerializeField]
    SFX hurtSquish;
    [TabGroup("Audio"), Required, SerializeField]
    SFX Walk;
    [TabGroup("Audio"), Required, SerializeField]
    SFX DieSFX;
    [TabGroup("Audio"), Required, HideIf("@typeSetting != 3"), SerializeField]
    SFX hiss;
    [TabGroup("Audio"), Required, HideIf("@typeSetting != 3"), SerializeField]
    SFX boom;
    private bool firstAttack = true;
    private bool showDmgNumber;
    private bool showIsDirty = false;
    #endregion
    #region Base IEnumerators 
    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(4.1f);
        //Particl
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            Destroy(t.gameObject);
        }

    }
    IEnumerator Dropbag()
    {
     
        yield return new WaitForSecondsRealtime(3.8f);
        float chance;
        chance = Random.Range(1f, 10f);
        if (chance > 6.5f)
        {
            PhotonNetwork.Instantiate(possibleBags.GetRandom().name, transform.position, Quaternion.identity);
        }
      
    }

    private IEnumerator FOVRoutine()
    {
        if (typeSetting != 0)
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true)
            {
                yield return wait;

                FieldOfViewCheck();
            }
        }
    }

    private IEnumerator LookAt()
    {
        if (Target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(Target.position - transform.position);

            float time = 0;

            Quaternion initialRotation = transform.rotation;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);

                time += UnityEngine.Time.deltaTime * LSpeed;

                yield return null;
            }
        }
    }
    private IEnumerator LoseTarget()
    {
        if (isLoseTargetExecuting)
            yield break;
        isLoseTargetExecuting = true;
        yield return new WaitForSecondsRealtime(5f);
        Target = null;
        firstAttack = true;
        agent.SetDestination(agent.transform.position);
        UnityEngine.Debug.Log(this + " is lost");
        animator.SetTrigger("lost");
        UpdateMoving(false);
        isLoseTargetExecuting = false;

    }
    private IEnumerator RangeExpand()
    {
        radius = 100;
        angle = 360;
        yield return new WaitForSecondsRealtime(1);
        angle = 222;
        if (typeSetting == 1 || typeSetting == 3)
            radius = 20;
        if (typeSetting == 2)
            radius = 45;
    }
    #endregion
    #region Type 1 IEnumerators 
    private IEnumerator TAttack()
    {
        if (isTAttackExecuting)
            yield break;
        isTAttackExecuting = true;
        UpdateMoving(false);
#pragma warning disable CS0618 // Type or member is obsolete
        agent.Stop();
#pragma warning restore CS0618 // Type or member is obsolete
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, targetMask);
        if (hitPlayers.Length != 0)
        {
            Transform target = hitPlayers[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            if (player.isAlive == true)
            {
                photonView.RPC("UpdateAttack", RpcTarget.All);
                yield return new WaitForSecondsRealtime(.5f);
                player.TakeDamge(Mathf.RoundToInt(Power / Mathf.Pow(2.2f, (player.Defense / Power)))); ;
                if (player.CurrentHealth <= 0)
                {
                    Target = null;
                    StartCoroutine(LoseTarget());
                }
#pragma warning disable CS0618 // Type or member is obsolete
                agent.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
                yield return new WaitForSecondsRealtime(attackCooldown);
            }

        }

        isTAttackExecuting = false;
    }
    #endregion
    #region Type 2 IEnumerators
    private IEnumerator TRangedAttack()
    {
        if (isTRangedAttackExecuting)
            yield break;
        if (Target != null && runCheck() == false && canSeePlayer == true)
        {
            photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, true);
            if (firstAttack == true)
            {
                firstAttack = false;
                yield return new WaitForSecondsRealtime(.6f);
            }
            photonView.RPC("UpdateAttack", RpcTarget.All);
            GameObject projectile = PhotonNetwork.Instantiate(rangedProjectile.name, attackPoint.position, attackPoint.rotation);
            projectile.gameObject.GetComponent<enemyProjectile>().setOrigin(this);
            yield return new WaitForSecondsRealtime(attackCooldown);
            photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, false);
            StartCoroutine(TRangedAttack());
        }
        else if (Target != null && runCheck() == true)
        {
            bool running = false;
            photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, true);
            if (agent != null)
            {

                UpdateMoving(true);
                if (running)
                {
                    if (runCheck() == false)
                    {
                        UpdateMoving(false);
                        agent.SetDestination(agent.transform.position);
                        running = false;
                        photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, false);
                    }
                }
                else
                {
                    agent.SetDestination(RandomNavLocal());
                    UpdateMoving(true);
                    running = true;
                    Target = null;
                    yield return new WaitForSecondsRealtime(runTime);
                    photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, false);
                }

            }
            StartCoroutine(TRangedAttack());
        }
    }
    [PunRPC]
    private void rangedCoroutineCheckRPC(bool state)
    {
        isTRangedAttackExecuting = state;
    }
    #endregion
    #region Type 3 IEnumerators 
    private IEnumerator GoBoom()
    {
        UpdateMoving(false);
#pragma warning disable CS0618 // Type or member is obsolete
        agent.Stop();
#pragma warning restore CS0618 // Type or member is obsolete
        yield return new WaitForSecondsRealtime(timeToBlow);
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, targetMask);
        PhotonNetwork.Instantiate(explosionGFX.name, transform.position, Quaternion.identity);
        boom.PlaySFX();
        Destroy(gameObject);
        if (hitPlayers.Length != 0)
        {
            Transform target = hitPlayers[1].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            if (player.isAlive == true)
            {
                player.TakeDamge(Mathf.RoundToInt(Power / Mathf.Pow(2.6f, (player.Defense / Power)))); ;
            }
        }
    }
    #endregion
    #region Mono
    private void OnValidate()
    {
        Defense = Mathf.RoundToInt(Vitality * 1.1f / 2f);
    }
    private void OnEnable()
    {
        SettingMenu.DmgNumberToggle += toggleDmgNumber;
        if (GameManger.Instance != null && EnemyTier == Tier.T1)
            Destroy(gameObject, GameManger.Instance.gameTime);
    }
    private void OnDisable()
    {
        SettingMenu.DmgNumberToggle -= toggleDmgNumber;

    }
    private void Awake()
    {
        if (typeSetting != 0)
        {
            maxHealth = Mathf.RoundToInt(Mathf.Pow(1.115f, (Vitality) / 2f));
            Defense = Mathf.RoundToInt(Vitality * 1.1f / 2f);
        }
        else if (typeSetting == 0)
        {
            maxHealth = 999999999999;
            Defense = 30;
        }
        PhotonView photonView = PhotonView.Get(this);
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;


    }
    void Start()
    {
        agent.updateRotation = false;
        if (typeSetting != 0)
            StartCoroutine(FOVRoutine());
        currentHealth = maxHealth;
        if (!showIsDirty && SettingMenu.instance != null)
        {
            showDmgNumber = SettingMenu.instance.DmgToggle;
            showIsDirty = true;
        }
    }
    private void FixedUpdate()
    {

        if (Target != null)
        {
            if (typeSetting == 1)
            {
                UpdateMoving(true);

#pragma warning disable CS0618 // Type or member is obsolete
                agent.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
                agent.SetDestination(Target.position);
                photonView.RPC("StartRotating", RpcTarget.All);
            }
            if (typeSetting == 2)
            {
                photonView.RPC("StartRotating", RpcTarget.All);
                StartCoroutine(TRangedAttack());
            }

            if (typeSetting == 3)
            {
                UpdateMoving(true);

#pragma warning disable CS0618 // Type or member is obsolete
                agent.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
                agent.SetDestination(Target.position);
                photonView.RPC("StartRotating", RpcTarget.All);

            }
        }
    }
    #endregion
    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (isDead == false)
        {
            PlayerManger player = other.GetComponent<PlayerManger>();
            if (typeSetting == 1)
                if (player != null)
                    StartCoroutine("TAttack");
            if (typeSetting == 3)
                if (player != null)
                {
                    StartCoroutine("GoBoom");
                    hiss.PlaySFX();
                }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (isDead == false)
        {
            if (typeSetting == 1)
            {
                StartCoroutine("TAttack");
                UpdateMoving(false);
            }

            if (typeSetting == 3)
            {
                PlayerManger player = other.GetComponent<PlayerManger>();
                if (player != null)
                {
                    transform.localScale += Vector3.one * Time.deltaTime / 2.5f;

                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (isDead == false)
        {
            if (typeSetting == 1)
            {
                if (Target != null)
                {
                    agent.SetDestination(Target.position);
                    UpdateMoving(true);
                }
                else if (Target == null)
                {
                    agent.SetDestination(transform.position);
                    UpdateMoving(false);
                }
            }
            if (typeSetting == 3)
            {
                PlayerManger player = other.GetComponent<PlayerManger>();
                if (player != null)
                {
                    StopCoroutine("GoBoom");
                    hiss.StopSFX();
                    transform.localScale = Vector3.one;
                }
                if (Target != null)
                {
                    agent.SetDestination(Target.position);
                    UpdateMoving(true);
                }
                else if (Target == null)
                {
                    agent.SetDestination(transform.position);
                    UpdateMoving(false);
                }
            }
        }

    }
    #endregion
    #region Recive Dmg
    public void TakeDamge(float damage)
    {
        currentHealth -= damage;
        if (showDmgNumber)
        {
            var text = Instantiate(FloatingText, FloatingTextspawn.position, Quaternion.Euler(0, 180, 0), FloatingTextspawn);
            text.GetComponent<TextMesh>().text = damage.ToString();
        }
        UnityEngine.Debug.Log(this + "takes " + damage + " damage.");
        photonView.RPC("Hit", RpcTarget.All);
        if (isDead == false)
            StartCoroutine(RangeExpand());
        if (currentHealth <= 0)
        {
            animator.SetBool("Dead", true);
            photonView.RPC("Die", RpcTarget.All);
            StartCoroutine(Dropbag());
        }
        if (typeSetting == 0)
            currentHealth = 999999999999;
    }
    [PunRPC]
    public void Die()
    {
        isDead = true;
        Target = null;
        StopCoroutine(LookAt());
#pragma warning disable CS0618 // Type or member is obsolete
        agent.Stop();
#pragma warning restore CS0618 // Type or member is obsolete
        animator.SetTrigger("Die");
        col.enabled = false;
        agent.enabled = false;
        StartCoroutine(ExecuteAfterTime());
    }
    #endregion
    #region Anim and Sound
    [PunRPC]
    private void Hit()
    {
        animator.SetTrigger("wasHurt");
        hurtSquish.PlaySFX();
        SFX soundToPlay = HurtSounds.GetRandom();
        soundToPlay.PlaySFX();
    }
    [PunRPC]
    public void StartRotating()
    {
        LookCoroutine = StartCoroutine(LookAt());
    }
    private void UpdateMoving(bool moving)
    {

        animator.SetBool("moving", moving);
    }
    [PunRPC]
    public void UpdateAttack()
    {
        animator.SetTrigger("Attack");
        SFX soundToPlay = AttackSounds.GetRandom();
        soundToPlay.PlaySFX();
    }
    #endregion
    #region Misc
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            PlayerManger player = target.GetComponent<PlayerManger>();
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (isDead == false)
                    {
                        canSeePlayer = true;
                        StopCoroutine(LoseTarget());
                        Target = target.transform;
                        if (typeSetting == 1)
                        {
                            if (player.CurrentHealth <= 0)
                                photonView.RPC("StartRotating", RpcTarget.All); ;
                        }
                        if (typeSetting == 2)
                        {
                            if (player.CurrentHealth <= 0)
                                StartCoroutine(TRangedAttack());
                        }
                        if (typeSetting == 3)
                        {
                            if (player.CurrentHealth <= 0)
                                photonView.RPC("StartRotating", RpcTarget.All);
                        }
                    }
                }
                else
                {
                    canSeePlayer = false;

                }
            }
            else
            {
                canSeePlayer = false;

            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            photonView.RPC("StartRotating", RpcTarget.All);
            UpdateMoving(true);
            StartCoroutine(LoseTarget());
        }
    }
    private Vector3 RandomNavLocal()
    {


        Vector3 finalPostion = Vector3.zero;
        Vector3 randomPos = Random.insideUnitCircle * runDistance;
        randomPos += transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, runDistance, 1))
        {
            finalPostion = hit.position;
        }
        return finalPostion;
    }
    private bool runCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, runRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            return true;
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
    public string GetEnemyTier()
    {
        return EnemyTier.ToString();
    }
    void toggleDmgNumber(bool state)
    {
        if (state)
        {
            showDmgNumber = true;
            showIsDirty = true;
        }
        else if (!state)
        {
            showDmgNumber = false;
            showIsDirty = true;
        }
    }
}
#endregion