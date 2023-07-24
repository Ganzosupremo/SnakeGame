using SnakeGame.GameUtilities;
using SnakeGame.PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SnakeGame
{
    public class PlayerSettingsUI : MonoBehaviour
    {
        #region Tooltip
        [Tooltip("Populate with the Input Field")]
        #endregion
        [SerializeField] private TMP_InputField _PlayerNameInput;

        private CurrentPlayerSO currentSnake;
        private void Awake()
        {
            //Load resources
            //playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
            //playerDetailsList = GameResources.Instance.playerDetailsList;
            currentSnake = GameResources.Instance.currentSnake;
        }
        
        public void UpdatePlayerName()
        {
            _PlayerNameInput.text = _PlayerNameInput.text.ToUpper();

            currentSnake.snakeName = _PlayerNameInput.text;
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(_PlayerNameInput), _PlayerNameInput);
        }
#endif
        #endregion

    }
}
