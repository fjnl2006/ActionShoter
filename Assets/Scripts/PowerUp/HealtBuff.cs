using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]
public class HealtBuff : PowerUpEffect
{
    public int amount;
    
    public override void Apply(GameObject target)
    {
        Debug.Log(amount);
        target.GetComponentInChildren<PlayerController>().currentHealth += amount;
    }
}
