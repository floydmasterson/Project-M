using Cinemachine;
using Photon.Pun;
using Sirenix.OdinInspector;
using SmartConsole.Components;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Transform = UnityEngine.Transform;

public class PlayerManger : MonoBehaviourPun
{
    #region Vars
    //Partds
    [TabGroup("Components")]
    [Required]
    [SerializeField] CinemachineFreeLook cineCamera;

    [TabGroup("Components")]
    [Required]
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
    [ProgressBar(0, "MaxHealth", 0, 1, 0)]
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
    public static event Action<int> OnDamaged;


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
    readonly float _groundDistance = .0001f;
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
    [Required]
    public Transform attackPoint;
    [TabGroup("Attack")]
    [Required]
    public LayerMask enemyLayers;
    public bool IsLocal = false;


    //PLayer UI
    [TabGroup("Ui"), Required, SerializeField]
    GameObject UiPrefab;
    [TabGroup("Ui"), Required, SerializeField]
    GameObject InventoryPrefab;
    [TabGroup("Ui"), Required, SerializeField]
    GameObject MiniMapIcon;
    public delegate void EscapeMenu();
    public static event EscapeMenu escapeMenu;
    bool escapeMenuOpen;

    bool InvIsOpen = false;
    bool inChest = false;
    LootContainerControl lootContainerControl;
    bool locked;
    public delegate void inventoryO();
    public static event inventoryO onInventoryOpen;
    public delegate void inventoryC();
    public static event inventoryC onInventoryClose;

    //Audio 
    [TabGroup("Audio"), Required, SerializeField]
    SFX walk;
    [TabGroup("Audio"), Required, SerializeField]
    SFX run;
    [TabGroup("Audio"), Required, SerializeField]
    SFX hurt;

    PlayerController controls;

