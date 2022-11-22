using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRot;
    float yRot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //MOuse Input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.smoothDeltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.smoothDeltaTime * sensY;

        yRot += mouseX;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        //Rotation
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);

      
        
    }
}
