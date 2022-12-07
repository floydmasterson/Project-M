using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Cursor = UnityEngine.Cursor;
using Transform = UnityEngine.Transform;

public class PlayerManger : MonoBehaviourPun
{
    #region Vars
    //Partds
    [TabGroup("Components")]
    [SerializeField] CinemachineFreeLook cineCamera;

    [TabGroup("Components")]
    [SerializeField] CinemachineVirtualCamera lockCamera;

    [TabGroup("Components")]
    [SerializeField] GameObject player;

    [TabGroup("Components")]
    [SerializeField] Collider col;

    [TabGroup("Components")]
    GameObject cam;

    CharacterController characterController;
    Animator animator;
    Rigidbody Rb;

   
    //hp
    float _maxHealth;
    [TabGroup("Health")]
    [ProgressBar(0, "MaxHealth" , 0 ,1 ,0)]
    [SerializeField] float _currentHealth;
    [TabGroup("Health")]
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
            if (_currentHealth < 0 && !isAlive)
            {
                _currentHealth = 0;
            }
        }
    }
    [TabGroup("Health")]
    [SerializeField] float _defense;
    [TabGroup("Health")]
    public float Defense
    {
        get { return _defense; }
        set { _defense = value; }
    }
    [TabGroup("Health")]
    public float DefenseMod = 0;
    [TabGroup("Health")]
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    [TabGroup("Health")]
    public bool isAlive = true;
    [TabGroup("Health")]
    public bool isInvulnerable = false;
    [TabGroup("Health")]
    [ProgressBar(0, 3, 1, 0, 0, Segmented = true)]
    public int lifes = 3;
    [TabGroup("Health")]
    public Transform spawnPoint;
    public delegate void Death(PlayerManger player);
    public static event Death OnDeath;
   

    //Movment
    [TabGroup("Movement")]
    [SerializeField] float turnSmoothTime = 0.1f;
    [TabGroup("Movement")]
    [SerializeField] float SprintSpeed = 12f;
    [TabGroup("Movement")]
    [SerializeField] float speed = 6f;
    [TabGroup("Movement")]
    public float pushAmt = 6f;
    [TabGroup("Movement")]
    [SerializeField] float dodgeCooldown = 1f;
    [TabGroup("Movement")]
    [SerializeField] Vector2 turn;
    readonly float _groundDistance = 1f;
    readonly float _gravity = -9.81f;
    float turnSmoothVelc;
    [Space]
    [TabGroup("Movement")]
    public Transform groundCheck;
    [TabGroup("Movement")]
    public LayerMask groundMask;
    [TabGroup("Movement")]
    public bool isGrounded;
    [TabGroup("Movement")]
    public bool canMove = true;
    [TabGroup("Movement")]
    public bool canLook = true;
    Vector3 velocity;
    Vector3 Direction;
    float ActCooldown;
    bool isRollExecuting = false;


    //attacking
    [TabGroup("Attack")]
    [SerializeField] float attackCoolDown = 1f;
    bool isAttackSetExecuting = false;
    [TabGroup("Attack")]
    public float attackRange = .5f;
    [TabGroup("Attack")]
    public bool LifeSteal = false;
    [TabGroup("Attack")]
    public bool canAttack = true;
    [TabGroup("Attack")]
    public bool pvp = false;
    [TabGroup("Attack")]
    public Transform attackPoint;
    [TabGroup("Attack")]
    public LayerMask enemyLayers;


    //PLayer UI
    [TabGroup("Ui")]
    [SerializeField] GameObject UiPrefab;
    [TabGroup("Ui")]
    [SerializeField] GameObject InventoryPrefab;
    bool InvIsOpen = false;
    bool inChest = false;
    ChestControl chestControl;
    bool locked;
    public delegate void inventoryO();
    public static event inventoryO onInventoryOpen;
    public delegate void inventoryC();
    public static event inventoryC onInventoryClose;


    #endregion
    #region Ienumerators
    IEnumerator ExecuteAfterTime()
    {
        if (lifes > 0)
        {
            OnDeath(this);
            yield return new WaitForSeconds(4);
            //Particl
            lifes--;
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(false);
            Respawn();
        }
        else
        {
            yield return new WaitForSeconds(4);
            Destroy(this);
        }
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
    public IEnumerator PreLoad()
    {
        onInventoryOpen();
        yield return new WaitForSeconds(.001f);
        onInventoryClose();
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
    private void OnEnable()
    {
        GameManger.PvPon += EnbalePvpCombat;
    }
    private void OnDisable()
    {
        GameManger.PvPon -= EnbalePvpCombat;
    }
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
        StartCoroutine(PreLoad());
        CurrentHealth = MaxHealth;

    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && inChest == false && isAlive == true)
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                UsableItem quickItem = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Item as UsableItem;
                QuickSlot quickSlot = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true);
                if (quickItem != null)
                {
                    if (quickItem.UseableCheck())
                    {
                        quickItem.Use(Character.Instance);
                        quickSlot.Amount--;
                        PlayerUi.Instance.CheckAmount();
                        quickItem.Destroy();

                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (locked == false)
                    ForwardCamLock(true);
                else if (locked == true)
                    ForwardCamLock(false);
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
                    if (direction.magnitude >= 0.1f && !locked)
                    {
                        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelc, turnSmoothTime);
                        transform.rotation = Quaternion.Euler(0f, angle, 0f);

                        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                        Direction = moveDirection;
                        characterController.Move(speed * Time.fixedDeltaTime * moveDirection.normalized);
                    }
                    else if (locked)
                    {

                        turn.x += Input.GetAxisRaw("Mouse X") * 1.5f;
                        transform.localRotation = Quaternion.Euler(0f, turn.x, 0f);
                        if (direction.magnitude >= 0.1f)
                        {
                            characterController.Move(speed * Time.fixedDeltaTime * transform.forward * y);
                            characterController.Move(speed * Time.fixedDeltaTime * transform.right * x);
                            Vector3 xDir = transform.right * x;
                            Vector3 yDir = transform.forward * y;

                            if (x > 0 || x < 0)
                                Direction = xDir;
                            if (y > 0 || y < 0)
                            {
                                Direction = yDir;
                            }
                            if (x == 0f && y == 0f)
                            {
                                Direction = Vector3.zero;
                            }
                        }
                    }
                }
                    
                UpdateMoving(x != 0f || y != 0f);


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
        Cursor.lockState = CursorLockMode.None;

    }
    public void UiUnlock()
    {
        canMove = true;
        canAttack = true;
        cineCamera.m_XAxis.m_MaxSpeed = 250;
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
            onInventoryClose();
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
    private void Respawn()
    {
        if (lifes > 0)
        {
            gameObject.transform.position = spawnPoint.position;
            CurrentHealth = MaxHealth;
            isAlive = true;
            canMove = true;
            canAttack = true;
            col.isTrigger = true;
            animator.SetTrigger("Res");
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    #endregion
    #region StatChecks

    public void CheckMaxHealth()
    {
        _maxHealth = Mathf.RoundToInt(Mathf.Pow(1.115f, (Character.Instance.Vitality.Value / 2f)));
    }
    public float CheckDefense()
    {
        _defense = Mathf.RoundToInt(Character.Instance.Vitality.Value * 1.1f / 2f) + DefenseMod;
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
    private void EnbalePvpCombat()
    {
        enemyLayers |= LayerMask.GetMask("enenmyMask") | LayerMask.GetMask("target");
        pvp = true;
    }
    [PunRPC]
    private void SetName()
    {
        gameObject.name = PhotonNetwork.NickName;
    }
    private void ForwardCamLock(bool state)
    {
        if (state == true)
        {
            locked = true;
            cineCamera.Priority = 0;
            lockCamera.Priority = 10;

        }
        else if (state == false)
        {
            locked = false;
            cineCamera.Priority = 10;
            lockCamera.Priority = 0;
        }
    }
    #endregion
}
