using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    int health { get; set; }

    public void TakeDamage(int amount);

    public void GainHealth(int amount);
}
