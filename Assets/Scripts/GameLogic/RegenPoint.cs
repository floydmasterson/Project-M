using System.Collections;
using UnityEngine;

public class RegenPoint : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectRadius = 5f;
    private float detectTime;
    [SerializeField] ParticleSystem ParticleSystem;
    [SerializeField] SFX Regen;
    AudioSource audioSource;
    [SerializeField]
    float fadeTime = 1f;
    private float startVolume;
    bool playing = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startVolume = audioSource.volume;
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
        Regen.StopSFX();
    }
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);
        if (hitColliders.Length > 0 && Time.time >= detectTime)
        {
            if (playing == false)
            {
                audioSource.volume = startVolume;
                Regen.PlaySFX();
                ParticleSystem.Play();
                playing = true;
            }
            foreach (Collider collider in hitColliders)
            {
                PlayerManger player = collider.gameObject.GetComponent<PlayerManger>();
                MagicController MagicController = collider.gameObject.GetComponent<MagicController>();
                player.Heal(1);
                if (MagicController != null)
                    MagicController.CurrMana += 1;
            }
            detectTime = Time.time + 1f;
        }
        else if (hitColliders.Length == 0 && playing)
        {
            StartCoroutine(FadeOut());
            ParticleSystem.Stop();
            playing = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
