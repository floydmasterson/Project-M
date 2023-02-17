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
    private const string Keybaord_Sensativity = "KS";
    private const string Gamepad_Sensativity = "GS";


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
            slider.value = PlayerPrefs.GetInt(gameObject.name, 7);    
    }
    
    public void HandleSliderValueChange(float value)
    {

        if (scale == Scale.Locked)
        {
            if (scaleMode == Scheme.Keyboard)
            {
                    PlayerPrefs.SetFloat(Keybaord_Sensativity, value * .15f / 3.4f);            
                valueText.text = "Locked Camera: " + value.ToString();

            }
            else if (scaleMode == Scheme.Gamepad)
            {
                PlayerPrefs.SetFloat(Gamepad_Sensativity, value * 1.1f / 2f);
                valueText.text = "Locked Camera: " + value.ToString();
            }
        }
        else if (scale == Scale.Cursor)
        {
            GamepadCursor cursor = GamepadCursor.Instance;
            if (cursor != null)
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

