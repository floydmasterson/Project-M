using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerManger : MonoBehaviourPun
{
    #region Vars
    //Partds
    [SerializeField] private CinemachineFreeLook cineCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private Collider col;
    private GameObject cam;
    private CharacterController characterController;
    private Animator animator;
    private Rigidbody Rb;

    [Space]
    //hp
    [SerializeField] private float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (_currentHealth > MaxHealth)
            {
                _currentHealth = MaxHealth;
            }
            if (_currentHealth > 0 && !isAlive)
            {
                _currentHealth = 0;
            }
        }
    }
    private float _maxHealth;
    private float _defense;
    public float Defense
    {
        get { return _defense; }
        set { _defense = value; }
    }

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    public bool isAlive = true;
    public bool isInvulnerable = false;
    [Space]

    //Movment
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float SprintSpeed = 12f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float pushAmt = 3;
    [SerializeField] private float dodgeCooldown = 1;
    private readonly float _groundDistance = 1f;
    private readonly float _gravity = -9.81f;
    private float turnSmoothVelc;
    [Space]
    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded;
    public bool canMove = true;
    public bool canLook = true;
    Vector3 velocity;
    Vector3 Direction;
    private float ActCooldown;
    private bool isRollExecuting = false;
    [Space]

    //attacking
    [SerializeField] private float attackCoolDown = 1f;
    private bool isAttackSetExecuting = false;
    public float attackRange = .5f;
    public bool LifeSteal = false;
    public bool canAttack = true;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Space]
    //PLayer UI
    [SerializeField] private GameObject UiPrefab;
    [SerializeField] private GameObject InventoryPrefab;
    private bool InvIsOpen = false;
    private bool inChest = false;
    private ChestControl chestControl;
    public delegate void inventoryO();
    public static event inventoryO onInventoryOpen;
    public delegate void inventoryC();
    public static event inventoryC onInventoryClose;

    #endregion
    #region Ienumerators
    IEnumerator ExecuteAfterTime()
    {

        yield return new WaitForSeconds(4);
        //Particl
        player.SetActive(false);
    }
    IEnumerator AttackSet()
    {
        if (photonView.IsMine)
        {
            if (isAttackSetExecuting)
                yield break;
            isAttackSetExecuting = true;
            canAttack = false;
            if (LifeSteal == true)
            {
                GetComponent<MeeleController>().AttackLifeSteal();
            }
            else
            {
                GetComponent<IAttack>().Attack();
            }
            yield return new WaitForSeconds(attackCoolDown);
            canAttack = true;
            isAttackSetExecuting = false;
        }


    }
    public IEnumerator MoveLock(float time)
    {
        canMove = false;
        yield return new WaitForSecondsRealtime(time);
        canMove = true;

    }
    public IEnumerator IFrames(float time)
    {

        isInvulnerable = true;
        yield return new WaitForSecondsRealtime(time);
        isInvulnerable = false;



    }
    IEnumerator Roll()
    {
        if (isRollExecuting)
            yield break;
        isRollExecuting = true;
        float startTime = Time.time;
        animator.SetTrigger("roll");
        while (Time.time < startTime + .5f)
        {
            characterController.Move(Direction.normalized * (pushAmt + Character.Instance.Agility.Value / 2) * Time.fixedDeltaTime);
            yield return null;
        }
        isRollExecuting = false;
    }
    #endregion
    #region Mono
    void Awake()
    {


        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoad(this.gameObject);
        if (photonView.IsMine)
        {
            photonView.RPC("SetName", RpcTarget.All);
            //gather comp set cam
            cam = GameObject.FindGameObjectWithTag("MainCamera");
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cineCamera.Priority = 10;
            Rb = GetComponent<Rigidbody>();
            if (InventoryPrefab != null)
            {
                GameObject _uiGoi = Instantiate(InventoryPrefab) as GameObject;
                _uiGoi.GetComponent<InventoryUi>().SetTargetI(this);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> IventoryPrefab reference on player Prefab.", this);
            }
            if (UiPrefab != null)
            {
                GameObject _uiGo = Instantiate(UiPrefab) as GameObject;
                _uiGo.GetComponent<PlayerUi>().SetTarget(this);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }
        else
        {
            cineCamera.Priority = 0;
        }

    }

    private void Start()
    {

        CheckMaxHealth();
        CheckDefense();
        CurrentHealth = MaxHealth;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && inChest == false)
            {
                if (InvIsOpen == false)
                {
                    UpdateMoving(false);
                    UiLockOut();
                    InvIsOpen = true;
                    onInventoryOpen();
                }
                else
                {
                    onInventoryClose();
                    InvIsOpen = false;
                    UiUnlock();
                }
            }

            if (canAttack == true)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {

                    StartCoroutine(AttackSet());
                }
            }

            if (chestControl != null && chestControl.pickUpAllowed && chestControl.isOpen == false && Input.GetKeyDown(KeyCode.E))
            {
                UiLockOut();
                UpdateMoving(false);
                chestControl.Open();
                inChest = true;
                onInventoryOpen();


            }
            if (chestControl != null && chestControl.isOpen == true)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    chestControl.Close();
                    inChest = false;
                    UiUnlock();
                    onInventoryClose();
                }
            }
        }


    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (isAlive == true)
            {

                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");


                Vector3 direction = new Vector3(x, 0f, y).normalized;


                //gravity
                if (InvIsOpen == false && canMove == true)
                {
                    isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
                    if (isGrounded && velocity.y < 0)
                    {
                        velocity.y = -2f;

                    }
                    else
                    {
                        velocity.y += _gravity * Time.fixedDeltaTime;
                        characterController.Move(velocity * Time.fixedDeltaTime);
                    }
                    //move and cam smooth
                    if (direction.magnitude >= 0.1f)
                    {
                        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelc, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        Direction = moveDirection;
                        characterController.Move(speed * Time.fixedDeltaTime * moveDirection.normalized);
                    }

                    UpdateMoving(x != 0f || y != 0f);
                }

                //sprint
                if (Input.GetButton("Fire3"))
                {
                    speed = CheckSprintSpeed();
                    UpdateRun(speed >= SprintSpeed);
                }
                else
                {
                    CheckSpeed();
                    UpdateRun(false);
                }
                if (Input.GetMouseButton(1))
                {
                    if (ActCooldown <= 0 && direction.magnitude >= 0.1f)
                    {

                        ActCooldown = dodgeCooldown;

                        StartCoroutine(IFrames(.8f));
                        StartCoroutine(Roll());


                    }
                    else
                    {
                        ActCooldown -= Time.fixedDeltaTime;
                        if (ActCooldown <= 0)
                        {
                            ActCooldown = 0;
                        }
                    }

                }
                else
                {
                    ActCooldown -= Time.fixedDeltaTime;
                    if (ActCooldown <= 0)
                    {
                        ActCooldown = 0;
                    }
                }

            }
        }
    }
    #endregion
    #region Animations
    public void UpdateMoving(bool moving)
    {
        if (canMove == true)
        {
            animator.SetBool("moving", moving);
        }
        else
        {
            moving = false;
            animator.SetBool("moving", moving);
        }
    }

    private void UpdateRun(bool running)
    {
        animator.SetBool("running", running);
    }
    [PunRPC]
    public void UpdateAttack()
    {
        if (photonView.IsMine)
            animator.SetTrigger("Attack 0");
    }
    #endregion
    #region UI
    public void UiLockOut()
    {
        canMove = false;
        canAttack = false;
        cineCamera.m_XAxis.m_MaxSpeed = 0f;
        cineCamera.m_YAxis.m_MaxSpeed = 0f;
        Cursor.lockState = CursorLockMode.None;

    }
    public void UiUnlock()
    {
        canMove = true;
        canAttack = true;
        cineCamera.m_XAxis.m_MaxSpeed = 250;
        cineCamera.m_YAxis.m_MaxSpeed = 0.5f;
        Cursor.lockState = CursorLockMode.Locked;

    }
    #endregion
    #region Damage and Healing
    public void TakeDamge(float damage)
    {
        photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamge_Rpc(float damage)
    {
        if (photonView.IsMine)
        {
            if (isInvulnerable == false)
            {
                CurrentHealth -= damage;
                animator.SetTrigger("wasHurt");
                if (CurrentHealth <= 0 && isAlive == true)
                {
                    CurrentHealth = 0;
                    col.isTrigger = false;
                    Die();
                    photonView.RPC("Die", RpcTarget.All);
                }
            }
        }
    }
    [PunRPC]
    public void Die()
    {
        if (photonView.IsMine)
        {

            Debug.Log("you die");
            animator.SetTrigger("Die");
            canAttack = false;
            canMove = false;
            isAlive = false;

            StartCoroutine(ExecuteAfterTime());
        }




    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    #endregion
    #region StatChecks

    public void CheckMaxHealth()
    {
        _maxHealth = Mathf.RoundToInt(Mathf.Pow(1.115f, (Character.Instance.Vitality.Value / 2f)));
    }
    public float CheckDefense()
    {
        _defense = Mathf.RoundToInt(Character.Instance.Vitality.Value * 1.1f / 2f);
        return _defense;
    }

    public float CheckSpeed()
    {
        speed = (6 + Character.Instance.Agility.Value / 2);
        return speed;
    }

    public float CheckSprintSpeed()
    {
        speed = (12 + Character.Instance.Agility.Value / 2);
        SprintSpeed = speed;
        return SprintSpeed;
    }

    #endregion
    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Chest"))
        {
            ChestControl chest = other.gameObject.GetComponent<ChestControl>();
            chestControl = chest;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Chest"))
        {
            chestControl = null;
        }
    }
    #endregion
    #region Misc
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
    [PunRPC]
    private void SetName()
    {
        gameObject.name = PhotonNetwork.NickName;
    }
    #endregion
}
