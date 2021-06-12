using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject enemyInstance;
    public float timeBtwnEachGoon = 1f;

    public bool spawning = false;
    public bool canSpawn = false;

    void Start()
    {
        
    }

    private void Update()
    {
        if (canSpawn && !spawning) StartSpawning();
    }

    public void DeployTheGoons()
    {
        Instantiate(enemyInstance, spawnLocation.position, Quaternion.identity);
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
