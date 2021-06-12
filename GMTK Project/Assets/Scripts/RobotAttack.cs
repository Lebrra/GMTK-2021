using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAttack : MonoBehaviour
{
    public bool isTurret = true;

    public Transform shootPosition;

    [SerializeField]
    LayerMask enemyLayer;

    public float damage;
    public float range;
    public float projectileRange;
    public float cooldown;

    [Tooltip("Keep this an odd number")]
    public int spreadCount = 1;
    public bool canPierce = false;

    public bool attacking;

    [SerializeField]
    List<Collider> EnemiesList;
    [SerializeField]
    GameObject activeTarget;

    private void Start()
    {
        EnemiesList = new List<Collider>();
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

        Debug.Log("attack!");
        attacking = true;
        ShootRaycast(0, canPierce);

        if(spreadCount > 1)
        {
            // keep this an odd number
            if (spreadCount % 2 == 0) spreadCount++;

            for (int i = 1; i < spreadCount; i+=2)
            {
                ShootRaycast(5 * i, canPierce);
                ShootRaycast(-5 * i, canPierce);
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
        foreach (var rayHit in Physics.RaycastAll(shootPosition.position, -direction.normalized, projectileRange, enemyLayer))
        {
            Debug.Log("I have hit " + rayHit.collider.gameObject, gameObject);
            // damage enemy

            if (!pierce) break;
        }
    }

    protected virtual void DoDamage(GameObject hit)
    {
        // do the damage to the thing
    }

    protected virtual IEnumerator FindEnemies()
    {
        // find all enemies within range distance, if not in list then add them         layer 6 is enemies

        List<Collider> withinRange = new List<Collider>();

        foreach (var rayHit in Physics.SphereCastAll(transform.position, range, transform.forward, range, enemyLayer))
        {
            withinRange.Add(rayHit.collider);
            if (!EnemiesList.Contains(rayHit.collider)) EnemiesList.Add(rayHit.collider);
        }

        // check if any are dead and/or out of range
        if (EnemiesList.Count > 0)
        {
            for (int i = EnemiesList.Count - 1; i >= 0; i--)
            {
                if (withinRange.Contains(EnemiesList[i])) continue;
                else
                {
                    // is this check needed? if its not in the list it should be removed right?
                    //if (Vector3.Distance(EnemiesList[i].transform.position, transform.position) > range) // or if enemy is dead
                    //{
                        EnemiesList.RemoveAt(i);
                    //}
                }
            }
        }

        if (activeTarget && !EnemiesList.Contains(activeTarget.GetComponent<Collider>()))
        {
            if (EnemiesList.Count > 0) activeTarget = EnemiesList[0].gameObject;
            else activeTarget = null;
        }
        else if (!activeTarget && EnemiesList.Count > 0) activeTarget = EnemiesList[0].gameObject;

        Debug.Log("Current enemy count: " + EnemiesList.Count, gameObject);

        yield return new WaitForSeconds(cooldown / 3F);
        StartCoroutine(FindEnemies());
    }
}
