using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviourPun
{
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    [TabGroup("Player Health")]
    [SerializeField] private Slider playerHealthSlider;
    [TabGroup("Player Health")]
    [SerializeField] private Slider playerHealthSliderInv;
    [TabGroup("Player Health")]
    [SerializeField] private TextMeshProUGUI HealthText;
    [TabGroup("Player Health")]
    [SerializeField] private TextMeshProUGUI HealthTextInv;

    [TabGroup("Player Mana")]
    [SerializeField] Slider playerManaSlider;
    [TabGroup("Player Mana")]
    [SerializeField] Slider playerManaSliderInv;
    [TabGroup("Player Mana")]
    [SerializeField] private TextMeshProUGUI ManaText;
    [TabGroup("Player Mana")]
    [SerializeField] private TextMeshProUGUI ManaTextInv;

    [TabGroup("Player Rage")]
    [SerializeField] public Slider playerRageSlider;
    [TabGroup("Player Rage")]
    [SerializeField] public Slider playerRageSliderInv;
    [TabGroup("Player Rage")]
    [SerializeField] private TextMeshProUGUI RageText;
    [TabGroup("Player Rage")]
    [SerializeField] private TextMeshProUGUI RageTextInv;


    [TabGroup("Quick Slot")]
    [SerializeField] Image quickSlotImage;
    [TabGroup("Quick Slot")]
    [SerializeField] GameObject quickSlot;
    [TabGroup("Quick Slot")]
    [SerializeField] TextMeshProUGUI quickSlotAmountText;
    [TabGroup("MiniMap")]
    public GameObject Minimap;
    [TabGroup("MiniMap")]
    public Canvas UiCanvas;
    [HideInInspector]
    public PlayerInput playerInput;
    int quickAmount;
    private MagicController MagicController;
    private MeeleController MeleeController;
    public static PlayerUi Instance;
    public PlayerManger target;
    public delegate void TargetSet();
    public static event TargetSet OnTargetSet;

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;
    private bool toggle = true;
    GamepadCursor gamepadCursor;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        _canvasGroup = this.GetComponent<CanvasGroup>();
        gamepadCursor = GetComponentInChildren<GamepadCursor>();
    }
    private void OnEnable()
    {
        PlayerManger.onInventoryOpen += CloseUi;
        PlayerManger.onInventoryClose += OpenUi;
        PlayerManger.escapeMenu += toggleUi;
        MapManager.MapState += ctx => toggleUi();

    }
    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= CloseUi;
        PlayerManger.onInventoryClose -= OpenUi;
        PlayerManger.escapeMenu -= toggleUi;
        MapManager.MapState -= ctx => toggleUi();
    }
    private void Start()
    {
        MagicController = target.gameObject.GetComponent<MagicController>();
        MeleeController = target.gameObject.GetComponent<MeeleController>();
        if (MagicController != null)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
        if (MeleeController != null)
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(true);
        }
        if (InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Item != null)
        {
            quickSlotImage.sprite = Character.Instance.currentQuickItem.sprite;
            if (quickAmount > 0)
            {
                quickSlot.transform.GetChild(2).gameObject.SetActive(true);
                quickSlotAmountText.text = quickAmount.ToString();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = target.MaxHealth;
            playerHealthSlider.value = target.CurrentHealth;
            HealthText.text = (target.CurrentHealth + "/" + target.MaxHealth);
        }
        if (playerHealthSliderInv != null)
        {
            playerHealthSliderInv.maxValue = target.MaxHealth;
            playerHealthSliderInv.value = target.CurrentHealth;
            HealthTextInv.text = (target.CurrentHealth + "/" + target.MaxHealth);
        }
        if (MagicController != null)
        {
            if (playerManaSlider != null)
            {
                playerManaSlider.maxValue = MagicController.MaxMana;
                playerManaSlider.value = MagicController.CurrMana;
                ManaText.text = (Mathf.RoundToInt(MagicController.CurrMana) + "/" + MagicController.MaxMana);
            }
            if (playerHealthSliderInv != null)
            {
                playerManaSliderInv.maxValue = MagicController.MaxMana;
                playerManaSliderInv.value = MagicController.CurrMana;
                ManaTextInv.text = (Mathf.RoundToInt(MagicController.CurrMana) + "/" + MagicController.MaxMana);
            }
        }
        if (MeleeController != null)
        {
            if (playerRageSlider != null)
            {
                playerRageSlider.maxValue = MeleeController.MaxRage;
                playerRageSlider.value = MeleeController.CurrentRage;
                RageText.text = (Mathf.RoundToInt(MeleeController.CurrentRage) + "/" + MeleeController.MaxRage);
            }
            if (playerRageSliderInv != null)
            {
                playerRageSliderInv.maxValue = MeleeController.MaxRage;
                playerRageSliderInv.value = MeleeController.CurrentRage;
                RageTextInv.text = (Mathf.RoundToInt(MeleeController.CurrentRage) + "/" + MeleeController.MaxRage);
            }
        }

        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        if (Character.Instance.currentQuickItem.sprite != null)
            quickSlotImage.sprite = Character.Instance.currentQuickItem.sprite;
        else
            quickSlot.SetActive(false);

    }
    void LateUpdate()
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
        }
    }

    public void OpenUi()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(6).gameObject.SetActive(false);
        Minimap.SetActive(true);
        if (quickSlotImage.sprite != null)
        {
            quickSlot.SetActive(true);
            CheckAmount();
            if (quickAmount > 0)
            {
                quickSlot.transform.GetChild(2).gameObject.SetActive(true);
                CheckAmount();
            }
        }
        if (MagicController != null)
        {

            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
        }
        if (MeleeController != null)
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).gameObject.SetActive(false);
        }
    }
    public void CloseUi()
    {
        quickSlot.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(6).gameObject.SetActive(true);
        Minimap.SetActive(false);
        if (quickSlotImage.sprite != null)
        {
            quickSlot.SetActive(false);
            if (quickAmount > 0)
            {
                quickSlot.transform.GetChild(2).gameObject.SetActive(false);
                CheckAmount();
            }
        }
        if (MagicController != null)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
        }
        if (MeleeController != null)
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).gameObject.SetActive(true);
        }
    }
    void toggleUi()
    {
        if (toggle)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            toggle = false;
        }
        else if (!toggle)
        {
            OpenUi();
            toggle = true;
        }
    }

    public void CheckAmount()
    {
        quickAmount = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Amount;
        quickSlotAmountText.text = quickAmount.ToString();
    }
    public void SetTarget(PlayerManger _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();
        playerInput = target.GetComponent<PlayerInput>();
        gamepadCursor.playerInput = playerInput;
        OnTargetSet();
        gamepadCursor.player = target;
        gamepadCursor.enabled = true;
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }

    }
}
