using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Food : MonoBehaviour
{
    // The grid dimensions
    private int gridWidth = 20;
    private int gridHeight = 20;
    private float timer = 2f;

    // The prefab for the food
    public GameObject foodPrefab;

    // The position of the food
    int foodX = 0;
    int foodY = 0;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag))
        {
            DisableFood();
        }
    }

    public GameObject GenerateFood()
    {
        foodX = Random.Range(0, gridWidth);
        foodY = Random.Range(0, gridHeight);
        return Instantiate(this.gameObject, new Vector2(foodX, foodY), Quaternion.identity);
    }

    private void DisableFood()
    {
        gameObject.SetActive(false);
    }
}
