using SnakeGame.GameUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.AudioSystem
{
    [CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sound System/Music Track")]
    public class MusicSO : ScriptableObject
    {
        #region Header Music Track Details
        [Space(10)]
        [Header("Music Settings")]
        #endregion

        public string musicName;

        public AudioClip musicClip;

        [Range(0f, 1f)]
        public float musicVolume = 1f;


        #region Tooltip
        [Tooltip("Optional. You can populate the array with multiple clips and the MusicManager will pick one at random. The two arrays must have the same amount of entries.")]
        #endregion
        public AudioClip[] MusicClips;

        #region Tooltip
        [Tooltip("Optional. You can populate the array with the names of multiple clips in the same order" +
            " as the clips. This is used to show the music name on the UI. The two arrays must have the same amount of entries.")]
        #endregion
        public string[] MusicNames;

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
}