using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class FoodSpawner : MonoBehaviour
{
    private int foodToSpawn;
    private int currentFoodCount;
    private int foodSpawnedSoFar;
    private int foodEatenSoFar;
    private int maxConcurrentNumberOfFood;

    private Room currentRoom;
    private RoomItemSpawnParameters foodSpawnParameters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        foodEatenSoFar = 0;
        currentFoodCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // Don't spawn any food on the entrance or any type of corridor
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // Get a random number of foods to spawn for this room
        foodToSpawn = currentRoom.GetNumberOfItemsToSpawns(GameManager.Instance.GetCurrentDungeonLevel(), 2); ;

        // Get the food spawn parameters for this room
        foodSpawnParameters = currentRoom.GetRoomItemSpawnParameters(GameManager.Instance.GetCurrentDungeonLevel(), 2);

        if (foodToSpawn == 0) return;

        // Get the number of concurrent foods to be spawn in this room
        maxConcurrentNumberOfFood = GetNumberOfFoodToExist();

        // Just spawn food when the room is cleared - add code later
        // when everything as been tested and works properly
        SpawnFood();
    }

    private void SpawnFood()
    {
        StartCoroutine(SpawnFoodCoroutine());
    }

    private IEnumerator SpawnFoodCoroutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Create an instance of the helper class used to select a random food
        RandomSpawnableObject<FoodSO> randomSpawnableObject = new RandomSpawnableObject<FoodSO>(currentRoom.FoodsByLevelList);

        // See if we have space to spawn the food
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            for (int i = 0; i < foodToSpawn; i++)
            {
                while (currentFoodCount >= maxConcurrentNumberOfFood)
                {
                    yield return null;
                }

                Vector3Int foodPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // Creates the food and get the next one to spawn
                CreateFood(randomSpawnableObject.GetRandomItem(), grid.CellToWorld(foodPosition));

                yield return new WaitForSeconds(GetFoodSpawnInterval());
            }
        }
    }

    private void CreateFood(FoodSO foodSO, Vector3 position)
    {
        foodSpawnedSoFar++;
        currentFoodCount++;

        // Get current dungeon level
        GameLevelSO gameLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // Instantiate the food
        GameObject food = Instantiate(foodSO.prefab, position, Quaternion.identity, transform);

        food.GetComponent<Food>().InitializeFood(foodSO, gameLevel);
    }

    private float GetFoodSpawnInterval()
    {
        return Random.Range(foodSpawnParameters.minSpawnInterval, foodSpawnParameters.maxSpawnInterval);
    }

    private int GetNumberOfFoodToExist()
    {
        return Random.Range(foodSpawnParameters.minConcurrentItems, foodSpawnParameters.maxConcurrentItems);
    }
}
