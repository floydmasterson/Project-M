using Photon.Pun;
using UnityEngine;

public class Crosshair : MonoBehaviourPun
{
    [SerializeField] Texture2D crosshairImage;
    [SerializeField] int size;
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;

    float lookHight;

    private void OnEnable()
    {
        MapManager.MapState += sizeToggle;
    }
    private void OnDisable()
    {
        MapManager.MapState -= sizeToggle;
    }
    void LookHight(float value)
    {
        lookHight += value;
        if (lookHight > maxAngle || lookHight < minAngle)
            lookHight -= value;
    }

    void sizeToggle(bool state)
    {
        if (state == true)
            size = 0;
        else if (state == false)
            size = 32;
    }

    private void OnGUI()
    {
        if (photonView.IsMine)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.y = Screen.height - screenPos.y;
            GUI.DrawTexture(new Rect(screenPos.x, screenPos.y - lookHight, size, size), crosshairImage);
        }
    }
}
