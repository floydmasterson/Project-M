using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetMoveV3 : MonoBehaviour
{
  public NavMeshAgent agent;
    
    [Range(0, 10)] public float speed;
    [Range(0, 50)] public float moveRadius;
    // Start is called before the first frame update
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
       
        if (agent != null)
        {
            agent.speed = speed;
            agent.SetDestination(RandomNavLocal());
        }
    }
    public Vector3 RandomNavLocal()
    {
     

        Vector3 finalPostion = Vector3.zero;
        Vector3 randomPos = Random.insideUnitCircle * moveRadius;
        randomPos += transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, moveRadius, 1))
        {
            finalPostion = hit.position;  
        }
        return finalPostion;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(agent != null && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(RandomNavLocal());
        }
    }
    void OnDestroy()
    {
        
        if (GameObject.FindGameObjectWithTag("WaveSpawner") != null)
        {
            GameObject.FindGameObjectWithTag("WaveSpawner").GetComponent<WaveSpawner>().spawnedEnemies.Remove(gameObject);
        }
    }
}