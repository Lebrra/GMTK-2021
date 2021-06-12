using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAttack : MonoBehaviour, IHealth
{
    [Header("Limb Properties")]
    public bool isLeg = false;
    public bool isTurret = true;

    [Header("Health")]
    [SerializeField]
    int health;
    int maxHealth;

    public bool isBroken = false;

    [Header("Weapon Properties")]
    [Tooltip("Defaults to its own position")]
    public Transform shootPosition;

    public int damage;
    public float range;
    public float projectileRange;
    public float cooldown;

    [Tooltip("Keep this an odd number")]
    public int spreadCount = 1;
    public bool canPierce = false;
    public bool isAOE = false;

    [Header("Targetting")]
    [SerializeField]
    LayerMask targetLayer;
    [SerializeField]
    List<Collider> TargetList;
    [SerializeField]
    GameObject activeTarget;

    bool attacking = false;

    private void Start()
    {
        maxHealth = health;
        TargetList = new List<Collider>();
        StartCoroutine(FindEnemies());
    }

    private void Update()
    {
        if (!attacking)
        {
            if (isTurret) Attack();
            //else if (Input.GetKeyDown(KeyCode.Space)) Attack();
        }
    }

    public virtual void Attack()
    {
        if (attacking) return;

        //Debug.Log("attack!");
        attacking = true;

        if (isAOE) ShootSphereCast();
        else
        {
            ShootRaycast(0, canPierce);

            if (spreadCount > 1)
            {
                // keep this an odd number
                if (spreadCount % 2 == 0) spreadCount++;

                for (int i = 1; i < spreadCount; i += 2)
                {
                    ShootRaycast(5 * i, canPierce);
                    ShootRaycast(-5 * i, canPierce);
                }
            }
        }
        StartCoroutine(ResetAttack());
    }

    protected IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(cooldown);
        attacking = false;
    }

    protected void ShootRaycast(float angleOffset, bool pierce)
    {
        if (!activeTarget) return;

        Vector3 direction = new Vector3(transform.position.x - activeTarget.transform.position.x, 0F, transform.position.z - activeTarget.transform.position.z);   // change this height later as needed
        direction = Quaternion.Euler(0, angleOffset, 0) * direction;

        //Debug.Log("shooting towards " + direction.normalized.ToString());
        Debug.DrawRay(shootPosition.position, -direction.normalized * projectileRange, Color.red, 1F);
        foreach (var rayHit in Physics.RaycastAll(shootPosition.position, -direction.normalized, projectileRange, targetLayer))
        {
            //Debug.Log("I have hit " + rayHit.collider.gameObject, gameObject);
            DoDamage(rayHit.collider.gameObject);

            if (!pierce) break;
        }
    }

    protected void ShootSphereCast()
    {
        if (TargetList.Count < 1) return;

        //Debug.Log("shooting towards " + direction.normalized.ToString());
        foreach (var rayHit in Physics.SphereCastAll(transform.position, projectileRange, transform.forward, projectileRange, targetLayer))
        {
            //Debug.Log("I have hit " + rayHit.collider.gameObject, gameObject);
            DoDamage(rayHit.collider.gameObject);
        }
    }

    protected virtual void DoDamage(GameObject hit)
    {
        // do the damage to the thing
        if (hit.GetComponent<EnemyHealth>())
        {
            hit.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
        else Debug.LogError("targetted enemy does not have health...", hit);
    }

    protected IEnumerator FindEnemies()
    {
        // find all enemies within range distance, if not in list then add them         layer 6 is enemies

        List<Collider> withinRange = new List<Collider>();

        foreach (var rayHit in Physics.SphereCastAll(transform.position, range, transform.forward, range, targetLayer))
        {
            withinRange.Add(rayHit.collider);
            if (!TargetList.Contains(rayHit.collider)) TargetList.Add(rayHit.collider);
        }

        // check if any are dead and/or out of range
        if (TargetList.Count > 0)
        {
            for (int i = TargetList.Count - 1; i >= 0; i--)
            {
                if (withinRange.Contains(TargetList[i])) continue;
                else
                {
                    // is this check needed? if its not in the list it should be removed right?
                    //if (Vector3.Distance(EnemiesList[i].transform.position, transform.position) > range) // or if enemy is dead
                    //{
                        TargetList.RemoveAt(i);
                    //}
                }
            }
        }

        if (activeTarget && !TargetList.Contains(activeTarget.GetComponent<Collider>()))
        {
            if (TargetList.Count > 0) activeTarget = TargetList[0].gameObject;
            else activeTarget = null;
        }
        else if (!activeTarget && TargetList.Count > 0) activeTarget = TargetList[0].gameObject;

        //Debug.Log("Current target count: " + TargetList.Count, gameObject);

        yield return new WaitForSeconds(cooldown / 3F);
        StartCoroutine(FindEnemies());
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            if (!isBroken)
            {
                isBroken = true;
                health = maxHealth;
            }
            else
            {
                Debug.LogWarning("TURRET DIED");
            }
        }
    }

    public void GainHealth(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }
}
