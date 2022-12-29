using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Food : MonoBehaviour
{
    // The grid dimensions
    private int gridWidth = 20;
    private int gridHeight = 20;

    // The prefab for the food
    private GameObject foodPrefab;

    // The position of the food
    int foodX = 0;
    int foodY = 0;

    private void Awake()
    {
        gridWidth = SnakeAI.Instance.GridWidth;
        gridHeight = SnakeAI.Instance.GridHeight;
        foodPrefab = this.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GenerateFood();
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
