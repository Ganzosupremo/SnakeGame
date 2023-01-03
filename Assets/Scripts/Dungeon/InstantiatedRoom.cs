using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundtilemap;
    [HideInInspector] public Tilemap decorations1Tilemap;
    [HideInInspector] public Tilemap decorations2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty; // This is used to store the movement penalties for the AStar Pathfinding
    [HideInInspector] public int[,] aStarItemObstacles; // Store the position of moveable items which acts as an obstacle

    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag) && room != GameManager.Instance.GetCurrentRoom())
        {
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    /// <summary>
    /// Initialise The Instantiated Rooms.
    /// </summary>
    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorways();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// Populate The Grid And Tilemap Variables Specified Above.
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        //Get the grid component
        grid = roomGameobject.GetComponentInChildren<Grid>();

        //Get the tilemap component in the children
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.CompareTag("Tilemap_Ground"))
            {
                groundtilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("Tilemap_Decorations1"))
            {
                decorations1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("Tilemap_Decorations2"))
            {
                decorations2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("Tilemap_Front"))
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("Tilemap_Collision"))
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("Tilemap_Minimap"))
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Blocks Off Unused Doorways Of The Room
    /// </summary>
    private void BlockOffUnusedDoorways()
    {
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
                continue;

            if (groundtilemap != null)
                BlockADoorwayOnTilemap(groundtilemap, doorway);
            if (decorations1Tilemap != null)
                BlockADoorwayOnTilemap(decorations1Tilemap, doorway);
            if (decorations2Tilemap != null)
                BlockADoorwayOnTilemap(decorations2Tilemap, doorway);
            if (frontTilemap != null)
                BlockADoorwayOnTilemap(frontTilemap, doorway);
            if (collisionTilemap != null)
                BlockADoorwayOnTilemap(collisionTilemap, doorway);
            if (minimapTilemap != null)
                BlockADoorwayOnTilemap(minimapTilemap, doorway);
        }
    }

    /// <summary>
    /// Blocks The Doorway On The Corresponding Tilemap
    /// </summary>
    private void BlockADoorwayOnTilemap(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.doorOrientation)
        {
            case Orientation.North:
            case Orientation.South:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.East:
            case Orientation.West:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.None:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Blocks the doorways horizontally
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int copyStartPosition = doorway.doorwayStartCopyPosition;

        // loop through all the tiles in x position to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            // loop through all tiles in y position to copy
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // Get the rotation of the tile that is gonna be copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0));

                // Copy the tile
                tilemap.SetTile(new Vector3Int(copyStartPosition.x + 1 + xPos, copyStartPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0)));

                // Set the correct rotation of the copied tile
                tilemap.SetTransformMatrix(new Vector3Int(copyStartPosition.x + 1 + xPos, copyStartPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Blocks the doorways vertically
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int copyStartPosition = doorway.doorwayStartCopyPosition;

        // loop through all the tiles in y position to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            // loop through all tiles in x position to copy
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Get the rotation of the tile that is gonna be copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos,
                    copyStartPosition.y - yPos, 0));

                //Copy the tile
                tilemap.SetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - 1 - yPos, 0),
                    tilemap.GetTile(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - yPos, 0)));

                // Set the correct rotation of the copied tile
                tilemap.SetTransformMatrix(new Vector3Int(copyStartPosition.x + xPos, copyStartPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Adds the doorways to the instantiated room
    /// </summary>
    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;

        // Instantiate door prefabs at the doorway position
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                switch (doorway.doorOrientation)
                {
                    case Orientation.North:
                        door = Instantiate(doorway.doorPrefab, gameObject.transform);
                        // Position the door correctly on the map
                        door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2, 
                            doorway.doorPosition.y + tileDistance, 0f);
                        break;
                    case Orientation.East:
                        door = Instantiate(doorway.doorPrefab, gameObject.transform);
                        // Position the door correctly on the map
                        door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance, 
                            doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                        break;
                    case Orientation.South:
                        door = Instantiate(doorway.doorPrefab, gameObject.transform);
                        // Position the door correctly on the map
                        door.transform.localPosition = new Vector3(doorway.doorPosition.x + tileDistance / 2, doorway.doorPosition.y, 0f);
                        break;
                    case Orientation.West:

                        door = Instantiate(doorway.doorPrefab, gameObject.transform);
                        // Position the door correctly on the map
                        door.transform.localPosition = new Vector3(doorway.doorPosition.x, 
                            doorway.doorPosition.y + tileDistance * 1.25f, 0f);
                        break;
                    default:
                        break;
                }

                Door doorComponent = door.GetComponent<Door>();

                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    // Prevent access until all enemies in the other rooms have been cleared
                    doorComponent.LockDoor();
                }
            }
        }
    }

    /// <summary>
    /// Disable The Collision Tilemap Renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
