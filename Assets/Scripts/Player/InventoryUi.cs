using Cinemachine;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviourPun
{
    public static InventoryUi Instance;
    private PlayerManger target;
    public TextMeshProUGUI timeText;
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
    }
    private void OnEnable()
    {
        PlayerManger.onInventoryOpen += OpenInv;
        PlayerManger.onInventoryClose += CloseInv;
        PlayerManger.OnDeath += UpdateHealth;
        MapManager.MapState += UpdateCanvas;
    }
    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= OpenInv;
        PlayerManger.onInventoryClose -= CloseInv;
        PlayerManger.OnDeath -= UpdateHealth;
        MapManager.MapState -= UpdateCanvas;
    }
    public void OpenInv()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void CloseInv()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void UpdateCanvas(bool state)
    {
        if (state == true)
            InvCanvas.enabled = false;
        if (state == false)
            InvCanvas.enabled = true;
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
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }
    private void Update()
    {
        if (GameManger.Instance.GameTimeLeft > 0)
        {
            updateTimer(GameManger.Instance.GameTimeLeft);
        }
        else
        {
            updateTimer(-1);
        }
    }
    void UpdateHealth(PlayerManger player)
    {
        int lifes = PlayerUi.Instance.target.lifes;
        if(lifes == 3)
            hearts[2].gameObject.SetActive(false);
        if (lifes == 2)
            hearts[1].gameObject.SetActive(false);
        if (lifes == 1)
            hearts[0].gameObject.SetActive(false);
    }
    private void updateTimer(float currentTime)
    {
        currentTime += 1;

        float min = Mathf.FloorToInt(currentTime / 60);
        float sec = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format("{0:00} : {1:00}", min, sec); 
    }
}

