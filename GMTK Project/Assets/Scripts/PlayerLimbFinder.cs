using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLimbFinder : MonoBehaviour
{
    public int pickupRange;
    public List<RobotAttack> nearbyLimbs;

    public ArmConstructor hub;

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
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hub"))
        {
            hub = other.GetComponent<ArmConstructor>();
            hub.SetNearby(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RobotAttack>() && nearbyLimbs.Contains(other.GetComponent<RobotAttack>()))
        {
            nearbyLimbs.Remove(other.GetComponent<RobotAttack>());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hub"))
        {
            hub?.SetNearby(false);
            hub = null;
        }
    }

    public RobotAttack SelectTurret()
    {
        /*if (nearbyLimbs.Count == 0) return null;
        else if (nearbyLimbs.Count > 1)
        {
            // find closest limb
        }*/

        RobotAttack turretChoice = null;
        foreach(RobotAttack turret in nearbyLimbs)
        {
            if (!turret.isBroken && !turret.isDead)   // and !isDead?
            {
                //select this turret
                turretChoice = turret;
            }
        }

        if (!turretChoice && hub)
        {
            if (hub.hasArmReady) return hub.GetArm();
            else AudioManager.inst?.LimbNotReadySound();
        }

        //if (nearbyLimbs.Contains(turretChoice)) nearbyLimbs.Remove(turretChoice);
        return turretChoice;
    }

    public void RemoveTurret(RobotAttack turret)
    {
        if (nearbyLimbs.Contains(turret)) nearbyLimbs.Remove(turret);
        if (turret.gameObject.activeInHierarchy) turret.DestroyTurret();
        //Destroy(turret.gameObject);
    }
}
