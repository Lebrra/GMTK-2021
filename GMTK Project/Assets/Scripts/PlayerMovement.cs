using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;
    public float blendValue = 0.1F;

    public void Move(Rigidbody rb, Animator anim, Transform myBody)
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 forward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        Vector3 right = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);

        float generalVelocity = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
        anim.SetBool("isMoving", generalVelocity > blendValue);

        if (generalVelocity > blendValue / 2F)
        {
            float finalSpd = speed;
            if (generalVelocity > 1) finalSpd = finalSpd * 0.7F;    // nerf diagonal running

            Vector3 finalVel = new Vector3(forward.x * finalSpd, rb.velocity.y, forward.z * finalSpd) * moveInput.y + new Vector3(right.x * finalSpd, 0, right.z * finalSpd) * moveInput.x;

            rb.velocity = finalVel;
            myBody.rotation = Quaternion.Slerp(myBody.rotation, Quaternion.LookRotation(finalVel), 0.15F);
        }
    }
}
