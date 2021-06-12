using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubReference : MonoBehaviour, IHealth
{
    public static HubReference reference;
    public int health = 100;

    void Awake()
    {
        if (reference) Destroy(gameObject);
        else reference = this;
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
    }

}
