using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    //public GameObject projectile;
    public float speed;

    public float duration = 1F;
    public Vector3 targetPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else{
            Debug.Log("Speed 0");
        }

        //Debug.Log(Vector3.Distance(targetPos, transform.position));
        if (targetPos != Vector3.zero && Vector3.Distance(targetPos, transform.position) <= 2F)
        {
            gameObject.SetActive(false);
        }

        /*duration -= Time.deltaTime;
        if (duration <= 0)
        {
            gameObject.SetActive(false);
        }*/
    }

}
