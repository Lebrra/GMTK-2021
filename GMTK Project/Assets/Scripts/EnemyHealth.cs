using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [Header("My health")]
    public int health = 100;

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

        if (health == 0)
        {
            //Add enemy death sfx here
            Debug.Log("Dead");
            VesselManager.inst.goonsRemaining--;
            Destroy(this.gameObject);
        }
    }
}
