using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLaser : RobotAttack
{
    public GameObject laser;

    public override void Attack()
    {
        if (!rotatePoint) rotatePoint = transform;

        if (attacking || !activeTarget) return;

        //Debug.Log("attack!");
        attacking = true;

        //MuzzleFlash(shootPosition.position);
        PlaySound();

        if (isAOE) ShootSphereCast();
        else
        {
            ShootRaycast(0, canPierce);
            StartCoroutine(LaserVisual());

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

    IEnumerator LaserVisual()
    {
        if (laser) laser?.SetActive(true);

        yield return new WaitForSeconds(cooldown / 3F);

        if (!isDead) laser?.SetActive(false);
    }

    protected override IEnumerator HitVisual(Vector3 hitPoint)
    {
        yield return new WaitForSeconds(0.05F);

        if (hitFlash)
        {
            GameObject effect = Instantiate(hitFlash, hitPoint, rotatePoint.rotation);
            effect.GetComponent<ParticleTimer>().LifeSpan(0.6F);
        }
    }

    protected override IEnumerator Destroying()
    {
        if (laser)
        {
            StopCoroutine("LaserVisual");
            Destroy(laser);
        }

        yield return new WaitForSeconds(0.05F);

        foreach (var target in FindObjectsOfType<EnemyMovement>())
        {
            //if (target.GetComponent<EnemyMovement>().thingsInSight.Contains(transform)) target.GetComponent<EnemyMovement>().thingsInSight.Remove(transform);
            // remove enemy target for each nearby enemy
            target?.UpdateDestination(transform);
        }
        Destroy(gameObject);
    }

    protected override void PlaySound()
    {
        if (isTurret) GetComponent<AudioSource>().Play();
        else AudioManager.inst?.SniperTurretSound();
    }
}
