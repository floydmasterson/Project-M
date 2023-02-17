using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    private bool open = false;
    [SerializeField]
    private GameObject Keyboard;
    [SerializeField]
    private GameObject gamepad;
    [SerializeField]
    private GameObject settings;
    [SerializeField]
    private GameObject buttons;
    [SerializeField]
    private GameObject controls;

    [SerializeField]
    TextMeshProUGUI schemeButton;
    private bool toggle = false;
    IEnumerator PreLoad()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        gamepad.SetActive(false);
        settings.SetActive(false);
        controls.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerUi.OnTargetSet += preLoad;
    }
    private void OnDisable()
    {
        PlayerUi.OnTargetSet -= preLoad;
    }
    void preLoad()
    {
        StartCoroutine(PreLoad());
    }
    public void toggleMenu()
    {
        if (!open)
        {
            buttons.SetActive(true);
            open = true;
        }
        else if (open)
        {
            buttons.SetActive(false);
            settings.SetActive(false);
            open = false;
        }
    }
    public void SettingMenu()
    {
        buttons.SetActive(false);
        settings.SetActive(true);
    }
    public void MainMenu()
    {
        PhotonNetwork.Disconnect();
        Loader.Load(Loader.Scene.Lobby);
    }
    public void SwitchScheme()
    {
        if (!toggle)
        {
            Keyboard.SetActive(false);
            gamepad.SetActive(true);
            toggle = true;
            schemeButton.text = "Keyboard";
        }
        else if (toggle)
        {
            gamepad.SetActive(false);
            Keyboard.SetActive(true);
            toggle = false;
            schemeButton.text = "Controller";
        }
    }
}
