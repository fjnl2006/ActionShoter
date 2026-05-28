using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpEffect", menuName = "Scriptable Objects/PowerUpEffect")]
public abstract class PowerUpEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);
}
