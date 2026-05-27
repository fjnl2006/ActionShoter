using UnityEngine;

public interface IWeapon
{
    void Shoot(EnemyController target);

    void Reload();

    float GetRange();

    void SwitchWeapon();
    
}