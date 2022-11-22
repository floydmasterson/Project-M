using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class InventoryUi : MonoBehaviourPun
{
    public static InventoryUi Instance;
    private PlayerManger target;
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
    }
    private void OnDisable()
    {
        PlayerManger.onInventoryOpen -= OpenInv;
        PlayerManger.onInventoryClose -= CloseInv;
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

