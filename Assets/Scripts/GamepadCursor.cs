using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.XR;

public class GamepadCursor : MonoBehaviour
{
    public static GamepadCursor Instance;
    public PlayerInput playerInput;
    [SerializeField] public RectTransform cursorTransfom;
    [SerializeField] float cursorSpeed = 1000f;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Canvas canvas;
    [SerializeField] float padding = 50f;

    bool prevoiusMouseState;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera mainCamera;
    string previousControlScheme = string.Empty;
    const string gamepadScheme = "Controller";
    const string mouseScheme = "Keyboard";

    bool TrackingControls;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;
        if (virtualMouse == null)
            virtualMouse = (Mouse)InputSystem.AddDevice("virtualMouse");
        else if (!virtualMouse.added)
            InputSystem.AddDevice(virtualMouse);
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransfom != null)
        {
            Vector2 position = cursorTransfom.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;

    }
    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;

    
        if (virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);
    }
    void onControlsChange(PlayerInput input)
    {
        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;

        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCourser(currentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }
    private void Update()
    {
        if (virtualMouse.added && playerInput != null && !TrackingControls)
        {
            if (previousControlScheme != playerInput.currentControlScheme)
            {
                onControlsChange(null);
                previousControlScheme = playerInput.currentControlScheme;

            }
        }
    }
    private void UpdateMotion()
    {
        if (cursorTransfom.gameObject.activeInHierarchy)
        {
            if (virtualMouse == null || Gamepad.current == null) return;

            Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
            deltaValue *= cursorSpeed * Time.deltaTime;

            Vector2 currentPostion = virtualMouse.position.ReadValue();
            Vector2 newPositon = currentPostion + deltaValue;

            newPositon.x = Mathf.Clamp(newPositon.x, padding, Screen.width - padding);
            newPositon.y = Mathf.Clamp(newPositon.y, padding, Screen.height - padding);

            InputState.Change(virtualMouse.position, newPositon);
            InputState.Change(virtualMouse.delta, deltaValue);

            bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
            if (prevoiusMouseState != Gamepad.current.aButton.isPressed)
            {
                virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Right, aButtonIsPressed);
                InputState.Change(virtualMouse, mouseState);
                prevoiusMouseState = aButtonIsPressed;
            }

            AnchorCourser(newPositon);
        }
    }
    private void AnchorCourser(Vector2 position)
    {
        Vector2 anchorPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchorPosition);
        cursorTransfom.anchoredPosition = anchorPosition;
    }

    public void ToggleTracking(bool state)
    {
        if (state == true) 
        {
            TrackingControls = true;
            cursorTransfom.gameObject.SetActive(true);
        }
        else if(state == false)
        {
            TrackingControls = false;
            cursorTransfom.gameObject.SetActive(false);
        }
    }
}
