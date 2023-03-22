using TMPro;
using UnityEngine;

namespace SnakeGame.HighscoreSystem
{
    public class ScorePrefab : MonoBehaviour
    {
        public TextMeshProUGUI RankText;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI ScoreText;

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(RankText), RankText);
            HelperUtilities.ValidateCheckNullValue(this, nameof(NameText), NameText);
            HelperUtilities.ValidateCheckNullValue(this, nameof(LevelText), LevelText);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ScoreText), ScoreText);
        }
#endif
        #endregion
    }
}
