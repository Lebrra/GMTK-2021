using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void TakeDamage(int amount);

    public void GainHealth(int amount);

    public int GetHealth();
}
