using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    NavMeshObstacle myObstacle;
    Vector3 destination;
    bool targetMet = false;
    bool attacking = false;
    public Transform target;

    [Header("Movement Stats")]
    public bool canMove;
    public float moveSpeed = 2.0f;

    [Header("Combat Stats")]
    public float attackRange;
    public int damage = 10;
    public float attackCooldown;
    [Tooltip("The layers I can see.")]
    public LayerMask sightLayer;
    public List<Transform> thingsInSight = new List<Transform>();
    Animator anim;

    AudioSource audioSource;

    void Start()
    {
        FindVisableTargets();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        myObstacle = GetComponent<NavMeshObstacle>();
        destination = agent.destination;
        agent.speed = moveSpeed;
        anim = GetComponentInChildren<Animator>();
        //thingsInSight.Add(HubReference.reference.transform);
        StartCoroutine("FindTargets", 0.2f);
        //moving = true;

        //Might change this
        canMove = true;
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
                agent.enabled = false;
                myObstacle.enabled = true;
                if (target.GetComponent<RobotAttack>() && !target.GetComponent<RobotAttack>().isDead)
                {
                    Attack();

                    //Might need some changes
                    if (target.GetComponent<RobotAttack>().isDead)
                    {
                        thingsInSight.Remove(target);
                        SetTarget();
                        //agent.SetDestination(target.position);
                        targetMet = false;
                    }
                }
                else if (target.GetComponent<HubReference>())
                {
                    Attack();
                }
            }
            else if (!targetMet)
            {
                myObstacle.enabled = false;
                agent.enabled = true;

                if (target)
                {
                    if (target == HubReference.reference.transform)
                    {
                        destination = target.position;
                        agent.destination = destination;

                        if (Vector3.Distance(transform.position, destination) <= attackRange + 4)
                        {
                            //print("At the hub");
                            targetMet = true;
                        }
                    }
                    else
                    {
                        destination = target.position;
                        agent.destination = destination;

                        if (Vector3.Distance(transform.position, destination) <= attackRange)
                        {
                            targetMet = true;
                        }
                    }
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    public IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            /*
            if (!targetMet)
                FindVisableTargets();
            */
            FindVisableTargets();
        }
    }

    public void FindVisableTargets()
    {
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, 999, sightLayer);

        for(int i = 0; i < targetsInRange.Length; i++)
        {
            Transform t = targetsInRange[i].transform;

            if (thingsInSight.Contains(t))
            {
                //Check if target is dead/inactive
                if (t.GetComponent<RobotAttack>() && t.GetComponent<RobotAttack>().isDead || !t.gameObject.activeInHierarchy)
                {
                    thingsInSight.Remove(t);
                    SetTarget();
                }
                continue;
            }

            thingsInSight.Add(t);

            if(thingsInSight.Count > 1)
            {
                for(int j = thingsInSight.Count-1; j > 0; j--)
                {
                    if (Vector3.Distance(transform.position, thingsInSight[j].position) < Vector3.Distance(transform.position, thingsInSight[j - 1].position))
                    {
                        Transform temp = thingsInSight[j];
                        thingsInSight[j] = thingsInSight[j - 1];
                        thingsInSight[j - 1] = temp;
                    }
                }
            }
        }
        SetTarget();
    }

    //Add the priority for who to target here
    void SetTarget()
    {
        if (thingsInSight.Count > 0)
            target = thingsInSight[0];
    }

    public void Attack()
    {
        if (attacking) return;

        //Attacking for towers
        if (target.GetComponent<RobotAttack>() && !target.GetComponent<RobotAttack>().isDead)
        {
            //Add enemy attack sfx here
            anim.SetTrigger("Attack");
            audioSource.Play();
            target.GetComponent<RobotAttack>().TakeDamage(damage);
        }

        //Attacking for hub
        else if (target.GetComponent<HubReference>())
        {
            //Add enemy attack sfx here
            anim.SetTrigger("Attack");
            audioSource.Play();
            target.GetComponent<HubReference>().TakeDamage(damage);
        }

        attacking = true;

        StartCoroutine(ResetAttack());
    }

    protected IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
    }

    private void OnDrawGizmos()
    {
        //Attack range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        /*
        Sight range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        */
    }

    public void UpdateDestination(Transform t)
    {
        if (thingsInSight.Contains(t))
        {
            thingsInSight.Remove(t);
            targetMet = false;
            SetTarget();
            destination = target.position;
            //agent.destination = destination;
        }
    }
}
