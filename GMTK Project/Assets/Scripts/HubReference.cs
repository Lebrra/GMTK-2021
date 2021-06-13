using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubReference : MonoBehaviour, IHealth
{
    public static HubReference reference;
    public int health = 100;

    public bool gameState = true;

    public GameObject gameOverPanel;

    void Awake()
    {
        if (reference) Destroy(gameObject);
        else reference = this;
    }

    private void Start()
    {
        //gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!gameState)
        {
            Time.timeScale = 0;
            //gameOverPanel.SetActive(true);
        }
    }

    public void GainHealth(int amount)
    {
        health += amount;
    }

    public int GetHealth()
    {
        return health;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if(health <= 0)
        {
            gameState = false;
        }
    }
}
