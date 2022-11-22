using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Text healthText;
    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Update()
    {
       // healthText.text = "Health: " + HealthTracker.currentHealth + "/3";
    }

    // Update is called once per frame
   
}
