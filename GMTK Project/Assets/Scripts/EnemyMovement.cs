using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 destination;
    public bool targetMet = false;

    public Transform target;
    public bool canMove;
    public float attackRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        destination = agent.destination;
    }

    void Update()
    {
        Move();
    }

    //Maybe add a Transform param
    void Move()
    {
        if (canMove)
        {
            if (targetMet)
            {
                agent.SetDestination(transform.position);
            }
            else if (!targetMet)
            {
                destination = target.position;
                agent.destination = destination;

                if (Vector3.Distance(transform.position, destination) < attackRange)
                {
                    print("Yuh");
                    targetMet = true;
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
