using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLimbFinder : MonoBehaviour
{
    public int pickupRange;
    public List<RobotAttack> nearbyLimbs;

    [SerializeField]
    LayerMask turretLayer;

    private void Start()
    {
        GetComponent<SphereCollider>().radius = pickupRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        if (other.GetComponent<RobotAttack>() && !nearbyLimbs.Contains(other.GetComponent<RobotAttack>()))
        {
            nearbyLimbs.Add(other.GetComponent<RobotAttack>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RobotAttack>() && nearbyLimbs.Contains(other.GetComponent<RobotAttack>()))
        {
            nearbyLimbs.Remove(other.GetComponent<RobotAttack>());
        }
    }

    public RobotAttack SelectTurret()
    {
        if (nearbyLimbs.Count == 0) return null;
        else if (nearbyLimbs.Count > 1)
        {
            // find closest limb
        }

        RobotAttack turretChoice = null;
        foreach(RobotAttack turret in nearbyLimbs)
        {
            if (!turret.isBroken)   // and !isDead?
            {
                //select this turret
                turretChoice = turret;
            }
        }

        //if (nearbyLimbs.Contains(turretChoice)) nearbyLimbs.Remove(turretChoice);
        return turretChoice;
    }

    public void RemoveTurret(RobotAttack turret)
    {
        if (nearbyLimbs.Contains(turret)) nearbyLimbs.Remove(turret);
        // REMOVE THIS TARGET FROM ENEMIES
        Destroy(turret.gameObject);
    }
}
