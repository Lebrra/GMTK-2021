using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [Header("My health")]
    public int health = 100;
    public bool isDead = false;

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
        if (!isDead)
        {
            health -= amount;

            if (health <= 0)
            {
                //Add enemy death sfx here
                //Debug.Log("Dead");
                //VesselManager.inst.goonsRemaining--;
                isDead = true;
                //Destroy(this.gameObject);
            }
        }
        if (isDead == true)
        {
            VesselManager.inst.goonsRemaining--;
            Destroy(this.gameObject);
        }
    }
}
