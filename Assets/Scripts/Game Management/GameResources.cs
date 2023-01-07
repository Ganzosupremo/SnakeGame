using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    #region Dungeon
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("This Must Be Populated With The Dungeon RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;
    
    #region Tooltip
    [Tooltip("The current player Scriptable object - this is used to reference the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header Game Audio
    [Space(10)]
    [Header("Game Music")]
    #endregion
    #region Tooltip
    [Tooltip("The master mixer group of the music")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;

    #region Tooltip
    [Tooltip("The music full snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnFull;

    #region Tooltip
    [Tooltip("The low music snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnLow;

    #region Tooltip
    [Tooltip("The music off snapshot")]
    #endregion
    public AudioMixerSnapshot musicOff;

    #region Header Sound Management
    [Space(10)]
    [Header("Sound Management")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the SFX master group on the audio mixer")]
    #endregion
    public AudioMixerGroup soundMasterMixerGroup;

    #region Tooltip
    [Tooltip("The sound effects for the door")]
    #endregion
    public SoundEffectSO doorSoundEffect;

    #region Header Materials
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Materials")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("The Default Sprite Lit Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with tha variable lit shader")]
    #endregion
    public Shader variableLitShader;

    #region Header UI
    [Space(10)]
    [Header("Game UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the AmmoIcon prefab")]
    #endregion
    public GameObject ammoIconPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);

        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFull), musicOnFull);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnLow), musicOnLow);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOff), musicOff);

        HelperUtilities.ValidateCheckNullValue(this, nameof(soundMasterMixerGroup), soundMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorSoundEffect), doorSoundEffect);
    }
#endif
    #endregion
}
