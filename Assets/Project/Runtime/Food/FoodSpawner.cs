using SnakeGame.Debuging;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame.FoodSystem
{
    [DisallowMultipleComponent]
    public class FoodSpawner : MonoBehaviour
    {
        private int foodToSpawn;
        private int currentFoodCount;

        public int FoodSpawnedSoFar { get; set; }
        private int foodEatenSoFar;
        private int maxConcurrentNumberOfFood;

        private bool shouldSpawnFood = true;

        private Room currentRoom;
        private RoomItemSpawnParameters foodSpawnParameters;
        private Coroutine spawnFoodRoutine;

        private void OnEnable()
        {
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
        {
            FoodSpawnedSoFar = 0;
            currentFoodCount = 0;
            shouldSpawnFood = true;

            currentRoom = roomEnemiesDefeatedArgs.room;

            // Don't spawn food on corridors, entrances or chest rooms
            if (currentRoom.roomNodeType.isCorridorEW ||
                currentRoom.roomNodeType.isCorridorNS ||
                currentRoom.roomNodeType.isEntrance ||
                currentRoom.roomNodeType.isChestRoom)
                return;

            if (!currentRoom.IsClearOfEnemies || !shouldSpawnFood) return;

            // Get a random number of foods to spawn for this room
            foodToSpawn = currentRoom.GetNumberOfItemsToSpawn(GameManager.Instance.GetCurrentDungeonLevel(), false);

            // Get the food spawn parameters for this room
            foodSpawnParameters = currentRoom.GetRoomItemSpawnParameters(GameManager.Instance.GetCurrentDungeonLevel(), 2);

            if (foodToSpawn == 0) return;

            // Get the number of concurrent foods to be spawn in this room
            maxConcurrentNumberOfFood = GetNumberOfFoodToExist();

            // Finally spawn the food on the world
            SpawnFood();
        }

        private void SpawnFood()
        {
            if (spawnFoodRoutine != null)
                StopCoroutine(spawnFoodRoutine);
            spawnFoodRoutine = StartCoroutine(SpawnFoodCoroutine());
        }

        private IEnumerator SpawnFoodCoroutine()
        {
            Grid grid = currentRoom.InstantiatedRoom.grid;

            // Create an instance of the helper class used to select a random food
            RandomSpawnableObject<FoodSO> randomSpawnableObject = new(currentRoom.FoodsByLevelList);

            // See if we have space to spawn the food
            if (currentRoom.spawnPositionArray.Length > 0)
            {
                for (int i = 0; i < foodToSpawn; i++)
                {
                    // Wait until the food count is less than the max concurrent foods
                    while (currentFoodCount >= maxConcurrentNumberOfFood)
                    {
                        yield return null;
                    }

                    Vector3Int foodPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                    // Creates the food and get the next one to spawn
                    FoodSO a = randomSpawnableObject.GetRandomItem();
                    CreateFood(a, grid.CellToWorld(foodPosition));

                    yield return new WaitForSeconds(GetFoodSpawnInterval());
                }
            }
        }

        private void CreateFood(FoodSO foodSO, Vector3 position)
        {
            FoodSpawnedSoFar++;
            currentFoodCount++;

            // Instantiate the food from the pool
            Food food = (Food)PoolManager.Instance.ReuseComponent(foodSO.FoodPrefab, position, Quaternion.identity);
            food.gameObject.SetActive(true);
            
            food.InitializeFood(foodSO);
            food.GetComponent<DestroyEvent>().OnDestroy += FoodSpawner_OnDestroy;
        }

        private void FoodSpawner_OnDestroy(DestroyEvent destroyEvent, DestroyedEventArgs destroyedEventArgs)
        {
            destroyEvent.OnDestroy -= FoodSpawner_OnDestroy;

            // Reduce the food count
            currentFoodCount--;

            foodEatenSoFar++;

            // Score points
            StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

            if (currentFoodCount <= 0 && FoodSpawnedSoFar == foodToSpawn)
            {
                shouldSpawnFood = false;
            }
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
}