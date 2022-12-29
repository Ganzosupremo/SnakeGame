using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Header Dungeon Levels
    [Space(10)]
    [Header("The Levels Of The Dungeon")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with all the game level for this game")]
    #endregion
    [SerializeField] private List<GameLevelSO> gameLevelList;

    #region Tooltip
    [Tooltip("Populate with the starting dungeon level for testing, first dungeon level = 0")]
    #endregion
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState currentGameState;
    [HideInInspector] public GameState previousGameState;

    private void Start()
    {
        currentGameState = GameState.Started;
    }

    private void Update()
    {
        HandleGameStates();

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGameState = GameState.Started;
        }
    }

    /// <summary>
    /// Handles the state of the current playtrought using the enum <see cref="GameState"/>. 
    /// </summary>
    private void HandleGameStates()
    {
        switch (currentGameState)
        {
            case GameState.Started:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                currentGameState = GameState.Playing;
                break;
            case GameState.Playing:
                break;
            case GameState.EngagingEnemies:
                break;
            case GameState.BossStage:
                break;
            case GameState.EngagingBoss:
                break;
            case GameState.LevelCompleted:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            case GameState.Paused:
                break;
            case GameState.Restarted:
                break;
            default:
                break;
        }
    }

    private void PlayDungeonLevel(int currentDungeonLevelListIndex)
    {
        throw new NotImplementedException();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(gameLevelList), gameLevelList);
    }
#endif
    #endregion
}
