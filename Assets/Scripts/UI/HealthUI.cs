using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
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
        SetHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach (GameObject heartIcon in healthHeartsList)
        {
            Destroy(heartIcon);
        }

        healthHeartsList.Clear();
    }

    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        // Display the health as n%
        int healthHearts = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f);

        GameObject heart = Instantiate(GameResources.Instance.healthPrefab, transform);

        heart.GetComponentInChildren<TextMeshProUGUI>().text = healthHearts + "%";
        healthHeartsList.Add(heart);

        //for (int i = 0; i < healthHearts; i++)
        //{
        //    //Instantiate the heart Icon prefab
        //    GameObject heart = Instantiate(GameResources.Instance.healthPrefab, transform);

        //    //Set correctly the position of the hearts in the UI
        //    heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);

        //    healthHeartsList.Add(heart);
        //}
    }
}
