using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class VolumeControler : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 30f;
    [SerializeField] Toggle toggle;
    bool disableToggleEvent;
    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChange);
        toggle.onValueChanged.AddListener(HandleToggleValueChange);
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
        if (toggle.isOn)
            PlayerPrefs.SetInt(toggle.gameObject.name.ToString(), 1);
        else if (!toggle.isOn)
            PlayerPrefs.SetInt(toggle.gameObject.name.ToString(), 0);
    }
    private void Start()
    {
            slider.value = PlayerPrefs.GetFloat(volumeParameter, 7);
            toggle.isOn = PlayerPrefs.GetInt(toggle.gameObject.name.ToString(), 1) > 0;    
    }

    private void HandleToggleValueChange(bool enableSound)
    {
        if (disableToggleEvent)
            return;
        if (enableSound)
            slider.value = slider.maxValue;
        else
            slider.value = slider.minValue;
    }
    private void HandleSliderValueChange(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }
}