    #endregion
    #region Ienumerators
    IEnumerator ExecuteAfterTime()
    {
        if (lifes > 0)
        {
            OnDeath(this);
            yield return new WaitForSeconds(4);
            //Particl
            //lifes--;
            Respawn();
            yield return new WaitForSeconds(1.5f);
            isInvulnerable = false;
        }
        else
        {
            yield return new WaitForSeconds(4);
            Destroy(photonView);
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
        Vector3 initalDirection = Direction.normalized;
        isRollExecuting = true;
        float startTime = Time.time;
        animator.SetTrigger("roll");
        while (Time.time < startTime + .6f)
        {
            canMove = false;
            characterController.Move(initalDirection * (pushAmt + Character.Instance.Agility.Value * .8f) * Time.fixedDeltaTime);
            yield return null;
        }
        canMove = true;
        isRollExecuting = false;
    }
    #endregion
    #region Mono
    private void OnEnable()
    {
        GameManger.PvPon += EnbalePvpCombat;
        ConsoleSystem.ConsoleOpenClose += UiLockOut;

        controls.Player.Enable();
        controls.Player.Attack.performed += context => inputAttack();
        controls.Player.Roll.performed += context => inputRoll();
        controls.Player.LockedCam.performed += context => CamSwitch();
        controls.Player.QuickSlot.performed += context => QuickSlot();
        controls.Player.InventoryOpen.performed += context => InventorySwitch();
        controls.Player.InteractOpen.performed += context => Interact();
        controls.Player.MapOpen.performed += context => Map();
        controls.Player.Menu.performed += context => inputEscapeMenu();
        controls.Player.Sprint.performed += Sprint;
        controls.Player.Sprint.canceled += Sprint;
    }
    private void OnDisable()
    {
        GameManger.PvPon -= EnbalePvpCombat;
        ConsoleSystem.ConsoleOpenClose -= UiLockOut;

        controls.Player.Disable();
        controls.Player.Attack.performed -= context => inputAttack();
        controls.Player.Roll.performed -= context => inputRoll();
        controls.Player.LockedCam.performed -= context => CamSwitch();
        controls.Player.QuickSlot.performed -= context => QuickSlot();
        controls.Player.InventoryOpen.performed -= context => InventorySwitch();
        controls.Player.InteractOpen.performed -= context => Interact();
        controls.Player.MapOpen.performed -= context => Map();
        controls.Player.Menu.performed -= context => inputEscapeMenu();
        controls.Player.Sprint.performed -= Sprint;
        controls.Player.Sprint.canceled -= Sprint;
    }
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)
        {
            #region Control Events
            controls = new PlayerController();
            IsLocal = true;
           
            #endregion

            animator = GetComponent<Animator>();
            cam = GameObject.FindGameObjectWithTag("MainCamera");
            characterController = GetComponent<CharacterController>();
            Rb = GetComponent<Rigidbody>();
            gameObject.name = PhotonNetwork.NickName;
            MiniMapIcon.SetActive(true);
            cineCamera.Priority = 10;

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
    private void FixedUpdate()
    {
        if (photonView.IsMine && isAlive == true)
        {
            Vector2 inputVector = controls.Player.Movment.ReadValue<Vector2>();
            float x = inputVector.x;
            float y = inputVector.y;

            Vector3 direction = new Vector3(x, 0f, y).normalized;

            //gravity
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

            if (canMove && !locked)
            {

                //Primary Move + Camera smooth effect
                if (direction.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelc, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    Direction = moveDirection;
                    characterController.Move(speed * Time.fixedDeltaTime * moveDirection.normalized);
                }
                //Locked cam movemnt
            }
            else if (locked)
            {
                turn += controls.Player.Lockturn.ReadValue<Vector2>();
                transform.rotation = Quaternion.Euler(0f, turn.x, 0f);
                if (direction.magnitude >= 0.1f && canMove)
                {
                    characterController.Move(speed * Time.fixedDeltaTime * (transform.forward * y + transform.right * x).normalized);
                    Vector3 xDir = transform.right * x;
                    Vector3 yDir = transform.forward * y;

                    if (x > 0 || x < 0)
                        Direction = xDir;
                    if (y > 0 || y < 0)
                        Direction = yDir;
                    if (x == 0f && y == 0f)
                    {
                        Direction = Vector3.zero;
                    }
                }
            }
            UpdateMoving(x != 0f || y != 0f);
            //Roll Cooldown
            if (ActCooldown > 0)
            {
                ActCooldown -= Time.fixedDeltaTime;
                if (ActCooldown <= 0)
                {
                    ActCooldown = 0;
                }
            }
        }
    }
    #region Controler 
    void inputRoll()
    {
        if (ActCooldown <= 0 && characterController.velocity != Vector3.zero)
        {
            ActCooldown = dodgeCooldown;
            StartCoroutine(IFrames(.8f));
            StartCoroutine(Roll());
        }
    }
    void QuickSlot()
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
    void inputAttack()
    {
        if (canAttack == true && !isRollExecuting)
        {
            StartCoroutine(AttackSet());
        }
    }
    void CamSwitch()
    {
        if (locked == false)
            ForwardCamLock(true);
        else if (locked == true)
            ForwardCamLock(false);
    }
    void InventorySwitch()
    {
        if (inChest == false && isAlive == true)
        {
            if (InvIsOpen == false)
            {
                UpdateMoving(false);
                UpdateRun(false);
                InvIsOpen = true;
                onInventoryOpen();

                CursorToggle(true);
                controls.Player.Disable();
                controls.Inventory.Enable();
                controls.Inventory.InventoryClose.performed += context => InventorySwitch();
            }
            else if (InvIsOpen == true)
            {
                onInventoryClose();
                CursorToggle(false);
                cineCamera.m_XAxis.m_MaxSpeed = 250;
                InvIsOpen = false;
                controls.Inventory.Disable();
                controls.Player.Enable();
                controls.Inventory.InventoryClose.performed -= context => InventorySwitch();
            }
        }
    }
    void Interact()
    {
        if (lootContainerControl != null && lootContainerControl.pickUpAllowed && lootContainerControl.isOpen == false)
        {
            UpdateMoving(false);
            UpdateRun(false);
            controls.Player.Disable();
            lootContainerControl.Open();
            onInventoryOpen();
            inChest = true;
            controls.Container.Enable();
            controls.Container.ContainerClose.performed += context => Interact();
            CursorToggle(true);
        }
        else if (lootContainerControl != null && lootContainerControl.isOpen == true)
        {
            lootContainerControl.Close();
            onInventoryClose();
            inChest = false;
            controls.Container.Disable();
            controls.Player.Enable();
            controls.Container.ContainerClose.performed -= context => Interact();
            CursorToggle(false);
        }
    }
    void Map()
    {
        if (!MapManager.Instance.mapOpen)
        {
            MapManager.Instance.MapChange();
            controls.Player.Disable();
            controls.Map.Enable();
            controls.Map.MapClose.performed += context => Map();
            CursorToggle(true);
        }
        else if (MapManager.Instance.mapOpen)
        {
            MapManager.Instance.MapChange();
            controls.Map.MapClose.performed -= context => Map();
            controls.Map.Disable();
            controls.Player.Enable();
            CursorToggle(false);
        }
    }
    void inputEscapeMenu()
    {
        if (!escapeMenuOpen)
        {
            if (escapeMenu != null)
                escapeMenu();
            controls.Player.Disable();
            CursorToggle(true);
            escapeMenuOpen = true;
            controls.Escape.Enable();
            controls.Escape.CloseEscape.performed += context => inputEscapeMenu();
        }
        else if (escapeMenuOpen)
        {
            escapeMenuOpen = false;
            controls.Player.Enable();
            controls.Escape.Disable();
            controls.Escape.CloseEscape.performed -= context => inputEscapeMenu();
            CursorToggle(false);
        }

    }
    void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speed = CheckSprintSpeed();
            UpdateRun(true);
        }
        if (context.canceled)
        {
            CheckSpeed();
            UpdateRun(false);
        }
    }
    #endregion
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
        {
            animator.SetTrigger("Attack 0");

        }
    }
    #endregion
    #region UI
    public void UiLockOut(bool state)
    {
        if (state == true)
        {
            onInventoryClose();
            controls.Player.Disable();
            canMove = false;
            canAttack = false;
            CursorToggle(true);
        }
        else if (state == false)
        {
            controls.Player.Enable();
            canMove = true;
            canAttack = true;
            CursorToggle(false);
        }
    }
    void CursorToggle(bool state)
    {
        if (state == true)
        {
            cineCamera.m_XAxis.m_MaxSpeed = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (state == false)
        {
            cineCamera.m_XAxis.m_MaxSpeed = 250;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion
    #region Damage and Healing
    public void TakeDamge(int damage, object attacker)
    {

        if (attacker is Enemys)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("wasHurt");
                photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
            }
        }
        else if (attacker is PlayerManger)
        {
            if (photonView.IsMine)
                animator.SetTrigger("wasHurt");
            photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
        }
        else if (attacker is null)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("wasHurt");

                photonView.RPC("TakeDamge_Rpc", RpcTarget.All, damage);
            }
        }

    }

    [PunRPC]
    public void TakeDamge_Rpc(int damage)
    {
        if (isInvulnerable == false)
        {
            hurt.PlaySFX();
            Debug.Log(this + "takes " + damage + " damage.");
            if (photonView.IsMine)
            {
                if (gameObject.GetComponent<MeeleController>() != null)
                    OnDamaged(damage);
                CurrentHealth -= damage;
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
            onInventoryClose();
            animator.SetTrigger("Die");
            canAttack = false;
            canMove = false;
            isAlive = false;
            isInvulnerable = true;
            StartCoroutine(ExecuteAfterTime());
            Debug.Log(this + " died");
        }
    }
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        Debug.Log(this + " healed for " + amount);
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
            LootContainerControl chest = other.gameObject.GetComponent<LootContainerControl>();
            lootContainerControl = chest;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Chest") && lootContainerControl == null)
        {
            LootContainerControl chest = other.gameObject.GetComponent<LootContainerControl>();
            lootContainerControl = chest;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Chest"))
        {
            lootContainerControl = null;
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
    [PunRPC]
    public void PlaySFXRPC(SFX sFX)
    {
        sFX.PlaySFX();
    }
}
#endregion