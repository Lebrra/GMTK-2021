using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
    public Transform goonLocation;
    public GameObject enemyInstance;
    public float timeBtwnEachGoon = 1f;
    public int maxGoonCount = 5;
    int goonCount = 0;

    public bool spawning = false;
    public bool canSpawn = false;

    void Start()
    {
        canSpawn = true;
        Invoke("StartSpawning", 2f);
    }

    private void Update()
    {
        //if (canSpawn && !spawning) StartSpawning();
    }

    public void DeployTheGoons()
    {
        if (goonCount < maxGoonCount)
        {
            Instantiate(enemyInstance, goonLocation.position, Quaternion.identity);
            goonCount++;
        }

        if (goonCount == maxGoonCount)
            Destroy(this.gameObject);
    }

    public void StartSpawning()
    {
        spawning = true;
        StartCoroutine("Spawning");
    }

    public IEnumerator Spawning()
    {
        DeployTheGoons();
        yield return new WaitForSeconds(timeBtwnEachGoon);

        if (canSpawn) StartCoroutine("Spawning");
    }
}
