using Cysharp.Threading.Tasks;
using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using SnakeGame.HealthSystem;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame.Enemies
{
    [DisallowMultipleComponent]
    public class EnemySpawner : MonoBehaviour
    {
        private int enemiesToSpawn;
        private int currentEnemyCount;
        private int enemiesSpawnedSoFar;
        private int enemiesKilledSoFar;
        private int maxConcurrentNumberOfEnemies;

        private Room currentRoom;
        private RoomItemSpawnParameters roomEnemySpawnParemeters;

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        }

        private async void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            enemiesSpawnedSoFar = 0;
            currentEnemyCount = 0;

            currentRoom = roomChangedEventArgs.room;

            if (currentRoom.normalMusic != null && currentRoom.IsClearOfEnemies)
                MusicManager.CallOnMusicClipChangedEvent(currentRoom.normalMusic);

            // Don't spawn enemies on corridors, entrance, chest and exit rooms
            if (currentRoom.roomNodeType.isCorridorEW ||
                currentRoom.roomNodeType.isCorridorNS ||
                currentRoom.roomNodeType.isEntrance ||
                currentRoom.roomNodeType.isChestRoom ||
                currentRoom.roomNodeType.isExit)
                return;

            // If the room is already clear of enemies, then return
            if (currentRoom.IsClearOfEnemies) return;

            StaticEventHandler.CallOnDisplayObjectivesEvent
                (Settings.DisplayObjectivesTime, 0f, 1f, $"Defeat 'em All!!");


            // Get a random number of enemies to spawn for this room
            enemiesToSpawn = currentRoom.GetNumberOfItemsToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

            // Get the enemy spawn parameters for this room
            roomEnemySpawnParemeters = currentRoom.GetRoomItemSpawnParameters(GameManager.Instance.GetCurrentDungeonLevel(), 1);

            // If no enemies to spawn, return and mark the room as cleared
            if (enemiesToSpawn == 0)
            {
                currentRoom.IsClearOfEnemies = true;
                return;
            }

            // Get the number of concurrent enemies to be spawn in this room
            maxConcurrentNumberOfEnemies = GetConcurrentEnemiesToSpawn();
            
            // Start locking the doors with a delay
            await currentRoom.InstantiatedRoom.LockDoorsAsync();
            
            if (currentRoom.battleMusic != null && !currentRoom.IsClearOfEnemies)
                MusicManager.CallOnMusicClipChangedEvent(currentRoom.battleMusic);

            // ... And actually spawn the enemies
            SpawnEnemies();
        }

        /// <summary>
        /// Spawns the enemies onto the current room
        /// </summary>
        private async void SpawnEnemies()
        {
            if (GameManager.CurrentGameState == GameState.BossStage)
            {
                GameManager.PreviousGameState = GameState.BossStage;
                GameManager.CurrentGameState = GameState.EngagingBoss;
            }
            else if (GameManager.CurrentGameState == GameState.Playing)
            {
                GameManager.PreviousGameState = GameState.Playing;
                GameManager.CurrentGameState = GameState.EngagingEnemies;
            }

            await SpawnEnemiesAsync();
        }

        /// <summary>
        /// Spawn the enemies coroutine
        /// </summary>
        private IEnumerator SpawnEnemiesRoutine()
        {
            Grid grid = currentRoom.InstantiatedRoom.grid;

            // Create an instance of the helper class used to select a random enemy
            RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject = new(currentRoom.EnemiesByLevelList);

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

                    // Creates the enemy and gets the next one to spawn
                    CreateEnemy(randomSpawnableObject.GetRandomItem(), grid.CellToWorld(cellPosition));

                    yield return new WaitForSeconds(GetEnemySpawnInterval());
                }
            }
        }

        private async UniTask SpawnEnemiesAsync()
        {
            Grid grid = currentRoom.InstantiatedRoom.grid;

            // Create an instance of the helper class used to select a random enemy
            RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject = new(currentRoom.EnemiesByLevelList);

            // See if we have space to spawn the enemies
            if (currentRoom.spawnPositionArray.Length > 0)
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    // Wait until the enemy count is less than the max concurrent enemies
                    while (currentEnemyCount >= maxConcurrentNumberOfEnemies)
                    {
                        await UniTask.NextFrame();
                    }

                    Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                    // Creates the enemy and gets the next one to spawn
                    CreateEnemy(randomSpawnableObject.GetRandomItem(), grid.CellToWorld(cellPosition));

                    await UniTask.Delay((int)GetEnemySpawnInterval());
                }
            }
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
            GameLevelSO gameLevel = GameManager.Instance.GetCurrentDungeonLevel();

            // Instantiate the enemy
            GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

            // Initialize the enemy parameters
            enemy.GetComponent<Enemy>().InitialiseEnemy(enemyDetails, enemiesSpawnedSoFar, gameLevel);

            // Suscribe to the destroy enemies event
            enemy.GetComponent<DestroyEvent>().OnDestroy += Enemy_OnDestroyed;
        }

        /// <summary>
        /// Destroy enemy event handler
        /// </summary>
        private async void Enemy_OnDestroyed(DestroyEvent destroyEvent, DestroyedEventArgs destroyedEventArgs)
        {
            // Unsuscribe
            destroyEvent.OnDestroy -= Enemy_OnDestroyed;

            //Reduce the enemy count
            currentEnemyCount--;
            enemiesKilledSoFar++;

            // Call the multiplier event here, because at some point the 
            // player will run out of ammo, but can still kill enemies when the
            // snake segments collides with an enemy
            StaticEventHandler.CallMultiplierEvent(true);

            if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
            {
                currentRoom.IsClearOfEnemies = true;

                // Set the state of the game
                if (GameManager.CurrentGameState == GameState.EngagingEnemies)
                {
                    GameManager.CurrentGameState = GameState.Playing;
                    GameManager.PreviousGameState = GameState.EngagingEnemies;
                }
                else if (GameManager.CurrentGameState == GameState.EngagingBoss)
                {
                    GameManager.CurrentGameState = GameState.BossStage;
                    GameManager.PreviousGameState = GameState.EngagingBoss;
                }

                // Unlock the doors
                await currentRoom.InstantiatedRoom.UnlockDoorsAsync(Settings.doorUnlockDelay);

                // Play the normal music again
                if (currentRoom.normalMusic != null)
                    MusicManager.CallOnMusicClipChangedEvent(currentRoom.normalMusic);

                // Trigger the static event to indicate the room is clear of enemies
                StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
            }
        }

        /// <summary>
        /// Gets a random interval spawn value in seconds
        /// </summary>
        private float GetEnemySpawnInterval()
        {
            return Random.Range(roomEnemySpawnParemeters.minSpawnInterval, roomEnemySpawnParemeters.maxSpawnInterval);
            //return Random.Range(enemySpawnParameters.minSpawnInterval, enemySpawnParameters.maxSpawnInterval);
        }

        /// <summary>
        /// Gets the number of enemies that can be present in a room at any given time.
        /// </summary>
        private int GetConcurrentEnemiesToSpawn()
        {
            return Random.Range(roomEnemySpawnParemeters.minConcurrentItems, roomEnemySpawnParemeters.maxConcurrentItems);
            //return Random.Range(enemySpawnParameters.minConcurrentItems, enemySpawnParameters.maxConcurrentItems);
        }
    }
}