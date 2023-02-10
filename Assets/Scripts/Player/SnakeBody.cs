using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SnakeBody : MonoBehaviour
{
    public Sprite[] bodySprites;
    private SpriteRenderer spriteRenderer;
    private Room room;
    private InstantiatedRoom instantiatedRoom;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    [HideInInspector] public Bounds segmentBounds;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        segmentBounds = boxCollider2D.bounds;

        //room = GameManager.Instance.GetCurrentRoom();
        //instantiatedRoom = room.instantiatedRoom;
    }

    private void Start()
    {
        if (bodySprites.Length != 0)
        {
            // Set a random sprite for diferent parts of the snake body
            int randomSprite = Random.Range(0, bodySprites.Length);
            spriteRenderer.sprite = bodySprites[randomSprite];
        }
    }

    //private void FixedUpdate()
    //{
    //    UpdateSegmentObstacles();
    //}

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
        //UpdateSegmentObstacles(roomChangedEventArgs);
    }

    /// <summary>
    /// Makes the enemies consider the snake segments as obstacles
    /// so the enemies will try to avoid them and look for other path to the player
    /// </summary>
    private void UpdateSegmentObstacles()
    {
        room = GameManager.Instance.GetCurrentRoom();
        instantiatedRoom = room.instantiatedRoom;
        Grid grid = instantiatedRoom.grid;
        instantiatedRoom.UpdateSnakeSegmenstObstacles();

        Vector3Int minColliderBounds = grid.WorldToCell(boxCollider2D.bounds.min);
        Vector3Int maxColliderBounds = grid.WorldToCell(boxCollider2D.bounds.max);

        //Loop through and add moveable item colliders bounds to the obstacle array
        for (int i = minColliderBounds.x; i <= maxColliderBounds.x; i++)
        {
            for (int j = minColliderBounds.y; j <= maxColliderBounds.y; j++)
            {
                instantiatedRoom.aStarSnakeSegmentsObstacles[i - room.tilemapLowerBounds.x, j - room.tilemapLowerBounds.y] = 0;
            }
        }
    }
}
