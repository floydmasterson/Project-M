using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class healthTracker : MonoBehaviour
{
    public static healthTracker Instance;
    private int health = 3;
    public float currentHealth;
    // Start is called before the first frame update
    private void Awake()
    {
        currentHealth = health;
        Instance = this; 
    }

    // Update is called once per frame
    void Update()
    {
        //if (//Relavent == true)
       // {
          // LoseHealth();
           
       // }
    }
    public void LoseHealth()
    {
        currentHealth--;
        if (currentHealth == 0)
        {
            GameOver();
        }
        
  
    }

    public void GainHealth()
    {
        if (currentHealth <= 2)
        {
            currentHealth++;
        }
        
    }
    public void GameOver()
    {
           //relavant game over code

            
    }
}
