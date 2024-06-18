using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public float wanderRadius = 20f;
    public float wanderTimer = 20f;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator;
    private bool isWandering = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = Random.Range(wanderTimer, wanderTimer * 5);
        animator = GetComponent<Animator>();
        StartCoroutine(Wander());
    }

    void Update()
    {
        if (agent.velocity.sqrMagnitude == 0f && !isWandering)
        {
            animator.SetBool("Idle", true);
        }
        else
        {
            animator.SetBool("Idle", false);
        }

        if (target != null && agent.enabled)
        {
            agent.SetDestination(target.position);
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            timer = wanderTimer;

            while (true)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    newPos = hit.position;
                    Debug.Log(hit.position);
                    animator.SetBool("WalkForward", true);
                    break;
                }

                if (agent.enabled)
                {
                    agent.SetDestination(newPos);
                    animator.SetBool("WalkForward", false);
                    isWandering = true;
                }
            }
            yield return new WaitForSeconds(timer);
            isWandering = false;
        }
    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            timer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
        }
    }
}