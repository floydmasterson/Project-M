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

    [TabGroup("Components"), SerializeField, Required]
    private CinemachineVirtualCamera lockCamera;

    [TabGroup("Components"), SerializeField, Required]
    private GameObject player;

    [TabGroup("Components"), SerializeField, Required]
    private Collider col;

    [TabGroup("Components"), Required]
    public PlayerInput playerInput;

    [TabGroup("Components"), SerializeField, Required]
    private CharacterController characterController;

    [TabGroup("Components"), SerializeField, Required]
    private Animator animator;

    [TabGroup("Components"), SerializeField, Required]
    private Rigidbody Rb;

    [TabGroup("Components"), SerializeField]
    private GameObject FloatingText;

    [TabGroup("Components"), SerializeField]
    private Transform DmgTextspawn;

    [TabGroup("Components"), SerializeField]
    private Transform HealTextspawn;

    [TabGroup("Components"), Required]
    public bool IsLocal = false;

    private GameObject cam;
    LootContainerManager lootContainerManager;


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

    public bool noClip;

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
    [SerializeField] private float SprintSpeed = 12f;
    [TabGroup("Movement")]
    [SerializeField] private float speed = 6f;
    [TabGroup("Movement")]
    [SerializeField] public float speedMod = 0;
    [TabGroup("Movement")]
    public float pushAmt = 6f;
    [TabGroup("Movement")]
    [SerializeField] float dodgeCooldown = 1f;
    [TabGroup("Movement")]
    [SerializeField] Vector2 turn;
    readonly float _groundDistance = .0001f;
    readonly float _gravity = -9.81f;
    private float turnSmoothVelc;
    [Space]
    [TabGroup("Movement")]
    public Transform groundCheck;
    [TabGroup("Movement")]
    public LayerMask groundMask;
    [TabGroup("Movement")]
    public bool isGrounded;
    [TabGroup("Movement")]
    public bool canMove = true;
    [HideInInspector]
    public float currentCameraSpeed = 250;

    private Vector2 inputMove;
    private Vector2 inputTurn;
    private Vector3 velocity;
    private Vector3 Direction;
    private float ActCooldown;
    private bool isRollExecuting = false;
    private float turnSens = 0.025f;


    //attacking
    [TabGroup("Attack")]
    [SerializeField] float attackCoolDown = 1f;
    private bool isAttackSetExecuting = false;
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



    //PLayer UI
    [TabGroup("Ui"), Required, SerializeField]
    private GameObject UiPrefab;
    [TabGroup("Ui"), Required, SerializeField]
    private GameObject InventoryPrefab;
    [TabGroup("Ui"), Required, SerializeField]
    private GameObject MiniMapIcon;
    [TabGroup("Ui")]
    public bool InvIsOpen = false;
    [TabGroup("Ui")]
    public bool inChest = false;
    public Character character;
    [HideInInspector]
    public ShopController shop;

    private bool escapeMenuOpen;
    private const string gamepadScheme = "Controller";
    private bool locked;
    private bool showHealingNumber;
    private bool showDmgNumber;
    public bool inShop;

    public delegate void EscapeMenu();
    public static event EscapeMenu escapeMenu;
    public delegate void MapD();
    public static event MapD CrossToggle;
    public delegate void inventoryO();
    public static event inventoryO onInventoryOpen;
    public delegate void inventoryC();
    public static event inventoryC onInventoryClose;

    //Audio 
    [TabGroup("Audio"), Required, SerializeField]
    private SFX walk;
    [TabGroup("Audio"), Required, SerializeField]
    private SFX run;
    [TabGroup("Audio"), Required, SerializeField]
    private SFX hurt;

    private const string Keybaord_Sensativity = "KS";
    private const string Gamepad_Sensativity = "GS";
    #endregion
    #region enumerators
    private IEnumerator ExecuteAfterTime()
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
    private IEnumerator AttackSet()
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
    private IEnumerator StartRoll()
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
            characterController.Move((pushAmt + Character.Instance.Agility.Value) * Time.deltaTime * initalDirection);
            yield return null;
        }
        canMove = true;
        isRollExecuting = false;
    }
    private IEnumerator shopTimer()
    {
        canMove = false;
        UpdateMoving(false);
        UpdateRun(false);
        StartCoroutine(IFrames(2));
        PlayerUi.Instance.ShopTime();
        yield return new WaitForSecondsRealtime(1.8f);
        transform.position = new Vector3(1753, -107, -592);
        inShop = true;
        canMove = true;
    }
    #endregion
    #region Mono
    private void OnEnable()
    {
        GameManger.Instance.TimerOver += () =>
        {
            if (!inShop)
            {
                StartCoroutine(shopTimer());
            }
        };
        ConsoleSystem.ConsoleOpenClose += UiLockOut;
        SettingMenu.HealNumberToggle += toggleHealNumber;
        SettingMenu.DmgNumberToggle += toggleDmgNumber;
    }
    private void OnDisable()
    {
        GameManger.Instance.TimerOver += () =>
        {
            if (!inShop)
            {
                StartCoroutine(shopTimer());
            }
        };
        ConsoleSystem.ConsoleOpenClose -= UiLockOut;
        SettingMenu.HealNumberToggle -= toggleHealNumber;
        SettingMenu.DmgNumberToggle -= toggleDmgNumber;
    }
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine)
        {
            IsLocal = true;
            cam = GameObject.FindGameObjectWithTag("MainCamera");
            gameObject.name = PhotonNetwork.NickName;
            lootContainerManager = GetComponent<LootContainerManager>();
            MiniMapIcon.SetActive(true);
            lockCamera.Priority = 10;
            locked = true;
            if (UiPrefab != null)
            {
                GameObject _uiGo = Instantiate(UiPrefab) as GameObject;
                _uiGo.GetComponent<PlayerUi>().SetTarget(this);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            if (InventoryPrefab != null)
            {
                GameObject _uiGoi = Instantiate(InventoryPrefab) as GameObject;
                _uiGoi.GetComponent<InventoryUi>().SetTargetI(this);
                character = _uiGoi.GetComponent<Character>();
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> IventoryPrefab reference on player Prefab.", this);
            }
            GamepadCursor.Instance.playerInput = playerInput;

        }
        else
        {
            lockCamera.Priority = 0;
        }

    }
    private void Start()
    {
        CheckMaxHealth();
        CheckDefense();
        StartCoroutine(PreLoad());
        CurrentHealth = MaxHealth;
        updateSens();

    }
    private void FixedUpdate()
    {
        if (photonView.IsMine && isAlive == true && canMove == true)
        {

            float x = inputMove.x;
            float y = inputMove.y;
            Vector3 direction = new Vector3(x, 0f, y).normalized;
            //gravity
            if (noClip == false)
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
            }
            if (locked)
            {
                if (turnSens == 0)
                    updateSens();
                turn += inputTurn * turnSens;
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
    #endregion
    #region PlayerInput Events
    public void Roll()
    {
        if (ActCooldown <= 0 && !Rb.IsSleeping())
        {
            ActCooldown = dodgeCooldown;
            StartCoroutine(IFrames(.8f));
            StartCoroutine(StartRoll());
        }
    }
    public void QuickSlot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UsableItem quickItem = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Item as UsableItem;
            QuickSlot quickSlot = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true);
            if (quickItem != null)
            {
                if (quickItem.UseableCheck(PlayerUi.Instance))
                {
                    quickItem.Use(character);
                    quickSlot.Amount--;
                    PlayerUi.Instance.CheckAmount();
                }
            }
        }
    }
    public void Attack()
    {
        if (canAttack == true && !isRollExecuting)
        {
            StartCoroutine(AttackSet());
        }
    }
    public void Movement(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        inputMove = new Vector2(input.x, input.y);
    }
    public void LockTurn(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        inputTurn = new Vector2(input.x, input.y);
    }
    public void InventorySwitch(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        if (inChest == false && isAlive == true)
        {
            if (InvIsOpen == false)
            {
                UpdateMoving(false);
                UpdateRun(false);
                InvIsOpen = true;
                onInventoryOpen();
                CursorToggle(true);

            }
            else if (InvIsOpen == true)
            {
                onInventoryClose();
                CursorToggle(false);
                InvIsOpen = false;

            }
        }
    }
    public void ContainerSwitch(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        if (lootContainerManager.CanBeOpened() && inChest == false)
        {
            UpdateMoving(false);
            UpdateRun(false);
            lootContainerManager.OpenContiner(character);
            onInventoryOpen();
            inChest = true;
            playerInput.SwitchCurrentActionMap("Container");
            CursorToggle(true);
        }
        else if (lootContainerManager.CanBeClosed() && inChest == true)
        {
            lootContainerManager.CloseContiner(character);
            onInventoryClose();
            inChest = false;
            playerInput.SwitchCurrentActionMap("Player");
            CursorToggle(false);
        }
        else if (shop != null && !shop.isOpen)
        {
            UpdateMoving(false);
            UpdateRun(false);
            onInventoryOpen();
            shop.Open(character);
            playerInput.SwitchCurrentActionMap("Container");
            CursorToggle(true);
        }
        else if (shop != null && shop.isOpen)
        {
            shop.Close(character);
            onInventoryClose();
            playerInput.SwitchCurrentActionMap("Player");
            CursorToggle(false);
        }
    }
    public void Menu(InputAction.CallbackContext context)
    {
        if (photonView.IsMine)
        {
            if (!context.started)
                return;
            if (!escapeMenuOpen)
            {
                escapeMenu?.Invoke();
                CursorToggle(true);
                escapeMenuOpen = true;


            }
            else if (escapeMenuOpen)
            {
                escapeMenu?.Invoke();
                escapeMenuOpen = false;
                CursorToggle(false);
                updateSens();
            }
        }

    }
    public void Map(InputAction.CallbackContext context)
    {
        if (photonView.IsMine)
        {
            if (!context.started)
                return;
            if (!MapManager.Instance.mapOpen)
            {
                MapManager.Instance.MapChange();
                CursorToggle(true);
            }
            else if (MapManager.Instance.mapOpen)
            {
                MapManager.Instance.MapChange();
                CursorToggle(false);
            }
        }
    }
    public void Sprint(InputAction.CallbackContext context)
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
            playerInput.SwitchCurrentActionMap("Dead");
            CursorToggle(true);
        }
        else if (state == false)
        {
            playerInput.SwitchCurrentActionMap("Player");
            CursorToggle(false);
        }
    }

    void CursorToggle(bool state)
    {
        if (state == true)
        {
            Cursor.lockState = CursorLockMode.None;
            CrossToggle?.Invoke();
            if (playerInput.currentControlScheme == gamepadScheme)
                GamepadCursor.Instance.ToggleTracking(true);
        }
        else if (state == false)
        {
            CrossToggle?.Invoke();
            Cursor.lockState = CursorLockMode.Locked;
            GamepadCursor.Instance.ToggleTracking(false);
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
            StartCoroutine(IFrames(0.5f));
            Debug.Log(this + "takes " + damage + " damage.");
            if (photonView.IsMine)
            {
                if (gameObject.GetComponent<MeeleController>() != null)
                    OnDamaged(damage);
                CurrentHealth -= damage;
                if (showDmgNumber)
                {
                    var text = Instantiate(FloatingText, DmgTextspawn.position, Quaternion.Euler(0, 180, 0), DmgTextspawn);
                    text.GetComponent<TextMesh>().text = damage.ToString();

                }
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
            playerInput.SwitchCurrentActionMap("Dead");
            StartCoroutine(ExecuteAfterTime());
            Debug.Log(this + " died");
        }
    }
    public void Heal(int amount)
    {
        if (CurrentHealth != MaxHealth)
        {
            CurrentHealth += amount;
            if (showHealingNumber)
            {
                var text = Instantiate(FloatingText, HealTextspawn.position, Quaternion.identity, HealTextspawn);
                text.GetComponent<TextMesh>().color = Color.green;
                text.GetComponent<TextMesh>().text = amount.ToString();
            }
        }
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
            playerInput.SwitchCurrentActionMap("Player");
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
        float baseSpeed = (6 + Character.Instance.Agility.Value / 2);
        float modSpeed = baseSpeed + (baseSpeed * speedMod);
        speed = modSpeed;
        return speed;
    }

    public float CheckSprintSpeed()
    {
        speed = (12 + Character.Instance.Agility.Value / 2);
        SprintSpeed = speed;
        return SprintSpeed;
    }

    #endregion
    #region Misc
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }
#endif

    [PunRPC]
    public void PlaySFXRPC(SFX sFX)
    {
        sFX.PlaySFX();
    }

    void toggleHealNumber(bool state)
    {
        if (photonView.IsMine)
        {
            if (state)
                showHealingNumber = true;
            else if (!state)
                showHealingNumber = false;
        }
    }
    void toggleDmgNumber(bool state)
    {
        if (photonView.IsMine)
        {
            if (state)
                showDmgNumber = true;
            else if (!state)
                showDmgNumber = false;
        }
    }
    public void updateSens()
    {
        if (photonView.IsMine)
        {
            if (playerInput.currentControlScheme == "Keyboard")
                turnSens = PlayerPrefs.GetFloat(Keybaord_Sensativity, .3f);
            if (playerInput.currentControlScheme == "Controller")
                turnSens = PlayerPrefs.GetFloat(Gamepad_Sensativity, 3.85f);
        }
    }
    #endregion
}