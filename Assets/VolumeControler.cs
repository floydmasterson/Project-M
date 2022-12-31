using Unity.VisualScripting;
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
    }
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
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
