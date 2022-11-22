using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class WaveDisplay : MonoBehaviour
{
    
    WaveSpawner waveSpawner;
    [SerializeField] GameObject WaveSpawner;
    public Text waveText;
    // Start is called before the first frame update

    private void Awake()
    {
        waveSpawner = WaveSpawner.GetComponent<WaveSpawner>();
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        waveText.text = "Wave: " + waveSpawner.currWave;
    }
}
