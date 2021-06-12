using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFlamethrower : RobotAttack
{
    public GameObject flameThrower;

    public override void Attack()
    {
        if (!rotatePoint) rotatePoint = transform;
        if (activeTarget) flameThrower?.SetActive(true);
        else flameThrower?.SetActive(false);

        if (attacking || !activeTarget) return;

        //Debug.Log("attack!");
        attacking = true;

        //MuzzleFlash(shootPosition.position);

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

    public override void SetLimb()
    {
        flameThrower = null;
        StartCoroutine(FindEnemies());
    }
}
