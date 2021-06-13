using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAttack : MonoBehaviour, IHealth
{
    [Header("Limb Properties")]
    public bool isLeg = false;
    public bool isTurret = true;

    public GameObject myLimbPrefabLeft;
    public GameObject myLimbPrefabRight;
    public string myTurretPrefab;

    public GameObject smoke;
    public GameObject healthBar;
    HealthBar3D barRef;

    [Header("Health")]
    public int health;
    public int maxHealth;

    public bool isBroken = false;
    public bool isDead = false;

    public float healTimer = 3F;
    public int healAmount = 10;

    [Header("Weapon Properties")]
    [Tooltip("Defaults to its own position")]
    public Transform shootPosition;
    public Transform rotatePoint;

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
    public LayerMask targetLayer;
    [SerializeField]
    protected List<Collider> TargetList;
    [SerializeField]
    protected GameObject activeTarget;

    protected bool attacking = false;
    protected bool lookingForTargets = false;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public GameObject hitFlash;

    protected void Start()
    {
        //TargetList = new List<Collider>();
        //StartCoroutine(FindEnemies());
    }

    protected void Update()
    {
        if (!attacking && !isBroken)
        {
            if(gameObject.activeInHierarchy && !lookingForTargets)
            {
                lookingForTargets = true;
                StartCoroutine(FindEnemies());
            } 
            Attack();
        }
    }

    public void SetTurret(int currentHealth, string prefabRef)
    {
        attacking = true;
        StartCoroutine(ResetAttack(0.5F));

        health = currentHealth;
        isTurret = true;
        myTurretPrefab = prefabRef;

        healthBar = Instantiate(healthBar, transform);
        healthBar.transform.rotation = Quaternion.identity;
        barRef = healthBar.GetComponent<HealthBar3D>();
        if (health == maxHealth) healthBar.SetActive(false);
        else barRef?.SetHealth((float)health / (float)maxHealth);

        TargetList = new List<Collider>();
        StartCoroutine(FindEnemies());
    }

    public virtual void SetLimb(RobotAttack priorRef)
    {
        myTurretPrefab = priorRef.myTurretPrefab;
        isTurret = false;

        health = priorRef.health;
        maxHealth = priorRef.maxHealth;

        attacking = true;
        //if (gameObject.activeInHierarchy)
        //{
        TargetList = new List<Collider>();
        StartCoroutine(FindEnemies());
        StartCoroutine(ResetAttack(0.5F));
        StartCoroutine(Heal());
        //}
    }

    IEnumerator Heal()
    {
        yield return new WaitForSeconds(healTimer);

        GainHealth(healAmount);
        if (health < maxHealth && !isDead) StartCoroutine(Heal());
    }

    public virtual void Attack()
    {
        if (!rotatePoint) rotatePoint = transform;

        if (attacking || !activeTarget) return;

        //Debug.Log("attack!");
        attacking = true;

        MuzzleFlash(shootPosition.position);

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
        StartCoroutine(ResetAttack(cooldown));
    }

    protected IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        attacking = false;
    }

    protected void ShootRaycast(float angleOffset, bool pierce)
    {
        if (!activeTarget) return;

        Vector3 direction = new Vector3(transform.position.x - activeTarget.transform.position.x, rotatePoint.position.y - activeTarget.transform.position.y, transform.position.z - activeTarget.transform.position.z);   // change this height later as needed
        direction = Quaternion.Euler(0, angleOffset, 0) * direction;
        direction = new Vector3(-direction.normalized.x, -0.05F, -direction.normalized.z);
        //direction = new Vector3(-direction.normalized.x, -direction.normalized.y, -direction.normalized.z);

        if (/*isTurret && */rotatePoint != transform && angleOffset == 0)
        {
            //rotatePoint.rotation = Quaternion.Slerp(rotatePoint.rotation, Quaternion.LookRotation(direction), 0.15F);
            rotatePoint.rotation = Quaternion.LookRotation(direction);
        }
        DoVisual(shootPosition.position, direction, projectileRange, cooldown / 2F);

        //Debug.Log("shooting towards " + direction.normalized.ToString());
        Debug.DrawRay(shootPosition.position, direction * projectileRange, Color.red, 1F);
        foreach (var rayHit in Physics.RaycastAll(shootPosition.position, direction, projectileRange, targetLayer))
        {
            //Debug.Log("I have hit " + rayHit.collider.gameObject, gameObject);
            StartCoroutine(HitVisual(rayHit.point));
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
            StartCoroutine(HitVisual(rayHit.point));
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
        if (!lookingForTargets) lookingForTargets = true;

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
        else activeTarget = null;

        if (activeTarget && !TargetList.Contains(activeTarget.GetComponent<Collider>()))
        {
            if (TargetList.Count > 0) activeTarget = TargetList[0].gameObject;
            else activeTarget = null;
        }
        else if (!activeTarget && TargetList.Count > 0) activeTarget = TargetList[0].gameObject;

        //Debug.Log("Current target count: " + TargetList.Count, gameObject);

        yield return new WaitForSeconds(0.3F);
        if (!isDead) StartCoroutine(FindEnemies());
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

                smoke.SetActive(true);
            }
            else
            {
                isDead = true;
                DestroyTurret();
                Debug.LogWarning("TURRET DIED");
            }
        }

        if (isTurret) barRef?.SetHealth((float)health / (float)maxHealth);
    }

    public void GainHealth(int amount)
    {
        if (isDead) return;

        health += amount;
        if (health > maxHealth) health = maxHealth;
        if (isTurret) barRef?.SetHealth((float)health / (float)maxHealth);
    }

    public int GetHealth()
    {
        return health;
    }

    public void DestroyTurret()
    {
        isDead = true;
        StartCoroutine(Destroying());
    }

    protected virtual IEnumerator Destroying()
    {
        yield return new WaitForSeconds(0.05F);

        foreach(var target in FindObjectsOfType<EnemyMovement>())
        {
            //if (target.GetComponent<EnemyMovement>().thingsInSight.Contains(transform)) target.GetComponent<EnemyMovement>().thingsInSight.Remove(transform);
            // remove enemy target for each nearby enemy
            target?.UpdateDestination(transform);
        }
        Destroy(gameObject);
    }

    protected virtual void DoVisual(Vector3 shotPos, Vector3 direction, float distance, float duration)
    {
        // on default do nothing for now
        //Debug.Log("do the visual");
    }

    protected virtual IEnumerator HitVisual(Vector3 hitPoint)
    {
        yield return new WaitForSeconds(cooldown / 2F);

        if (hitFlash)
        {
            GameObject effect = Instantiate(hitFlash, hitPoint, rotatePoint.rotation);
            StartCoroutine(effect.GetComponent<ParticleTimer>().LifeSpan(0.6F));
        }
    }

    protected virtual void MuzzleFlash(Vector3 shotPos)
    {
        if (muzzleFlash)
        {
            GameObject effect = Instantiate(muzzleFlash, shotPos, rotatePoint.rotation);
            StartCoroutine(effect.GetComponent<ParticleTimer>().LifeSpan(0.6F));
        }
    }
}
