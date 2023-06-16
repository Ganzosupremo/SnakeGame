using SnakeGame.AudioSystem;
using SnakeGame.HealthSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private SoundEffectSO _LowHealthSoundEffect;

        private List<GameObject> healthHeartsList = new();

        private void OnEnable()
        {
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        }

        private void OnDisable()
        {
            GameManager.Instance.GetSnake().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            SpawnHeart(healthEventArgs);
        }

        private void ClearHearth()
        {
            foreach (GameObject heartIcon in healthHeartsList)
            {
                Destroy(heartIcon);
            }

            healthHeartsList.Clear();
        }

        private void SpawnHeart(HealthEventArgs healthEventArgs)
        {
            ClearHearth();

            // Display the health as n%
            int healthPercent = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f);

            GameObject heart = Instantiate(GameResources.Instance.healthPrefab, transform);
            heart.transform.localPosition = new Vector3(6f, 0f, 0f);

            heart.GetComponentInChildren<TextMeshProUGUI>().text = healthPercent + "%";
            healthHeartsList.Add(heart);
        }
    }
}