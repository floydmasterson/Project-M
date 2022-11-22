using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPlayerTracker : MonoBehaviour
{
    private Transform playerPos;
    [Range(0, 10)] public float speed;
    public NavMeshAgent agent;
    // Start is called before the first frame update

    private void Awake()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void OnDestroy()
    {
        if (GameObject.FindGameObjectWithTag("WaveSpawner") != null)
        {
            GameObject.FindGameObjectWithTag("WaveSpawner").GetComponent<WaveSpawner>().spawnedEnemies.Remove(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        agent.transform.position = Vector2.MoveTowards(agent.transform.position, playerPos.position, speed * Time.fixedDeltaTime);
    }
}
