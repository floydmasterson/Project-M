using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public delegate void Menu();
    public static event Menu escapeMenuClose;
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
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.001f);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        controls = new PlayerController();

        PlayerManger.escapeMenu += toggleMenu;
        StartCoroutine(PreLoad());
    }
    private void OnDisable()
    {
        PlayerManger.escapeMenu -= toggleMenu;
    }
    void toggleMenu()
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
            escapeMenuClose();  
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
