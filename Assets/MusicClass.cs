using UnityEngine;

public class MusicClass : MonoBehaviour
{
    private AudioSource _audioSource;
    public static MusicClass Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}