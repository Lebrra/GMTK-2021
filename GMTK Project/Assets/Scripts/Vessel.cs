using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
    public Transform goonLocation;
    public GameObject enemyInstance;
    [Tooltip("Time between each enemy spawn.")]
    public float timeBtwnEachGoon = 1f;
    [Tooltip("Time before enemies leave the vessel.")]
    public float holdTime = 3f;
    public int maxGoonCount = 5;
    int goonCount = 0;
    Rigidbody rb;
    public GameObject groundVfx, fireVfx;

    public bool spawning = false;
    public bool canSpawn = false;

    AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        canSpawn = true;
        Invoke("StartSpawning", holdTime);
    }

    private void Update()
    {
        //if (canSpawn && !spawning) StartSpawning();
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.up * -40);
    }

    public void DeployTheGoons()
    {
        if (goonCount < maxGoonCount)
        {
            Instantiate(enemyInstance, goonLocation.position, Quaternion.identity);
            goonCount++;
            if (VesselManager.inst) VesselManager.inst.maxGoonsForRound++;
        }

        if (goonCount == maxGoonCount)
        {
            canSpawn = false;
            Destroy(this.gameObject);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Instantiate(groundVfx, transform);
            audioSource.Play();
            fireVfx.SetActive(false);
        }
    }
}
