using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviourPun
{
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider playerHealthSlider2;
    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private TextMeshProUGUI HealthText2;
    [Space]
    [SerializeField] Slider playerManaSlider;
    [SerializeField] Slider playerManaSlider2;
    [SerializeField] private TextMeshProUGUI ManaText;
    [SerializeField] private TextMeshProUGUI ManaText2;
    [Space]
    [SerializeField] Image quickSlotImage;
    [SerializeField] GameObject qickSlot;
    [SerializeField] TextMeshProUGUI quickSlotAmountText;
    int quickAmount;
    private MagicController MC;
    public static PlayerUi Instance;
    public PlayerManger target;

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;







    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        _canvasGroup = this.GetComponent<CanvasGroup>();

    }
    private void OnEnable()
    {
        PlayerManger.onInventoryOpen += CloseUi;
        PlayerManger.onInventoryClose += OpenUi;
    }
    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= CloseUi;
        PlayerManger.onInventoryClose -= OpenUi;
    }
    private void Start()
    {
        MC = target.gameObject.GetComponent<MagicController>();
        if (MC != null)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Item != null)
        {
            quickSlotImage.sprite = Character.Instance.currentQuickItem.sprite;
            if (quickAmount > 0)
            {
                qickSlot.transform.GetChild(2).gameObject.SetActive(true);
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
        if (playerHealthSlider2 != null)
        {
            playerHealthSlider2.maxValue = target.MaxHealth;
            playerHealthSlider2.value = target.CurrentHealth;
            HealthText2.text = (target.CurrentHealth + "/" + target.MaxHealth);
        }
        if (MC != null)
        {
            if (playerManaSlider != null)
            {
                playerManaSlider.maxValue = MC.MaxMana;
                playerManaSlider.value = MC.CurrMana;
                ManaText.text = (Mathf.RoundToInt(MC.CurrMana) + "/" + MC.MaxMana);
            }
            if (playerHealthSlider2 != null)
            {
                playerManaSlider2.maxValue = MC.MaxMana;
                playerManaSlider2.value = MC.CurrMana;
                ManaText2.text = (Mathf.RoundToInt(MC.CurrMana) + "/" + MC.MaxMana);
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
            qickSlot.SetActive(false);

    }

    public void OpenUi()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
        if (quickSlotImage.sprite != null)
        {
            qickSlot.SetActive(true);
            CheckAmount();
            if (quickAmount > 0)
            {
                qickSlot.transform.GetChild(2).gameObject.SetActive(true);
                CheckAmount();
            }
        }
        if (MC != null)
        {

            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    public void CloseUi()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(true);
        if (quickSlotImage.sprite != null)
        {
            qickSlot.SetActive(false);
            if (quickAmount > 0)
            {
                qickSlot.transform.GetChild(2).gameObject.SetActive(false);
                CheckAmount();
            }
        }
        if (MC != null)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
    public void CheckAmount()
    {
        quickAmount = InventoryUi.Instance.GetComponentInChildren<QuickSlot>(true).Amount;
        quickSlotAmountText.text = quickAmount.ToString();
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
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }
}
