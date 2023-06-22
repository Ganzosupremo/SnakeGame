using SnakeGame.GameUtilities;
using SnakeGame.ProceduralGenerationSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class ActivateRooms : MonoBehaviour
    {
        #region Header Put the Minimap Camera Here
        [Header("The Minimap Camera")]
        #endregion
        [SerializeField] private Camera minimapCamera;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            InvokeRepeating(nameof(EnableRooms), 0.5f, 0.75f);
        }

        private void EnableRooms()
        {
            //If the current game state is dungeonOverviewMap, don't process any room activation
            if (GameManager.CurrentGameState == GameState.OverviewMap) return;

            HelperUtilities.CameraWorldPositionBounds(out Vector2Int minimapWorldPositionLowerBounds,
                out Vector2Int minimapWorldPositionUpperBounds, minimapCamera);

            HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds,
                out Vector2Int mainCameraWorldPositionUpperBounds, mainCamera);

            // Iterate through all the dungeon rooms to see if there are in the viewport of the camera
            foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.DungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                // If the room is within the viewport bounds, then activate the room
                if ((room.lowerBounds.x <= minimapWorldPositionUpperBounds.x && room.lowerBounds.y <= minimapWorldPositionUpperBounds.y) &&
                    (room.upperBounds.x >= minimapWorldPositionLowerBounds.x && room.upperBounds.y >= minimapWorldPositionLowerBounds.y))
                {
                    room.InstantiatedRoom.gameObject.SetActive(true);

                    //If the room is within the main camera viewport activate the environment objects
                    if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y) &&
                    (room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                    {
                        room.InstantiatedRoom.ActivateEnvironmentObjects();
                    }
                    else
                    {
                        room.InstantiatedRoom.DeactivateEnvironmentObjects();
                    }
                }
                else
                {
                    room.InstantiatedRoom.gameObject.SetActive(false);
                }
            }
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(minimapCamera), minimapCamera);
        }
#endif
        #endregion
    }
}