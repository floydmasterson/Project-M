using Photon.Pun;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviourPun
{
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    [TitleGroup("Health & Resource")]
    [HorizontalGroup("Health & Resource/Split")]
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField]
    private Slider playerHealthSlider;
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField]
    private Slider playerHealthSliderInv;
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField]
    private TextMeshProUGUI HealthText;
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField]
    private TextMeshProUGUI HealthTextInv;

    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField]
    private Slider playerManaSlider;
    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField]
    private Slider playerManaSliderInv;
    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField]
    private TextMeshProUGUI ManaText;
    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField]
    private TextMeshProUGUI ManaTextInv;

    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField]
    public Slider playerRageSlider;
    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField]
    public Slider playerRageSliderInv;
    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField]
    private TextMeshProUGUI RageText;
    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField]
    private TextMeshProUGUI RageTextInv;

    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    public Slider playerArrowSlider;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    public Slider playerArrowSliderInv;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    public Slider playerArrowPullSlider;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    private TextMeshProUGUI ArrowText;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    private TextMeshProUGUI ArrowTextInv;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    private Image ArrowReload;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField]
    private Image ArrowPullFill;

    [TitleGroup("HUD")]
    [HorizontalGroup("HUD/Split")]
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private Image quickSlotImage;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private TextMeshProUGUI quickSlotAmountText;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private Image QSlotCooldown;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private TextMeshProUGUI QSlotCooldownText;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private TextMeshProUGUI SecondaryCooldownText;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private Image SecondaryCooldown;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private Image SecondaryCooldownIcon;


    [TabGroup("HUD/Split/Player", "Minimap")]
    public GameObject Minimap;
    [TabGroup("HUD/Split/Player", "Minimap")]
    public Canvas UiCanvas;
    [TabGroup("HUD/Split/Player", "Timers"), SerializeField, Required]
    private TextMeshProUGUI timeText;
    [TabGroup("HUD/Split/Player", "Timers"), SerializeField, Required]
    private TextMeshProUGUI timeTextInv;
    [TitleGroup("Menus")]
    [HorizontalGroup("Menus/Split")]
    [TabGroup("Menus/Split/Player", "Escape Menu"), SerializeField, Required]
    private EscapeMenu escapeMenu;
    [TabGroup("Menus/Split/Player", "Escape Menu"), SerializeField, Required]
    private sensitivityControler[] controllers;
    [TabGroup("Menus/Split/Player", "Loading Menu"), SerializeField, Required]
    private Image loadingShield;
    [TabGroup("Menus/Split/Player", "Loading Menu"), SerializeField, Required]
    private TextMeshProUGUI toShop;


    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private GameObject quickSlot;
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField, Required]
    private GameObject Health;
    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField, Required]
    private GameObject Mana;
    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField, Required]
    private GameObject Rage;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField, Required]
    private GameObject Arrow;
    [TabGroup("HUD/Split/Player", "Timers"), SerializeField, Required]
    private GameObject Timer;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField, Required]
    private GameObject ArrowInv;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField, Required]
    private GameObject ArrowEquiped;
    [TabGroup("HUD/Split/Player", "Timers"), SerializeField, Required]
    private GameObject TimerInv;
    [TabGroup("Health & Resource/Split/Player", "Rage"), SerializeField, Required]
    private GameObject RageInv;
    [TabGroup("Health & Resource/Split/Player", "Mana"), SerializeField, Required]
    private GameObject ManaInv;
    [TabGroup("Health & Resource/Split/Player", "Health"), SerializeField, Required]
    private GameObject HealthInv;
    [TabGroup("HUD/Split/Player", "Quick Slot"), SerializeField, Required]
    private GameObject Secondary;
    [TabGroup("Health & Resource/Split/Player", "Arrow"), SerializeField, Required]
    private GameObject ArrowPull;

    [HideInInspector]
    public PlayerInput playerInput;

    private int quickAmount;
    private MagicController MagicController;
    private MeeleController MeleeController;
    private ArrowController ArrowController;
    public static PlayerUi Instance;

    public delegate void TargetSet();
    public static event TargetSet OnTargetSet;

    private float characterControllerHeight = 0f;
    private Transform targetTransform;
    private Vector3 targetPosition;
    private bool toggle = true;
    private GamepadCursor gamepadCursor;
    private float elaspedTime;
    private float reloadTime;



    [Tooltip("Set at runtime")]
    public PlayerManger target;
    public bool Sick = false;
    private IEnumerator ApplyPotionSickness(float time)
    {
        Sick = true;
        yield return new WaitForSeconds(time);
        Sick = false;
    }

    private IEnumerator Fadeout(float time, bool showShopText)
    {
        if (showShopText)
        {
            toShop.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
            loadingShield.CrossFadeAlpha(255, time, false);
        }
        yield return new WaitForSecondsRealtime(.5f);
        loadingShield.CrossFadeAlpha(0, time, false);
        yield return new WaitForSecondsRealtime(time);
        toShop.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        gamepadCursor = GetComponentInChildren<GamepadCursor>();
        QSlotCooldown.fillAmount = 0f;
        SecondaryCooldown.fillAmount = 0f;
        SecondaryCooldownText.gameObject.SetActive(false);
        QSlotCooldownText.gameObject.SetActive(false);
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
        ArrowController = target.gameObject.GetComponent<ArrowController>();

        if (MagicController != null)
        {
            Mana.SetActive(true);
            SecondaryCooldownIcon.sprite = MagicController.manaPulseImage;
        }
        if (MeleeController != null)
        {
            Rage.SetActive(true);
            SecondaryCooldownIcon.sprite = MeleeController.rageAuraImage;
        }
        if (ArrowController != null)
        {
            Arrow.SetActive(true);
            ArrowPull.SetActive(true);
            SecondaryCooldownIcon.sprite = ArrowController.bombArrowIcon;
        }
        if (InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Item != null)
        {
            quickSlotImage.sprite = target.character.currentQuickItem.sprite;
            if (quickAmount > 0)
            {
                quickSlot.transform.GetChild(2).gameObject.SetActive(true);
                quickSlotAmountText.text = quickAmount.ToString();
            }
        }
    }
    // Update is called once per frame

    private void PotionSickness(float time)
    {
        StartCoroutine("ApplyPotionSickness", time);
        CooldownManager.CooldownGFX(QSlotCooldown, QSlotCooldownText, time, this);
    }
    public void SecondaryCooldownGFX(float time)
    {
        CooldownManager.CooldownGFX(SecondaryCooldown, SecondaryCooldownText, time, this);
    }
    public void SecondaryEquipped(bool state)
    {
        if (state)
        {
            ArrowEquiped.SetActive(true);

        }
        else if (!state)
        {
            ArrowEquiped.SetActive(false);
        }
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
        if (ArrowController != null)
        {
            if (playerArrowSlider != null)
            {
                playerArrowSlider.maxValue = ArrowController.MaxArrows;
                playerArrowSlider.value = ArrowController.CurrArrows;
                ArrowText.text = (Mathf.RoundToInt(ArrowController.CurrArrows) + "/" + ArrowController.MaxArrows);
            }
            if (playerArrowSliderInv != null)
            {
                playerArrowSliderInv.maxValue = ArrowController.MaxArrows;
                playerArrowSliderInv.value = ArrowController.CurrArrows;
                ArrowTextInv.text = (Mathf.RoundToInt(ArrowController.CurrArrows) + "/" + ArrowController.MaxArrows);
            }
            if (playerArrowPullSlider != null)
            {
                if (ArrowController.isDrawing)
                {
                    elaspedTime += Time.deltaTime;
                    float percentComplete = elaspedTime / ArrowController.DrawTime;
                    playerArrowPullSlider.value = Mathf.Lerp(0, 1, percentComplete);
                    if (ArrowController.readyToFire())
                    {
                        ArrowPullFill.color = Color.yellow;
                    }
                }
                else if (!ArrowController.isDrawing)
                {
                    playerArrowPullSlider.value = 0;
                    elaspedTime = 0;
                    ArrowPullFill.color = Color.white;
                }
                


            }
            if (ArrowReload != null)
            {
                if (ArrowController.isReloading)
                {
                    reloadTime += Time.deltaTime;
                    if (reloadTime <= ArrowController.timeBetweenReload)
                        ArrowReload.fillAmount = reloadTime / ArrowController.timeBetweenReload;
                    else if (reloadTime >= ArrowController.timeBetweenReload)
                    {
                        reloadTime = 0;
                        ArrowReload.fillAmount = 0;
                    }
                }
                else if (!ArrowController.isReloading)
                {
                    ArrowReload.fillAmount = 0;
                    reloadTime = 0;
                }
            }
        }
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        if (target.character.currentQuickItem.sprite != null)
        {
            quickSlotImage.color = new Color(1, 1, 1, 1);
            quickSlotImage.sprite = target.character.currentQuickItem.sprite;
        }
        else
        {
            quickSlotImage.color = new Color(1, 1, 1, 0);
            if (!quickSlot.activeInHierarchy && Health.activeInHierarchy)
                quickSlot.SetActive(true);
        }
        if (GameManger.Instance != null && GameManger.Instance.GameTimeLeft > 0)
        {
            updateTimer(GameManger.Instance.GameTimeLeft);
        }
        else
        {
            updateTimer(-1);
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
        Minimap.SetActive(true);
        Timer.SetActive(true);
        Secondary.SetActive(true);
        HealthInv.SetActive(false);
        TimerInv.SetActive(false);
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
        if (ArrowController != null)
        {
            Arrow.SetActive(true);
            ArrowInv.SetActive(false);
        }
    }
    public void CloseUi()
    {
        quickSlot.SetActive(false);
        Health.SetActive(false);
        Minimap.SetActive(false);
        Timer.SetActive(false);
        Secondary.SetActive(false);
        HealthInv.SetActive(true);
        TimerInv.SetActive(true);
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
        if (ArrowController != null)
        {
            Arrow.SetActive(false);
            ArrowInv.SetActive(true);
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
        else if (quickAmount <= 1)
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
        targetTransform = target.GetComponent<Transform>();
        playerInput = target.GetComponent<PlayerInput>();
        foreach (sensitivityControler controler in controllers)
        {
            controler.playerInput = playerInput;
        }
        gamepadCursor.playerInput = playerInput;
        OnTargetSet?.Invoke();
        gamepadCursor.player = target;
        gamepadCursor.enabled = true;
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
        StartCoroutine(Fadeout(.6f, false));

    }
    public void ShopTime()
    {
        StartCoroutine(Fadeout(2f, true));
    }
    private void updateTimer(float currentTime)
    {
        currentTime += 1;

        float min = Mathf.FloorToInt(currentTime / 60);
        float sec = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format("{0:00} : {1:00}", min, sec);
        timeTextInv.text = string.Format("{0:00} : {1:00}", min, sec);
    }
}
