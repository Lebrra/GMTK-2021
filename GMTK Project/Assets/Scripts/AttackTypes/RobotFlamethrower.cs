using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFlamethrower : RobotAttack
{
    public GameObject flamePrefab;

    protected override void DoVisual(Vector3 shotPos, Vector3 direction, float distance, float duration)
    {
        Debug.Log("do the visual");
    }
}
