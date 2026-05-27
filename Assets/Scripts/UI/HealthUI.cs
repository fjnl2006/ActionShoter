using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healtText;
    
    public void UpdateHealt(int current, int max)
    {
        healtText.text = current + " / " + max;
    }
}
