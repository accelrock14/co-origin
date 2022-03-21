using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int level;
    public int damage;

    public int maxHealth;
    public int currentHealth;

    public bool TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
            return true;
        else
            return false;
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
