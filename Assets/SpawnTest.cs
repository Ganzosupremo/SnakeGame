using System.Collections.Generic;
using UnityEngine;
using SnakeGame.Utilities;
using SnakeGame;
using SnakeGame.Enemies;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> testList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemy;
    private List<GameObject> instantiatedEnemyList = new();

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
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplate != null)
        {
            testList = roomTemplate.enemiesByLevelList;
            randomEnemy = new RandomSpawnableObject<EnemyDetailsSO>(testList);

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailsSO enemyDetails = randomEnemy.GetRandomItem();

            if (enemyDetails != null)
            {
                instantiatedEnemyList.Add(Instantiate(enemyDetails.enemyPrefab, 
                    HelperUtilities.GetNearestSpawnPointPosition(HelperUtilities.GetMouseWorldPosition()),
                    Quaternion.identity));
            }
        }
    }
}
