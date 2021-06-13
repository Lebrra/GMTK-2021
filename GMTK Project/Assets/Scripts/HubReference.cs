using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubReference : MonoBehaviour, IHealth
{
    public static HubReference reference;
    public int health = 100;
    int maxHealth;

    public bool gameState = true;

    public GameObject gameOverPanel;

    public HealthBar3D myHealthBar;

    void Awake()
    {
        if (reference) Destroy(gameObject);
        else
        {
            reference = this;
            maxHealth = health;
        }
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
        myHealthBar?.SetHealth((float)health / (float)maxHealth);

        if(health <= 0)
        {
            gameState = false;
        }
    }
}
