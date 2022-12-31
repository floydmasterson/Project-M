using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public delegate void Menu();
    public static event Menu escapeMenuClose;
    PlayerController controls;
    bool open = false;
    private void OnEnable()
    {
        controls = new PlayerController();
        controls.Escape.CloseEscape.performed += context => toggleMenu();
        PlayerManger.escapeMenu += toggleMenu;
    }
    private void OnDisable()
    {
        controls.Escape.CloseEscape.performed -= context => toggleMenu();
        PlayerManger.escapeMenu -= toggleMenu;
    }

    void toggleMenu()
    {
        if (!open)
        {
            controls.Escape.Enable();
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            open = true;
        }
        else if (open)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            open = false;
            escapeMenuClose();
            controls.Escape.Disable();
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
}
