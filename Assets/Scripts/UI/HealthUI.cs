using System;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healtText;
    public static HealthUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealt(int current, int max)
    {
        healtText.text = current + " / " + max;
    }
}
