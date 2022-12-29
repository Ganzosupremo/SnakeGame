using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This is the template for the room that the dungeon builder will use to build the level
/// </summary>
[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion Header ROOM PREFAB
    #region Tooltip

    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]

    #endregion Tooltip
    public GameObject prefab;


    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the guid if the SO is copied and the prefab is changed

    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion Header ROOM CONFIGURATION
    #region Tooltip
    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exceptions being with corridors. In the room node graph there is just one corridor type 'Corridor'. For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]
    #endregion Tooltip
    public RoomNodeTypeSO roomNodeType;

    #region Tooltip
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position")]
    #endregion Tooltip
    public Vector2Int lowerBounds;

    #region Tooltip
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position")]
    #endregion Tooltip
    public Vector2Int upperBounds;

    #region Tooltip
    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'")]
    #endregion Tooltip
    public List<Doorway> doorwayList;

    #region Tooltip
    [Tooltip("Each possible spawn position (this is used for enemies and chests) for the room in local coordinates should be added to this array")]
    #endregion Tooltip
    public Vector2Int[] spawnPositionArray;
    #region Tooltip
    [Tooltip("This array is used to spawn food on the rooms")]
    #endregion Tooltip
    public Vector2Int[] foodSpawnPositionArray;

    /// <summary>
    /// Returns the list of Entrances for the room template
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation - Implemet later
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);
    }
#endif
    #endregion
}
