using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RemoveAllBindings : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAction;

    public void ResetAllBindings()
    {
        foreach(InputActionMap map in inputAction.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}
