using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    private int health = 100;

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
