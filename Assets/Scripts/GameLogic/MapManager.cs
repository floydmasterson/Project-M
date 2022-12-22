using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public bool mapOpen = false;

    public static event Action<bool> MapState;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !mapOpen)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (MapState != null)
                MapState(true);
            mapOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapOpen)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            if (MapState != null)
                MapState(false);
            mapOpen = false;
        }
    }
}
