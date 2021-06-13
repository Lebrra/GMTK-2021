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
        StartCoroutine(ResetAttack());
    }

    IEnumerator LaserVisual()
    {
        laser?.SetActive(true);

        yield return new WaitForSeconds(cooldown / 3F);

        laser?.SetActive(false);
    }

    public override void SetLimb()
    {
        laser = null;
        StartCoroutine(FindEnemies());
    }

    protected override IEnumerator HitVisual(Vector3 hitPoint)
    {
        yield return new WaitForSeconds(0.05F);

        if (hitFlash)
        {
            GameObject effect = Instantiate(hitFlash, hitPoint, rotatePoint.rotation);
            StartCoroutine(effect.GetComponent<ParticleTimer>().LifeSpan(0.6F));
        }
    }
}
