using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : MonoBehaviour
{
    public List<GameObject> spawnLocations = new List<GameObject>();

    public GameObject vesselInstance;

    public bool roundState = false;
    public bool spawning = false;

    [Tooltip("The time inbetween vessel spawns")]
    public float timeBtwnReinforcments = 10f;

    public float timeBtwnRounds = 20f;
    public float gameStartTime = 30f;

    int roundNum = 1;
    int vesselCount;
    int maxVessels;

    void Start()
    {
        Invoke("StartTheGame", gameStartTime);
    }

    public void StartTheGame()
    {
        StartCoroutine("StartRound");
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("StartRound");
        }
        */
        switch (roundNum)
        {
            case 1:
                maxVessels = 3;
                return;
            case 2:
                maxVessels = 5;
                return;
            case 3:
                maxVessels = 7;
                return;
            case 4:
                maxVessels = 9;
                return;
            case 5:
                maxVessels = 12;
                return;
            default:
                maxVessels = 12;
                return;
        }
    }

    public void SpawnVessel(int location)
    {
        if (vesselCount < maxVessels)
        {
            Instantiate(vesselInstance, spawnLocations[location].transform.position, Quaternion.identity);
            vesselCount += 1;
        }

        if(vesselCount == maxVessels)
        {
            StopRound();
        }
    }

    public void StartRound()
    {
        AudioManager.inst.PlayIntroSong();
        StartCoroutine("VesselApproaching");
        roundState = true;
        spawning = true;
    }

    public void IShouldChangeThis()
    {
        AudioManager.inst.FadeOut();
    }

    public void StopRound()
    {
        StopCoroutine("VesselApproaching");
        roundState = false;
        spawning = false;
        roundNum++;
        vesselCount = 0;

        Invoke("IShouldChangeThis", 5f);
        Invoke("StartRound", timeBtwnRounds);
    }

    public IEnumerator VesselApproaching()
    {
        int temp = RollRandNum();
        SpawnVessel(temp);

        yield return new WaitForSeconds(timeBtwnReinforcments);

        if (roundState)
        {
            StartCoroutine("VesselApproaching");
        }
    }

    public int RollRandNum()
    {
        int rand = Random.Range(0, spawnLocations.Count);
        return rand;
    }
}
