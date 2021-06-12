using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool active = true;

    PlayerMovement pm;

    RobotAttack leftArm = null;
    RobotAttack rightArm = null;
    RobotAttack leftLeg = null;
    RobotAttack rightLeg = null;

    Rigidbody rb;
    Animator anim;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (active)
        {
            pm.Move(rb, anim, transform);
        }
    }

    public void AttachLimb(RobotAttack attack)
    {

    }
}
