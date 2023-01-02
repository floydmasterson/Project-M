using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private void LateUpdate()
    {
        if (PlayerUi.Instance != null && PlayerUi.Instance.target != null)
        {
            Vector3 newPosition = PlayerUi.Instance.target.gameObject.transform.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}
