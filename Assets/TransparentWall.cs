using Unity.VisualScripting;
using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    private GameObject wall;
    private MeshRenderer rend;
    private Camera mainCamera;
    private int wallLayer = 14; //change this to the layer number of your wall layer
    [SerializeField, Range(0, 20)]
    private float maxRaycastDistance = 5f;

    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, 1 << wallLayer))
        {
            if (wall != hit.collider.gameObject)
            {
                wall = hit.collider.gameObject;
                rend = wall.GetComponent<MeshRenderer>();
                rend.enabled = false;
            }
        }
        else if (wall != null)
        {
            rend.enabled = true;
            rend = null;
            wall = null;
            
        }
    }
}
