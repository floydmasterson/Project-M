using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    Resolution[] Res;
    public TMP_Dropdown ResDrop;
    public static event Action<bool> HealNumberToggle;
    public static event Action<bool> DmgNumberToggle;
    [SerializeField] Toggle Healtoggle;
    [SerializeField] Toggle Dmgtoggle;

    private void Awake()
    {
        Healtoggle.onValueChanged.AddListener(healNumberToggle);
        Dmgtoggle.onValueChanged.AddListener(dmgNumberToggle);
    }
    private void OnDisable()
    {
        setToggle(Healtoggle);
        setToggle(Dmgtoggle);
    }
    private void Start()
    {
        Res = Screen.resolutions;
        ResDrop.ClearOptions();
        List<string> options = new List<string>();
        int currRes = 0;
        for (int i = 0; i < Res.Length; i++)
        {
            string option = Res[i].width + " x " + Res[i].height;
            options.Add(option);

            if (Res[i].width == Screen.currentResolution.width)

            {
                if (Res[i].height == Screen.currentResolution.height)
                {
                    currRes = i;
                }

            }
        }
        ResDrop.AddOptions(options);
        ResDrop.value = currRes;
        ResDrop.RefreshShownValue();
        checkToggle(Healtoggle);
        checkToggle(Dmgtoggle);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution res = Res[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void OpenSystem()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }
    public void OpenControls()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }
    void setToggle(Toggle toggle)
    {
        if (toggle.isOn)
            PlayerPrefs.SetInt(toggle.gameObject.name.ToString(), 1);
        else if (!toggle.isOn)
            PlayerPrefs.SetInt(toggle.gameObject.name.ToString(), 0);
    }
    void checkToggle(Toggle toggle)
    {
        if (PlayerPrefs.HasKey(toggle.gameObject.name.ToString()))
            toggle.isOn = PlayerPrefs.GetInt(toggle.gameObject.name.ToString(), 1) > 0;
        else if (!PlayerPrefs.HasKey(toggle.gameObject.name.ToString()))
        {
            toggle.isOn = true;
            setToggle(toggle);
        }

    }
    void healNumberToggle(bool togglestate)
    {
        if (togglestate)
            HealNumberToggle(true);
        else if (!togglestate)
            HealNumberToggle(false);
    }
    void dmgNumberToggle(bool togglestate)
    {
        if (togglestate)
            DmgNumberToggle(true);
        else if (!togglestate)
            DmgNumberToggle(false);
    }
}