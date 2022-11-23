using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemys : MonoBehaviourPun
{

    public static Enemys Instance;
    //Health
    [Header("Health Settings")]
    [Range(68, 100)] public float Vitality;
    public float Defense;
    [SerializeField] private float currentHealth;
    private float maxHealth;
    public bool isDead = false;

    //Fov Detection/Movement
    [Header("Detection and Move Settings")]
    [SerializeField][Range(1, 100)] private float LSpeed = 1f;
    private bool isLoseTargetExecuting = false;
    private Coroutine LookCoroutine;
    [Range(0, 360)] public float angle;
    public float radius;
    public GameObject playerRef;
    public bool canSeePlayer;
    public Transform Target;


    //attacking
    [Header("Attack Settings")]
    [SerializeField][Range(25, 150)] private int Power = 28;
    [SerializeField][Range(0, 20)] private float attackRange;
    [SerializeField][Range(1, 5)] private float attackCooldown = 3f;
    private bool isTAttackExecuting = false;
    public Transform attackPoint;



    //masks
    [Header("Layer Masks")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    //Comps
    private Animator animator;
    private NavMeshAgent agent;
    private Collider col;


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
        foreach (Collider player in hitPlayers)
        {
            if (player.GetComponent<PlayerManger>().isAlive == true)
            {
                Debug.Log("Hit " + player.name);
                animator.SetTrigger("Attack");
                yield return new WaitForSecondsRealtime(.5f);
                player.GetComponent<PlayerManger>().TakeDamge(Mathf.RoundToInt(Power / Mathf.Pow(2f, (player.GetComponent<PlayerManger>().Defense / Power)))); ;
                if (player.GetComponent<PlayerManger>().CurrentHealth == 0)
                {
                    Target = null;
                    Lost();

                }
#pragma warning disable CS0618 // Type or member is obsolete
                agent.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
                yield return new WaitForSecondsRealtime(attackCooldown);
            }
            break;
        }

        isTAttackExecuting = false;


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
        if (isTAttackExecuting)
            yield break;
        isLoseTargetExecuting = true;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(3f);
        yield return wait;
        agent.SetDestination(agent.transform.position);
        Target = null;
        Debug.Log("lost");
        animator.SetTrigger("lost");
        UpdateMoving(false);
        Debug.Log("UpdateMove");
        isLoseTargetExecuting = false;

    }


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
        Instance = this;
        StartCoroutine(FOVRoutine());
        playerRef = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
    }

    // Update is called once per fram

    private void FixedUpdate()
    {

        if (Target != null)
        {
            UpdateMoving(true);

#pragma warning disable CS0618 // Type or member is obsolete
            agent.Resume();
#pragma warning restore CS0618 // Type or member is obsolete
            agent.SetDestination(Target.position);
            StartRotating();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDead == false)
            StartCoroutine(TAttack());
    }
    private void OnTriggerStay(Collider other)
    {
        if (isDead == false)
        {
            StartCoroutine(TAttack());
            UpdateMoving(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (isDead == false)
        {
            StopCoroutine(TAttack());
            if (Target != null)
            {
                agent.SetDestination(Target.position);
                UpdateMoving(true);
            }
            UpdateMoving(false);
        }

    }
    public void TakeDamge(float damage)
    {
        photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamge_Rpc(float damage)
    {
        currentHealth -= damage;
        Debug.Log(damage);
        animator.SetTrigger("wasHurt");

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

    private void Lost()
    {
        agent.SetDestination(agent.transform.position);
        Target = null;

        Debug.Log("lost");
        animator.SetTrigger("lost");

        UpdateMoving(false);
        Debug.Log("UpdateMove");
    }

    public void StartRotating()
    {
        LookCoroutine = StartCoroutine(LookAt());
    }
    private void UpdateMoving(bool moving)
    {

        animator.SetBool("moving", moving);
    }
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        { 
            Transform target = rangeChecks[0].transform;

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
                        if (target.GetComponent<PlayerManger>().CurrentHealth != 0)
                            Target = target.transform;
                        StartRotating();
                        if (agent != null && target != null)
                            agent.SetDestination(Target.position);
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
            StartRotating();
            UpdateMoving(true);
            StartCoroutine(LoseTarget());
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }


}


