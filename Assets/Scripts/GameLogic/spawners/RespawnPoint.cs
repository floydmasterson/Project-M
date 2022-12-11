using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] GameObject spawnPoint;
    [SerializeField] ParticleSystem ps;
    private void OnTriggerEnter(Collider other)
    {
        PlayerManger player = other.GetComponent<PlayerManger>();
        if (player != null && player.spawnPoint != spawnPoint.transform)
        {
            player.spawnPoint = spawnPoint.transform;
            if (ps != null)
                ps.Play();
        }
    }
}
