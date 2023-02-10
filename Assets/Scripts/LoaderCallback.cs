using UnityEngine;

public class LoaderCallback : MonoBehaviour
{

    [SerializeField] private float baseLoadtime = 0;
    private float elaspedLoadTime = 0;
    private void Update()
    {
        if (elaspedLoadTime < baseLoadtime)
            elaspedLoadTime += Time.deltaTime;
        else if (elaspedLoadTime > baseLoadtime)
            Loader.LoaderCallback();
    }
}
