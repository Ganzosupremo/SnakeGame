using UnityEngine;

[System.Serializable]
public class Doorway
{
    public Vector2Int doorPosition;
    public Orientation doorOrientation;
    public GameObject doorPrefab;
    #region Header
    [Header("The Upper Left Position To Start Copying From")]
    #endregion
    #region Tooltip
    [Tooltip("The position to start copying the tiles from, always measure at the upper left corner one tile before the door's corridor begins")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("The width of tiles in the doorway to copy over")]
    #endregion
    #region Tooltip
    [Tooltip("The width of the tiles that will be copied over, NOTE: use always the coordinate brush")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("The height of tiles in the doorway to copy over")]
    #endregion
    #region Tooltip
    [Tooltip("The height of the tiles that will be copied over, NOTE: use always the coordinate brush")]
    #endregion
    public int doorwayCopyTileHeight;
    [HideInInspector] public bool isConnected = false;
    [HideInInspector] public bool isUnavailable = false;
}
