using Newtonsoft.Json.Bson;
using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


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
    [Tooltip(" 0= target dummy 1 = Chase & Melee 2 = Avoid & Ranged !3 = Chase & Boom! not set !4 = hybrid! not set")]
    [Range(0, 4)] public int typeSetting = 0;

    [EnumToggleButtons]
    public Tier EnemyTier; 

    //Health
    [TabGroup("Health")]
    [Range(68, 100)] public float Vitality;
    [TabGroup("Health")]
    public float Defense;
    [TabGroup("Health")]
    [ProgressBar(0, "maxHealth", 0, 1, 0)]
    [SerializeField] float currentHealth;
    float maxHealth;
    [TabGroup("Health")]
    public bool isDead = false;

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
    public Transform attackPoint;
    [TabGroup("Attack")]
    [SerializeField][Range(0, 20)] float attackRange;
    [TabGroup("Attack")]
    [SerializeField][Range(1, 5)] float attackCooldown = 2f;
    [TabGroup("Attack")]
    [ShowIf("@typeSetting == 2")]
    [SerializeField] GameObject rangedProjectile;
    bool isTAttackExecuting = false;
    bool isTRangedAttackExecuting = false;
    //masks
    [TabGroup("Detection")]
    [SerializeField] LayerMask targetMask;
    [TabGroup("Detection")]
    [SerializeField] LayerMask obstructionMask;
    //Comps
    private Animator animator;
    private NavMeshAgent agent;
    private Collider col;
    #endregion
    #region Base IEnumerators 
    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(4);
        //Particl
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            Destroy(t.gameObject);
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;

            FieldOfViewCheck();
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

                time += Time.deltaTime * LSpeed;

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
        agent.SetDestination(agent.transform.position);
        Debug.Log(this + " is lost");
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
        if (typeSetting == 1)
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
                player.TakeDamge(Mathf.RoundToInt(Power / Mathf.Pow(2f, (player.Defense / Power)))); ;
                if (player.CurrentHealth <= 0)
                {
                    Target = null;
                    Lost();
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
            if (Target)
                photonView.RPC("rangedCoroutineCheckRPC", RpcTarget.All, true);
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
    private bool rangedCoroutineCheckRPC(bool state)
    {
        isTRangedAttackExecuting = state;
        return isTRangedAttackExecuting;
    }
    #endregion
    #region Type 3 IEnumerators 
    #endregion
    #region Mono
    private void Awake()
    {
        maxHealth = Mathf.RoundToInt(Mathf.Pow(1.115f, (Vitality) / 2f));
        Defense = Mathf.RoundToInt(Vitality * 1.1f / 2f);
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
                Debug.Log("?");
        }
    }
    #endregion
    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (isDead == false)
        {
            if (typeSetting == 1)
                StartCoroutine("TAttack");

            if (typeSetting == 3)
                Debug.Log("?");
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
                Debug.Log("?");
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
                UpdateMoving(false);
            }
        }

    }
    #endregion
    #region Recive Dmg
    public void TakeDamge(float damage)
    {
        photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamge_Rpc(float damage)
    {
        currentHealth -= damage;
        Debug.Log(this + "takes " + damage + " damage.");
        animator.SetTrigger("wasHurt");
        if (isDead == false)
            StartCoroutine(RangeExpand());
        if (currentHealth <= 0)
        {
            animator.SetBool("Dead", true);
            photonView.RPC("Die", RpcTarget.All);
        }
       
    }

    [PunRPC]
    public void Die()
    {
        isDead = true;
        StopCoroutine(LookAt());
#pragma warning disable CS0618 // Type or member is obsolete
        agent.Stop();
#pragma warning restore CS0618 // Type or member is obsolete
        Target = null;
        animator.SetTrigger("Die");

        col.enabled = false;
        agent.enabled = false;
        StartCoroutine(ExecuteAfterTime());

    }
    #endregion
    #region Anim
    private void Lost()
    {
        agent.SetDestination(agent.transform.position);
        Target = null;

        Debug.Log("lost");
        animator.SetTrigger("lost");

        UpdateMoving(false);
        Debug.Log("UpdateMove");
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
                            if (agent != null && target != null)
                                agent.SetDestination(Target.position);

                        }
                        if (typeSetting == 2)
                        {
                            if (player.CurrentHealth <= 0)
                                StartCoroutine(TRangedAttack());

                        }
                        if (typeSetting == 3)
                            Debug.Log("?");
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

}
#endregion