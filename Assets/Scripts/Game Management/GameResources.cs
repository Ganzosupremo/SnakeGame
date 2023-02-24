using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using SnakeGame.SoundsSystem;
using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;
using UnityEngine.Rendering;

namespace SnakeGame
{
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
        [Header("DUNGEON")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("This Must Be Populated With The Dungeon RoomNodeTypeListSO")]
        #endregion
        public RoomNodeTypeListSO roomNodeTypeList;

        #region Tooltip
        [Tooltip("The current player Scriptable object - this is used to reference the current player between scenes")]
        #endregion
        public CurrentPlayerSO currentSnake;

        public VolumeProfile mainGameData;

        #region Snake Mechanics
        [Header("SNAKE MECHANICS")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The food that will spawn randomly within the room borders")]
        #endregion
        public GameObject foodPrefab;

        #region Tooltip
        [Tooltip("The snake segement that will grow when the snake eats the food")]
        #endregion
        public Transform snakeBodyPrefab;

        #region Header Game Audio
        [Header("AUDIO MANAGEMENT")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The master mixer group of the music")]
        #endregion
        public AudioMixerGroup musicMasterMixerGroup;

        public AudioMixerGroup minigunFireMixerGroup;

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

        #region Tooltip
        [Tooltip("Populate with the SFX master group on the audio mixer")]
        #endregion
        public AudioMixerGroup soundMasterMixerGroup;

        #region Tooltip
        [Tooltip("The sound effects for the door")]
        #endregion
        public SoundEffectSO doorSoundEffect;

        #region Header Materials
        [Header("MATERIALS")]
        [Space(10)]
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

        #region Header Special Tilemap Tiles
        [Header("SPECIAL TILES")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("The Collision tiles the enemies can't go to")]
        #endregion
        public TileBase[] enemyUnwalkableCollisionTilesArray;

        #region Tooltip
        [Tooltip("The prefered path tile for the enemy navigation")]
        #endregion
        public TileBase preferredEnemyPathTile;

        #region Header UI
        [Header("GAME UI")]
        [Space(10)]
        #endregion
        #region Tooltip
        [Tooltip("Populate with the AmmoIcon prefab")]
        #endregion
        public GameObject ammoIconPrefab;
        public GameObject bossRoomCuePrefab;

        #region Tooltip
        [Tooltip("Populate with the Health prefab")]
        #endregion
        public GameObject healthPrefab;

        #region Header Difficulty
        [Header("DIFFICULTY SETTINGS")]
        [Space(10)]
        #endregion
        public List<EnemyDetailsSO> enemyDetailsList;
        public List<SnakeDetailsSO> snakeDetailsList;
        #region Tooltip
        [Tooltip("The light intensity of the global light will be adjusted" +
            " depending on the selected difficulty")]
        #endregion
        public Light2D globalLight;

        /// <summary>
        /// Set the intensity of the global light, the Game Manager gets the light reference from here.
        /// We do this here because on the Main Menu the Game Manager does not exits,
        /// The Difficulty can only be changed on the Main Menu and changing the difficulty also changes the
        /// intensity of the global light.
        /// </summary>
        /// <param name="intensity"></param>
        public void SetLightIntensity(float intensity)
        {
            globalLight.intensity = intensity;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
            HelperUtilities.ValidateCheckNullValue(this, nameof(currentSnake), currentSnake);
            HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
            HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
            HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
            HelperUtilities.ValidateCheckNullValue(this, nameof(bossRoomCuePrefab), bossRoomCuePrefab);
            HelperUtilities.ValidateCheckNullValue(this, nameof(globalLight), globalLight);

            HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
            HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFull), musicOnFull);
            HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnLow), musicOnLow);
            HelperUtilities.ValidateCheckNullValue(this, nameof(musicOff), musicOff);

            HelperUtilities.ValidateCheckNullValue(this, nameof(soundMasterMixerGroup), soundMasterMixerGroup);
            HelperUtilities.ValidateCheckNullValue(this, nameof(doorSoundEffect), doorSoundEffect);

            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
            HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        }
#endif
        #endregion
    }
}