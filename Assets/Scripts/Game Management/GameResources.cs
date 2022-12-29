using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get 
        {
            if (instance == null)
                instance = Resources.Load<GameResources>("GameResources");
            return instance;
        }
    }

    [Header("DUNGEON")]
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
    }
#endif
    #endregion
}
