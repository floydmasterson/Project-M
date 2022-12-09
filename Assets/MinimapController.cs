using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 newPosition = PlayerUi.Instance.target.gameObject.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
