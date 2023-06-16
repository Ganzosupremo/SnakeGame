using SnakeGame.ProceduralGenerationSystem;
using SnakeGame.AudioSystem;
using UnityEngine;

namespace SnakeGame.Decorations
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveableObstacle : MonoBehaviour
    {
        [SerializeField] private SoundEffectSO moveSound;
        [HideInInspector] public BoxCollider2D boxCollider2D;

        private Rigidbody2D rb;
        private InstantiatedRoom instantiatedRoom;
        private Vector3 previousPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();

            instantiatedRoom = GetComponentInParent<InstantiatedRoom>();
            instantiatedRoom.moveableItemsList.Add(this);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            UpdateObstacles();
        }

        private void UpdateObstacles()
        {
            StayWithinRoomBounds();

            instantiatedRoom.UpdateMoveableObstacles();
            previousPosition = transform.position;

            // Only play sound effect if moving
            if (Mathf.Abs(rb.velocity.x) > 0.001f || Mathf.Abs(rb.velocity.y) > 0.001f)
            {
                if (moveSound != null && Time.frameCount % 10 == 0)
                {
                    SoundEffectManager.CallOnSoundEffectChangedEvent(moveSound);
                }
            }
        }

        /// <summary>
        /// Don't allow the item to go out of the current room
        /// </summary>
        private void StayWithinRoomBounds()
        {
            Bounds itemBounds = boxCollider2D.bounds;
            Bounds roomBounds = instantiatedRoom.roomColliderBounds;

            if (itemBounds.min.x <= roomBounds.min.x ||
                itemBounds.max.x >= roomBounds.max.x ||
                itemBounds.min.y <= roomBounds.min.y ||
                itemBounds.max.y >= roomBounds.max.y)
            {
                transform.position = previousPosition;
            }
        }
    }
}