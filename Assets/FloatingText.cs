using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime;
    Vector3 camreaDir;
    void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
    private void Update()
    {
        camreaDir = Camera.main.transform.forward;
        camreaDir.y = 0;

        transform.rotation =  Quaternion.LookRotation(camreaDir);
    }
}

