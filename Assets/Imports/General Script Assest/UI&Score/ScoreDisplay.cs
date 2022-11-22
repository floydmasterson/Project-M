using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
  
    public int score = 0;
    public Text scoreText;

    private void Awake()
    {
       // MouseTarget.isHitting = false;
    }
   
    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score;

       // if (MouseTarget.isHitting == true)
        //{
           // score++; 
       // }
    }
}
