using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    PlayerController controls;
    bool open = false;
    [SerializeField]
    GameObject Keyboard;
    [SerializeField]
    GameObject gamepad;
    [SerializeField]
    TextMeshProUGUI schemeButton;
    bool toggle = false;
    IEnumerator PreLoad()
    {
        
        GameObject settings = gameObject.transform.GetChild(1).gameObject;
        GameObject control = settings.gameObject.transform.GetChild(2).gameObject;
        GameObject gamepad = control.gameObject.transform.GetChild(1).gameObject;
        yield return new WaitForSecondsRealtime(0.5f);
        settings.SetActive(false);
        control.SetActive(false);
        gamepad.SetActive(false);
    }

    private void OnEnable()
    {
        controls = new PlayerController();
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
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            open = true;
        }
        else if (open)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            open = false;
        }
    }
    public void SettingMenu()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void MainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
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
