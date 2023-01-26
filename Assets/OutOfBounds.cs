using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
           PlayerManger player = other.GetComponent<PlayerManger>();
            player.TakeDamge(9999, null);
        }
        if(other.CompareTag("enemyMask"))
        {
            Enemys enemy = other.GetComponent<Enemys>();
            enemy.TakeDamge(9999);
        }
    }
}
