using TMPro;
using UnityEngine;

namespace SnakeGame.HealthSystem
{
    public class HealthBar : MonoBehaviour
    {
        #region Header Game Object References
        [Space(10)]
        [Header("Game Object References")]
        #endregion
        [SerializeField] private GameObject healthBarContainer;
        [SerializeField] private TextMeshPro _bossName;

        /// <summary>
        /// Enables the health bar
        /// </summary>
        public void EnableHealthBar()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables the health bar
        /// </summary>
        public void DisableHealthBar()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Set the value of the health bar, btw 0 and 1 
        /// </summary>
        /// <param name="healthPercent"></param>
        public void SetHealthBarValue(float healthPercent)
        {
            healthBarContainer.transform.localScale = new Vector3(healthPercent, 1f, 0f);
        }

        public void SetBossName(string  bossName)
        {
            if (_bossName != null)
                _bossName.text = bossName;
        }
    }
}

