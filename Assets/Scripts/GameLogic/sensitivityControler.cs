using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class sensitivityControler : MonoBehaviour
{
    [SerializeField]
    [EnumToggleButtons]
    Scheme scaleMode;
    [SerializeField]
    [EnumToggleButtons]
    Scale scale;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;
    [HideIf("@scale == Scale.Unlocked || scale == Scale.Cursor")]
    [SerializeField] string actionName;
    public PlayerInput playerInput;


    enum Scheme
    {
        Keyboard,
        Gamepad,
    }
    enum Scale
    {
        Unlocked,
        Locked,
        Cursor,
    }
    private void OnDisable()
    {
        PlayerPrefs.SetInt(gameObject.name, (int)slider.value);
    }
    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChange);
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey(gameObject.name))
            slider.value = PlayerPrefs.GetInt(gameObject.name, (int)slider.value);
        else if (!PlayerPrefs.HasKey(gameObject.name))
        {
            PlayerPrefs.SetInt(gameObject.name, 7);
            slider.value = 7f;
        }
    }
    void HandleSliderValueChange(float value)
    {

        if (scale == Scale.Locked)
        {
            if (scaleMode == Scheme.Keyboard)
            {
                playerInput.actions.FindAction(actionName).ApplyParameterOverride("scaleVector2:x", value * .3f / 7.2f);
                Debug.Log("Locked Key Set to " + value * .3f / 7.2f);
                valueText.text = "Locked Camera: " + value.ToString();

            }
            else if (scaleMode == Scheme.Gamepad)
            {
                playerInput.actions.FindAction(actionName).ApplyParameterOverride("scaleVector2:x", value * 1.1f / 2f);
                Debug.Log("Locked Key Set to " + value * 1.1f / 2f);
                valueText.text = "Locked Camera: " + value.ToString();
            }
        }
        else if (scale == Scale.Unlocked)
        {
            PlayerManger player = PlayerUi.Instance.target;
            switch (value)
            {
                case 1:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 100;
                    player.currentCameraSpeed = 100;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 2:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 125;
                    player.currentCameraSpeed = 125;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 3:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 150;
                    player.currentCameraSpeed = 150;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 4:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 175;
                    player.currentCameraSpeed = 175;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 5:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 200;
                    player.currentCameraSpeed = 200;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 6:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 225;
                    player.currentCameraSpeed = 225;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 7:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 250;
                    player.currentCameraSpeed = 250;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 8:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 275;
                    player.currentCameraSpeed = 275;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 9:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 300;
                    player.currentCameraSpeed = 300;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 10:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 325;
                    player.currentCameraSpeed = 325;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 11:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 350;
                    player.currentCameraSpeed = 350;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 12:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 375;
                    player.currentCameraSpeed = 375;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 13:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 400;
                    player.currentCameraSpeed = 400;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;
                case 14:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 425;
                    player.currentCameraSpeed = 425;
                    valueText.text = "Unlocked Camera: " + value.ToString();
                    break;

                default:
                    player.cineCamera.m_XAxis.m_MaxSpeed = 250;
                    player.currentCameraSpeed = 250;
                    valueText.text = "Unlocked Camera: 7";
                    break;
            }
        }
        else if (scale == Scale.Cursor)
        {
            GamepadCursor cursor = GamepadCursor.Instance;
            switch (value)
            {
                case 1:
                    cursor.cursorSpeed = 750;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 2:
                    cursor.cursorSpeed = 800;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 3:
                    cursor.cursorSpeed = 950;
                    valueText.text = "Cursor Speed:: " + value.ToString();
                    break;
                case 4:
                    cursor.cursorSpeed = 1000;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 5:
                    cursor.cursorSpeed = 1100;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 6:
                    cursor.cursorSpeed = 1200;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 7:
                    cursor.cursorSpeed = 1300;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 8:
                    cursor.cursorSpeed = 1400;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 9:
                    cursor.cursorSpeed = 1500;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 10:
                    cursor.cursorSpeed = 1600;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 11:
                    cursor.cursorSpeed = 1700;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 12:
                    cursor.cursorSpeed = 1800;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 13:
                    cursor.cursorSpeed = 1900;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;
                case 14:
                    cursor.cursorSpeed = 2000;
                    valueText.text = "Cursor Speed: " + value.ToString();
                    break;

                default:
                    cursor.cursorSpeed = 1150;
                    valueText.text = "Cursor Speed: 7";
                    break;
            }
        }
    }
}

