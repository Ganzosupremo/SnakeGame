using SnakeGame.GameUtilities;
using SnakeGame.ProceduralGenerationSystem;
using UnityEngine;

namespace SnakeGame.Decorations
{
    public class Decoration : MonoBehaviour
    {
        #region Header References
        [Header("REFERENCES")]
        #endregion
        [Tooltip("Populate with the sprite renderer of the prefab")]
        public SpriteRenderer spriteRenderer;
        
        [Tooltip("Populate with the Environment gameobject of the room prefab")]
        public GameObject EnvironmentParentObject;

        public BoxCollider2D BoxCollider2D;

        private InstantiatedRoom _InstantiatedRoom;

        private void Awake()
        {
            _InstantiatedRoom = EnvironmentParentObject.GetComponentInParent<InstantiatedRoom>();
        }

        private void Start()
        {
            if (_InstantiatedRoom.room.roomNodeType.isEntrance ||
                _InstantiatedRoom.room.roomNodeType.isCorridorEW ||
                _InstantiatedRoom.room.roomNodeType.isCorridorNS ||
                _InstantiatedRoom.room.roomNodeType.isExit ||
                _InstantiatedRoom.room.roomNodeType.isCorridor)
                return;

            _InstantiatedRoom.StaticDecorations.Add(this);
            _InstantiatedRoom.UpdateStaticObstaclesArray();
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
            HelperUtilities.ValidateCheckNullValue(this, nameof(EnvironmentParentObject), EnvironmentParentObject);
        }
#endif
        #endregion
    }
}
