using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
    [SerializeField] GameObject XPrefab;
    [SerializeField] GameObject MapImage;

    public void SpawnX()
    {
        GameObject xMark = Instantiate(XPrefab);
        xMark.transform.SetParent(MapImage.transform, false);
    }

}
