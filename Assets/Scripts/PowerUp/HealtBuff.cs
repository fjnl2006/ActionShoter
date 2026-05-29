using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]
public class HealtBuff : PowerUpEffect
{
    
    public int amount;

    

    public override void Apply(GameObject target)
    {
        Debug.Log(amount);
        var player = target.GetComponentInChildren<PlayerController>();
        if (player != null)
        {
            player.Heal(amount);
        }
        else
        {
            Debug.LogWarning("PlayerController not found on target");
        }
    }
}
