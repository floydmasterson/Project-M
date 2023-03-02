using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoundKeyText : MonoBehaviour
{
    [SerializeField, TabGroup("Setup")]
    private string inputActionName;
    [SerializeField, TabGroup("Setup")]
    private TextMeshProUGUI textToChange;
    [SerializeField, TabGroup("Setup")]
    private string textToBeDisplayed1;
    [SerializeField, TabGroup("Setup")]
    private string textToBeDisplayed2;
    [SerializeField, TabGroup("Setup")]
    private InputActionAsset inputActionAsset;
    [SerializeField, TabGroup("Setup")]
    private string actionMapName;



    private void Update()
    {
        InputActionMap actionMap = inputActionAsset.FindActionMap(actionMapName);
        InputAction inputAction = actionMap.FindAction(inputActionName);
        string bindingDisplayString = inputAction.GetBindingDisplayString(0);
        textToChange.text = textToBeDisplayed1 + bindingDisplayString.ToUpper() + textToBeDisplayed2;
    }
}