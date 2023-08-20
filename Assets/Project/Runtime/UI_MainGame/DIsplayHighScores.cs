using SnakeGame.HighscoreSystem;
using UnityEngine;

namespace SnakeGame.UI
{
    public class DIsplayHighScores : MonoBehaviour
    {
        [Tooltip("The Content transform of the scroll view gameobject.")]
        [SerializeField] private Transform ContentAnchorTransform;

        private void Start()
        {
            DisplayScores();
        }

        private void DisplayScores()
        {
            HighScore highScores = HighScoreManager.Instance.GetHighScores();
            GameObject scoreGameObject;

            int rank = 0;
            foreach (Score score in highScores.ScoreList)
            {
                rank++;

                scoreGameObject = Instantiate(GameResources.Instance.ScorePrefab, ContentAnchorTransform);

                ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

                //Populate the text fields in the score prefab
                scorePrefab.RankText.text = rank.ToString();
                scorePrefab.NameText.text = score.PlayerName;
                scorePrefab.LevelText.text = score.LevelDescription;
                scorePrefab.ScoreText.text = score.PlayerScore.ToString("###,###,###0");
            }
        }
    }
}
