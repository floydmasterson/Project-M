using System;
using System.Collections;
using UnityEngine;

public class RegenPoint : MonoBehaviour
{
    PlayerManger Player;
    MagicController MagicController;
    [SerializeField] ParticleSystem ParticleSystem;
    [SerializeField] SFX Regen;
    AudioSource audioSource;
    [SerializeField]
    float fadeTime = 1f;
    private float startVolume;
    private void Awake()
    {
        audioSource= GetComponent<AudioSource>();
        startVolume = audioSource.volume;
    }
    private IEnumerator HealOT()
    {
        if (Player != null)
        {
            Player.Heal(1);
            if (MagicController != null)
                MagicController.CurrMana += 1;
            yield return new WaitForSecondsRealtime(1);
            StartCoroutine("HealOT");
        }
        else if (Player == null)
            StopCoroutine("HealOT");
      
    }
    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.gameObject.GetComponent<PlayerManger>();
            MagicController = other.gameObject.GetComponent<MagicController>();
            StartCoroutine("HealOT");
            if(audioSource.volume == 0)
                audioSource.volume = startVolume;
            Regen.PlaySFX();
            ParticleSystem.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player = null;
        MagicController = null;
        ParticleSystem.Stop();
        ParticleSystem.Clear();
        StartCoroutine(FadeOut());
    }


}
