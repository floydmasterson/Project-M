using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviourPun
{
    public static InventoryUi Instance;
    private PlayerManger target;
    [SerializeField] TextMeshProUGUI goldText;
    private int gold;
    public Image[] hearts;
    public Canvas InvCanvas;
    float characterControllerHeight = 0f;
    public bool isOpen = false;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    //Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        gold = 0;
        goldText.text = gold.ToString() + "G";
    }
    private void OnEnable()
    {
        PlayerManger.onInventoryOpen += OpenInv;
        PlayerManger.onInventoryClose += CloseInv;
        PlayerManger.OnDeath += UpdateLifes;
    }
    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= OpenInv;
        PlayerManger.onInventoryClose -= CloseInv;
        PlayerManger.OnDeath -= UpdateLifes;
    }
    public void OpenInv()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void CloseInv()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

    }
    public void SetTargetI(PlayerManger _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayManager target for PlayerUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        target = _target;
        targetTransform = target.GetComponent<Transform>();
        targetRenderer = target.GetComponentInChildren<Renderer>();

        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }
    void UpdateLifes(PlayerManger player)
    {
        int lifes = PlayerUi.Instance.target.lifes;
        if (lifes == 3)
            hearts[2].gameObject.SetActive(false);
        if (lifes == 2)
            hearts[1].gameObject.SetActive(false);
        if (lifes == 1)
            hearts[0].gameObject.SetActive(false);
    }
    public void UpdateGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString() + "G";
    }

    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }
    public int CheckGold()
    {

        return gold;
    }

    
}

