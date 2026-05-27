using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoText;
    
        public void UpdateAmmo(int current, int max)
        {
            ammoText.text = current + " / " + max;
        }
}
