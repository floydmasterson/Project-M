using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timeToText : MonoBehaviour
{
    TextMeshPro timeText;
    float time;
    private void Awake()
    {
        timeText= GetComponent<TextMeshPro>();
        time = GameManger.Instance.gameTime + 60;
    }
    private void Update()
    {
        if (time > 0)
        {
            updateTimer(time - Time.deltaTime);
            time -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void updateTimer(float currentTime)
    {
        currentTime += 1;

        float min = Mathf.FloorToInt(currentTime / 60);
        float sec = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format("{0:00} : {1:00}", min, sec);
    }
}
