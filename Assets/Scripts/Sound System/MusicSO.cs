using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sound System/Music Track")]
public class MusicSO : ScriptableObject
{
    #region Header Music Track Details
    [Space(10)]
    [Header("Music Track Details")]
    #endregion

    public string musicName;

    public AudioClip musicClip;

    [Range(0f, 1f)]
    public float musicVolume = 1f;

    public int musicIndex;

    //public int GetRandomMusic(int index)
    //{
    //    musicIndex = Random.Range(0, musicClip.Length);

    //    if (musicIndex == index)
    //    {
    //        return musicIndex;
    //    }
    //    else
    //    {
    //        return index;
    //    }
    //}

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
    #endregion
}
