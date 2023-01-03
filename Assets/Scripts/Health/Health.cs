using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;

    /// <summary>
    /// Sets the starting health for the snake
    /// </summary>
    /// <param name="startingHealth">The initial health amount</param>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// Gets the starting health
    /// </summary>
    /// <returns>The starting health</returns>
    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
