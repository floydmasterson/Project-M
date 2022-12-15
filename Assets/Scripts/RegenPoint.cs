using System.Collections;
using UnityEngine;

public class RegenPoint : MonoBehaviour
{
    PlayerManger Player;
    MagicController MagicController;
    [SerializeField] ParticleSystem ParticleSystem;
    private IEnumerator HealOT()
    {
        if (Player != null)
        {
            Player.Heal(1);
            if (MagicController != null)
                MagicController.CurrMana += 1;
            yield return new WaitForSecondsRealtime(1);
            StartCoroutine("HealOT");
        }
        else if (Player == null)
            StopCoroutine("HealOT");

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.gameObject.GetComponent<PlayerManger>();
            MagicController = other.gameObject.GetComponent<MagicController>();
            StartCoroutine("HealOT");
            ParticleSystem.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player = null;
        MagicController = null;
        ParticleSystem.Stop();
        ParticleSystem.Clear();
    }


}
