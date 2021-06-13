using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmConstructor : MonoBehaviour
{
    public RobotAttack[] turretTypes;

    public float contructionTimer = 30F;
    public bool hasArmReady = false;

    [Header("Notification")]
    public MeshRenderer notificationObject;

    bool nearby = false;
    public Material defaultMat;
    public Material nearbyMat;

    [Header("UI")]
    public UIRotator ui;

    void Start()
    {
        notificationObject.gameObject.SetActive(false);
        StartCoroutine(ConstructArm());
        ui?.ShowUI(false);
    }

    public RobotAttack GetArm()
    {
        if (!hasArmReady) return null;
        else
        {
            hasArmReady = false;
            notificationObject.gameObject.SetActive(false);
            ui?.ShowUI(false);

            StartCoroutine(ConstructArm());

            return turretTypes[Random.Range(0, turretTypes.Length)];
        }
    }

    IEnumerator ConstructArm()
    {
        yield return new WaitForSeconds(contructionTimer);

        hasArmReady = true;
        notificationObject.gameObject.SetActive(true);
        ui?.ShowUI(true);
        SetNearby(nearby);
    }

    public void SetNearby(bool near)
    {
        nearby = near;
        if (hasArmReady)
        {
            if (nearby) notificationObject.material = nearbyMat;
            else notificationObject.material = defaultMat;
        }
    }
}
