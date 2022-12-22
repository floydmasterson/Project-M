using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager _instance;
    public static SFXManager instance
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<SFXManager>();
            return _instance;
        }
    }

    [HorizontalGroup("Audio Source")]
    [SerializeField] private AudioSource defualtAudioSource;

    [TabGroup("Ambient")]
    [AssetList(Path = "/Audio/Ambient", AutoPopulate = true)]
    public List<SFXClip> ambientSFX;
    [TabGroup("UI")]
    [AssetList(Path = "/Audio/UI", AutoPopulate = true)]
    public List<SFXClip> uiSFX;
    [TabGroup("Player")]
    [AssetList(Path = "/Audio/Player", AutoPopulate = true)]
    public List<SFXClip> playerSFX;
    [TabGroup("Weapon")]
    [AssetList(Path = "/Audio/Weapon", AutoPopulate = true)]
    public List<SFXClip> WeaponSFX;
    [TabGroup("Enemy")]
    [AssetList(Path = "/Audio/Enemy", AutoPopulate = true)]
    public List<SFXClip> EnemySFX;

    public static void PlaySFX(SFXClip sfx, bool waitToFinsh = true, AudioSource audioSource = null)
    {
        if (audioSource == null)
            audioSource = SFXManager.instance.defualtAudioSource;
        if (audioSource == null)
        {
            Debug.LogError("Defualt audio source is missing from SFX manager");
        }

        if(!audioSource.isPlaying || !waitToFinsh)
        {
            audioSource.clip = sfx.clip;
            audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
            audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
            audioSource.Play();
        }
            
    }
     public static void StopSFX(AudioSource audioSource = null)
    {
        if (audioSource == null)
            audioSource = SFXManager.instance.defualtAudioSource;
        if (audioSource == null)
        {
            Debug.LogError("Defualt audio source is missing from SFX manager");
        }

        audioSource.Stop();
    }
    [HorizontalGroup("AudioSource")]
    [ShowIf("@defualtAudioSource  == null")]
    [GUIColor(1f, 0.5f, 0.5f, 1f)]
    [Button]
    private void AddAudioSource()
    {
        defualtAudioSource = gameObject.GetComponent<AudioSource>();

        if(defualtAudioSource == null) 
            defualtAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public enum SFXType
    {
        Ambient,
        Ui,
        Player,
        Weapon,
        Enemy
    }
}
