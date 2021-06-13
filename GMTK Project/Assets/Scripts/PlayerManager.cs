using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool active = true;

    PlayerMovement pm;
    PlayerLimbFinder plf;

    [SerializeField][Tooltip("LeftArm = 0\nRightArm = 1\nLeftLeg = 2\nRightLeg = 3")]
    RobotAttack[] limbs = new RobotAttack[4];
    public GameObject[] limbObjects;

    Rigidbody rb;
    Animator anim;

    [Header("Limbs")]
    public int activeLimbCount = 0;
    public int activeLimb = 0;
    bool scrollDelayer = false;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        plf = GetComponent<PlayerLimbFinder>();

        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (active)
        {
            pm.Move(rb, anim, transform);

            if(activeLimbCount > 1 && !scrollDelayer)
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") + Input.GetAxisRaw("ScrollAlt") > 0)
                {
                    // increment active limb
                    IncrementActiveLimb();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") + Input.GetAxisRaw("ScrollAlt") < -0)
                {
                    // decrement active limb
                    DecrementActiveLimb();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                DetatchLimb();
            }

            if (Input.GetMouseButtonUp(1))
            {
                // check if attach is available
                // attach
                AttachLimb(plf.SelectTurret());
            }
        }
    }

    public void AttachLimb(RobotAttack attack)
    {
        //Debug.Log("My prefab:" + attack.myTurretPrefab.name, attack.gameObject);

        if (activeLimbCount == 4)
        {
            Debug.LogWarning("limbs full, cannot add more");
            return;
        }
        else if (!attack)
        {
            Debug.LogWarning("no turrets found");
            return;
        }
        else
        {
            int slot = -1;

            if (!attack.isLeg)
            {
                if (!limbs[0])
                {
                    // insert left arm
                    slot = 0;
                }
                else if (!limbs[1])
                {
                    // insert right arm
                    slot = 1;
                }
            }
            else
            {
                if (!limbs[2])
                {
                    // insert left leg
                    slot = 2;
                }
                else if (!limbs[3])
                {
                    // insert right leg
                    slot = 3;
                }
            }

            if (slot < 0)
            {
                Debug.Log("cannot pick up turret (slots are likely full)");
                return;
            }
            else
            {
                // success, add component and remove turret
                SetLimbComponent(slot, attack);
                plf.RemoveTurret(attack);

                pm.speed -= 2F;
                anim.speed /= 1.2F;
            }
        }
    }

    void SetLimbComponent(int index, RobotAttack attack)
    {
        //anim.enabled = false;
        anim.gameObject.SetActive(false);

        GameObject newLimb = null;
        if (index % 2 == 0) newLimb = Instantiate(Resources.Load("ArmPrefabs/" + attack.myLimbPrefabLeft.name) as GameObject, limbObjects[index].transform);
        else newLimb = Instantiate(Resources.Load("ArmPrefabs/" + attack.myLimbPrefabRight.name) as GameObject, limbObjects[index].transform);
        newLimb.transform.SetSiblingIndex(0);
        newLimb.name = IntToName(index);

        if (limbObjects[index].transform.childCount > 1) Destroy(limbObjects[index].transform.GetChild(1).gameObject);

        /*System.Type type = attack.GetType();
        Component copy = newLimb.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(attack));
        }

        limbs[index] = copy as RobotAttack;*/

        limbs[index] = newLimb.GetComponent<RobotAttack>();

        // final missing elements:
        limbs[index].shootPosition = newLimb.GetComponentInChildren<LimbShootReference>().transform; // change to actual shoot position

        activeLimbCount++;
        activeLimb = index;

        anim.gameObject.SetActive(true);

        limbs[index].SetLimb(attack);
    }

    public void DetatchLimb()
    {
        if (activeLimb > -1 && activeLimb < 4)
        {
            //create new turret instance
            GameObject newTurret = Instantiate(Resources.Load("TurretPrefabs/" + limbs[activeLimb].myTurretPrefab) as GameObject, transform.position + transform.forward.normalized * 2F, transform.rotation);
            newTurret.GetComponent<RobotAttack>().SetTurret(limbs[activeLimb].health, limbs[activeLimb].myTurretPrefab);

            //remove limb
            Destroy(limbs[activeLimb]);
            limbs[activeLimb] = null;
            //Destroy(limbObjects[activeLimb].transform.GetChild(0).gameObject);
            limbObjects[activeLimb].transform.GetChild(0).gameObject.SetActive(false);


            activeLimbCount--;
            if (activeLimbCount > 0) IncrementActiveLimb();
            else activeLimb = -1;

            pm.speed += 2F;
            anim.speed *= 1.2F;
        }
        else Debug.LogWarning("cannot detatch limb unfound");

        Debug.Log("having this debug removes an error?");
    }

    void IncrementActiveLimb()
    {
        scrollDelayer = true;
        StartCoroutine(ResetDelay());

        activeLimb = (activeLimb + 1) % 4;

        int iterator = 0;
        while (limbs[activeLimb] == null)
        {
            activeLimb = (activeLimb + 1) % 4;
            iterator++;

            if (iterator >= 4)
            {
                Debug.LogWarning("Error Setting Limb");
                activeLimb = -1;
                return;
            }
        }

        Debug.Log(activeLimb);
    }

    void DecrementActiveLimb()
    {
        scrollDelayer = true;
        StartCoroutine(ResetDelay());

        activeLimb--;
        if (activeLimb < 0) activeLimb = 3;

        int iterator = 0;
        while (limbs[activeLimb] == null)
        {
            activeLimb--;
            if (activeLimb < 0) activeLimb = 3;
            iterator++;

            if (iterator >= 4)
            {
                Debug.LogWarning("Error Setting Limb");
                activeLimb = -1;
                return;
            }
        }
    }

    IEnumerator ResetDelay()
    {
        yield return new WaitForSeconds(0.2F);
        scrollDelayer = false;
    }

    string IntToName(int index)
    {
        switch (index)
        {
            case 0: return "LArmTurret";
            case 1: return "RArmTurret";
            case 2: return "pCube12";
            case 3: return "pCube39";
            default: return "";
        }
    }
}
