using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour
{
    public List<GameObject> spawnLocations = new List<GameObject>();
    public GameObject vesselInstance;

    //For manually testing spawn
    public bool test = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (test)
            StartCoroutine("VesselApproaching");
    }

    public void SpawnVessel(int location)
    {
        Instantiate(vesselInstance, spawnLocations[location].transform.position, Quaternion.identity);
    }

    public IEnumerator VesselApproaching()
    {
        yield return null;
        int rand = Random.Range(0, 2);

        SpawnVessel(rand);
        
        test = false;

        StopCoroutine("VesselApproaching");
    }
}
