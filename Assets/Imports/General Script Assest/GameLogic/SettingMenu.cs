using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    Resolution[] Res;
    public TMPro.TMP_Dropdown ResDrop;
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
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution res = Res[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
};
