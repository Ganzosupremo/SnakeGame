using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemiesKilledSoFar;
    private int maxConcurrentNumberOfEnemies;

    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParemeters;

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
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // Play some music later
        //MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 1.5f, 1f);

        // If the room is any type of corridor of is an entrance, then return
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // If the room is already clear of enemies, then return
        if (currentRoom.isClearOfEnemies) return;

        // Get a random nnumber of enemies to spawn for this room
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // Get the enemy spawn parameters for this room
        roomEnemySpawnParemeters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // If no enemies to spawn, return and mark the room as cleared
        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearOfEnemies = true;
            return;
        }

        // Get the number of concurrent enemies to be spawn in this room
        maxConcurrentNumberOfEnemies = GetConcurrentEnemiesToSpawn();


        //Annnnd lock the doors
        currentRoom.instantiatedRoom.LockDoors();

        // ... And actually spawn the enemies
        SpawnEnemies();
    }

    /// <summary>
    /// Spawns the enemies onto the current room
    /// </summary>
    private void SpawnEnemies()
    {
        if (GameManager.Instance.currentGameState == GameState.BossStage)
        {
            GameManager.Instance.previousGameState = GameState.BossStage;
            GameManager.Instance.currentGameState = GameState.EngagingBoss;
        }
        else if (GameManager.Instance.currentGameState == GameState.Playing)
        {
            GameManager.Instance.previousGameState = GameState.Playing;
            GameManager.Instance.currentGameState = GameState.EngagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    /// <summary>
    /// Spawn the enemies coroutine
    /// </summary>
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // See if we have space to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Wait until the enemy count is less than the max concurrent enemies
                while (currentEnemyCount >= maxConcurrentNumberOfEnemies)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                //Creates the enemy and gets the next one to spawn
                CreateEnemy(randomSpawnableObject.GetRandomItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// <summary>
    /// Gets a random interval spawn value in seconds
    /// </summary>
    private float GetEnemySpawnInterval()
    {
        return Random.Range(roomEnemySpawnParemeters.minSpawnInterval, roomEnemySpawnParemeters.maxSpawnInterval);
    }

    /// <summary>
    /// Gets the number of enemies that can be present in a room at any given time
    /// </summary>
    private int GetConcurrentEnemiesToSpawn()
    {
        return Random.Range(roomEnemySpawnParemeters.minConcurrentEnemies, roomEnemySpawnParemeters.maxConcurrentEnemies);
    }

    /// <summary>
    /// Creates an enemy in the specific position
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // Keep track of the number of enemies already spawned
        enemiesSpawnedSoFar++;

        // Add one to the enemy count - this count is reduced when an enemy is killed
        currentEnemyCount++;

        // Get current dungeon level
        GameLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // Instantiate the enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // Initialize the enemy parameters
        enemy.GetComponent<Enemy>().InitializeEnemy(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // Suscribe to the destroy enemies event
        //enemy.GetComponent<DestroyEvent>().OnDestroy += Enemy_OnDestroyed;
    }

    /// <summary>
    /// Destroy enemy event handler - Implement later
    /// </summary>
    private void Enemy_OnDestroyed()//DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        // Unsuscribe
        //destroyEvent.OnDestroy -= Enemy_OnDestroyed;

        //Reduce the enemy count
        currentEnemyCount--;
        enemiesKilledSoFar++;
        Debug.Log(enemiesKilledSoFar);

        // Call the points scored event, for scoring purposes
        //StaticEventHandler.CallPointsScoredEvent(destroyEventArgs.points);

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isClearOfEnemies = true;

            // Set the state of the game
            if (GameManager.Instance.currentGameState == GameState.EngagingEnemies)
            {
                GameManager.Instance.currentGameState = GameState.Playing;
                GameManager.Instance.previousGameState = GameState.EngagingEnemies;
            }
            else if (GameManager.Instance.currentGameState == GameState.EngagingBoss)
            {
                GameManager.Instance.currentGameState = GameState.BossStage;
                GameManager.Instance.previousGameState = GameState.EngagingBoss;
            }

            //Unlock the doors
            //currentRoom.instantiatedRoom.UnlockTheDoors(Settings.doorUnlockDelay);

            // Play the normal music again
            //MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 1.5f, 1f);

            // Trigger the static event to indicate the room  is clear of enemies
            //StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }
}
