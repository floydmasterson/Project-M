using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
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
    [TabGroup("Quick Slot")]
    Image QSlotCooldown;
    [TabGroup("Quick Slot")]
    TextMeshProUGUI QSlotCooldownText;
    [TabGroup("Quick Slot")]
    Image UiSlotCooldown;
    [TabGroup("Quick Slot")]
    Image UiCooldownIcon;

    private float cooldownTime;
    private float cooldownTimer = 0f;

    [TabGroup("MiniMap")]
    public GameObject Minimap;
    [TabGroup("MiniMap")]
    public Canvas UiCanvas;
    [TabGroup("Escape Menu"), SerializeField, Required]
    EscapeMenu escapeMenu;
    [TabGroup("Escape Menu"), SerializeField, Required]
    sensitivityControler[] controllers;
    [HideInInspector]
    public PlayerInput playerInput;
    int quickAmount;
    private MagicController MagicController;
    private MeeleController MeleeController;
    public static PlayerUi Instance;
    [Tooltip("Set at runtime")]
    public PlayerManger target;
    public delegate void TargetSet();
    public static event TargetSet OnTargetSet;

    GameObject Health;
    GameObject Mana;
    GameObject Rage;
    GameObject RageInv;
    GameObject ManaInv;
    GameObject HealthInv;

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;
    private bool toggle = true;
    GamepadCursor gamepadCursor;
    public bool Sick = false;
    IEnumerator ApplyPotionSickness(float time)
    {
        Sick = true;
        yield return new WaitForSeconds(time);
        Sick = false;
    }
    IEnumerator applyCooldown(float time)
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer < 0f)
        {
            QSlotCooldownText.gameObject.SetActive(false);
            UiCooldownIcon.gameObject.SetActive(false);
            QSlotCooldown.fillAmount = 0f;
            UiSlotCooldown.fillAmount = 0f;
            cooldownTime = 0f;
        }
        else
        {
            QSlotCooldownText.text = Mathf.RoundToInt(cooldownTimer).ToString();
            QSlotCooldown.fillAmount = cooldownTimer / cooldownTime;
            UiSlotCooldown.fillAmount = cooldownTimer / cooldownTime;
            yield return new WaitForEndOfFrame();
            StartCoroutine(applyCooldown(cooldownTimer));
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        _canvasGroup = this.GetComponent<CanvasGroup>();
        gamepadCursor = GetComponentInChildren<GamepadCursor>();
        Health = gameObject.transform.GetChild(2).gameObject;
        Mana = gameObject.transform.GetChild(3).gameObject;
        Rage = gameObject.transform.GetChild(4).gameObject;
        RageInv = gameObject.transform.GetChild(5).gameObject;
        ManaInv = gameObject.transform.GetChild(6).gameObject;
        HealthInv = gameObject.transform.GetChild(7).gameObject;
        QSlotCooldown = quickSlot.transform.GetChild(3).GetComponent<Image>();
        QSlotCooldownText = quickSlot.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        UiCooldownIcon = Health.transform.GetChild(4).GetComponent<Image>();
        UiSlotCooldown = UiCooldownIcon.gameObject.transform.GetChild(0).GetComponent<Image>();
        QSlotCooldown.fillAmount = 0f;
        UiSlotCooldown.fillAmount = 0f;
        QSlotCooldownText.gameObject.SetActive(false);
        UiCooldownIcon.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        UseableItemEffect.PotionSick += PotionSickness;
        PlayerManger.onInventoryOpen += CloseUi;
        PlayerManger.onInventoryClose += OpenUi;
        PlayerManger.escapeMenu += toggleUi;
    }

    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= CloseUi;
        PlayerManger.onInventoryClose -= OpenUi;
        PlayerManger.escapeMenu -= toggleUi;
        UseableItemEffect.PotionSick -= PotionSickness;
    }
    private void Start()
    {
        MagicController = target.gameObject.GetComponent<MagicController>();
        MeleeController = target.gameObject.GetComponent<MeeleController>();

        if (MagicController != null)
        {
            Mana.SetActive(true);
        }
        if (MeleeController != null)
        {
            Rage.SetActive(true);
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
    private void CooldownGFX(float time)
    {
        cooldownTime = time;
        cooldownTimer = cooldownTime;
        CheckAmount();
        QSlotCooldownText.gameObject.SetActive(true);
        UiCooldownIcon.gameObject.SetActive(true);
        StartCoroutine(applyCooldown(time));
    }
    private void PotionSickness(float time)
    {
        StartCoroutine("ApplyPotionSickness", time);
        CooldownGFX(time);
    }
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
        {
            quickSlotImage.color = new Color(1, 1, 1, 1);
            quickSlotImage.sprite = Character.Instance.currentQuickItem.sprite;
        }
        else
        {
            quickSlotImage.color = new Color(1, 1, 1, 0);
            if (!quickSlot.activeInHierarchy && Health.activeInHierarchy)
                quickSlot.SetActive(true);
        }

    }
    void LateUpdate()
    {
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
        Health.SetActive(true);
        HealthInv.SetActive(false);
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

            Mana.SetActive(true);
            ManaInv.SetActive(false);
        }
        if (MeleeController != null)
        {
            Rage.SetActive(true);
            RageInv.SetActive(false);
        }
    }
    public void CloseUi()
    {
        quickSlot.SetActive(false);
        Health.SetActive(false);
        HealthInv.SetActive(true);
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
            Mana.SetActive(false);
            ManaInv.SetActive(true);
        }
        if (MeleeController != null)
        {
            Rage.SetActive(false);
            RageInv.SetActive(true);
        }
    }
    void toggleUi()
    {
        if (toggle)
        {
            for (int i = 2; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            escapeMenu.toggleMenu();
            toggle = false;
        }
        else if (!toggle)
        {
            escapeMenu.toggleMenu();
            OpenUi();
            toggle = true;
        }
    }
    public void CheckAmount()
    {
        quickAmount = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Amount;
        if (quickAmount > 1)
        {
            quickSlotAmountText.gameObject.SetActive(true);
            quickSlotAmountText.text = quickAmount.ToString();

        }
        else if(quickAmount <= 1)
        {
            quickSlotAmountText.gameObject.SetActive(false);
        }
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
        foreach (sensitivityControler controler in controllers)
        {
            controler.playerInput = playerInput;
        }
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
