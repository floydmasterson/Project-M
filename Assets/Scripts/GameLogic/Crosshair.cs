using Photon.Pun;
using UnityEngine;

public class Crosshair : MonoBehaviourPun
{
    [SerializeField] Texture2D crosshairImage;
    [SerializeField] int size;
    [SerializeField] float maxAngle;
    [SerializeField] float minAngle;

    float lookHight;
    bool visable = true;

    private void OnEnable()
    {
        MapManager.MapState += state => sizeToggle();
        PlayerManger.onInventoryOpen += sizeToggle;
        PlayerManger.onInventoryClose += sizeToggle;
        PlayerManger.escapeMenu += sizeToggle;
     
    }
    private void OnDisable()
    {
        MapManager.MapState -= state => sizeToggle();
        PlayerManger.onInventoryOpen -= sizeToggle;
        PlayerManger.onInventoryClose -= sizeToggle;
        PlayerManger.escapeMenu -= sizeToggle;
    }
    void LookHight(float value)
    {
        lookHight += value;
        if (lookHight > maxAngle || lookHight < minAngle)
            lookHight -= value;
    }

    void sizeToggle()
    {
        if (visable == false)
        {
            size = 32;
            visable = true;

        }
        else if (visable == true)
        {
            size = 0;
            visable = false;

        }
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
