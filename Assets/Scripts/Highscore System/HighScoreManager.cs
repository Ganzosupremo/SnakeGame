using SnakeGame.Interfaces;
using SnakeGame.SaveAndLoadSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.HighscoreSystem
{
    public class HighScoreManager : SingletonMonoBehaviour<HighScoreManager>, IPersistenceData
    {
        private HighScore m_HighScores = new();

        protected override void Awake()
        {
            base.Awake();
            SaveDataManager.Instance.LoadGame();
        }

        /// <summary>
        /// Adds the current score to the high score list
        /// </summary>
        public void AddScore(Score score, int rank)
        {
            m_HighScores.ScoreList.Insert(rank - 1, score);

            //Maintain the max number of scores that can be saved
            if (m_HighScores.ScoreList.Count > Settings.maxNumberOfHighScoresToSave)
            {
                m_HighScores.ScoreList.RemoveAt(Settings.maxNumberOfHighScoresToSave);
            }

            SaveDataManager.Instance.SaveGame();
        }

        /// <summary>
        /// Gets the list of high scores saved to the disk
        /// </summary>
        /// <returns>Returns High Score class containing the list of current scores</returns>
        public HighScore GetHighScores()
        {
            return m_HighScores;
        }

        /// <summary>
        /// Compares the passed playerScore to the other high scores
        /// and returns the rank of tha score.
        /// </summary>
        /// <returns>Returns 0 if the current score is not higher than any of the previous scores.</returns>
        public int GetRank(long playerScore)
        {
            //If there are no high scores this should be at the top
            if (m_HighScores.ScoreList.Count == 0) return 1;

            int index = 0;

            //Loop to find the rank of this score
            for (int i = 0; i < m_HighScores.ScoreList.Count; i++)
            {
                index++;

                if (playerScore >= m_HighScores.ScoreList[i].PlayerScore)
                {
                    return index;
                }
            }

            if (m_HighScores.ScoreList.Count < Settings.maxNumberOfHighScoresToSave)
                return index + 1;

            return 0;
        }

        public void Load(GameData data)
        {
            m_HighScores = data.HighScores;
        }

        public void Save(GameData data)
        {
            data.HighScores = m_HighScores;
        }
    }
}
