using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/DamageBuff")]
public class DamageBuff : PowerUpEffect
{
    public int amount;
    
    public override void Apply(GameObject target)
    {
        Debug.Log(amount);
        target.GetComponentInChildren<GunController>().Damage += amount;
    }
}
