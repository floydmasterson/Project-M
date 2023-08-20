using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] public GameObject spawnPoint;
    [SerializeField] ParticleSystem ps;
    [SerializeField] SFX Respawn;
    Collider col;
    private void Awake()
    {
        col = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerManger player = other.GetComponent<PlayerManger>();
        if (player != null && player.spawnPoint != spawnPoint.transform)
        {
            player.spawnPoint = spawnPoint.transform;
            if (ps != null)
                ps.Play();
            if (Respawn.sfxToPlay != null)
                Respawn.PlaySFX();
        }
    }

    public void Selcted(GameObject inplayer)
    {
        PlayerManger player = inplayer.GetComponent<PlayerManger>();
        if (player != null)
        {
            player.spawnPoint = spawnPoint.transform;
            inplayer.transform.position = spawnPoint.transform.position;
            col.enabled = false;

        }
    }
}
