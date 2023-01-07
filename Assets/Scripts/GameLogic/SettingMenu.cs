using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingMenu : MonoBehaviour
{
    Resolution[] Res;
    public TMP_Dropdown ResDrop;

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

}