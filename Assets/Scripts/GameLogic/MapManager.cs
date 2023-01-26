using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public bool mapOpen = false;

    private void Awake()
    {
        Instance = this;
    }
    public void MapChange()
    {
        if (!mapOpen)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);  
            mapOpen = true;
        }
        else if (mapOpen)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);  
            mapOpen = false;
        }
    }
}
