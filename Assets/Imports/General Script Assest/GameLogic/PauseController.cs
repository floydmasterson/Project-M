using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    public UnityEvent GamePaused;
    public UnityEvent GameResumed;

    public AudioSource audioS;
  
    public GameObject PauseScreen;

   public bool _isPaused;
    public bool canPause;


    private void Awake()
    {
        canPause = true;
        Instance = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canPause == true)
            {
                _isPaused = !_isPaused;

                if (_isPaused)
                {
                    Time.timeScale = 0;
                    audioS.Pause();
                    PauseScreen.SetActive(true);
                    GamePaused.Invoke();
                }
                else
                {
                    Time.timeScale = 1;
                    audioS.Play();
                    PauseScreen.SetActive(false);
                    GameResumed.Invoke();
                }
            }
        }

    }
   

}