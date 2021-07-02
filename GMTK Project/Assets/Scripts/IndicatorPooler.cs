using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPooler : MonoBehaviour
{
    public static IndicatorPooler instance;

    public GameObject uiRotatorPrefab;
    List<GameObject> pool;

    Transform playerPos;

    private void Awake()
    {
        if (instance) Destroy(instance);
        instance = this;

        playerPos = FindObjectOfType<PlayerManager>().transform;
    }

    void Start()
    {
        pool = new List<GameObject>();
    }

    public GameObject Take(Transform myPos)
    {
        GameObject newUIRotator;

        if (pool.Count > 0)
        {
            newUIRotator = pool[0];
            pool.RemoveAt(0);
        }
        else
        {
            newUIRotator = Instantiate(uiRotatorPrefab, transform);
        }

        newUIRotator.SetActive(true);
        newUIRotator.GetComponent<UIRotator>().targetLocation = myPos;
        newUIRotator.GetComponent<UIRotator>().playerLocation = playerPos;
        newUIRotator.GetComponent<UIRotator>().ShowUI(true);

        return newUIRotator;
    }

    public void Return(GameObject uiRotator)
    {
        if (!uiRotator) Debug.LogWarning("no UIRotator found.");
        else
        {
            uiRotator.SetActive(false);
            pool.Add(uiRotator);
        }
    }
}
