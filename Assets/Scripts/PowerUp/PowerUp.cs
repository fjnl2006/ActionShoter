using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpEffect powerUpEffect;
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Destroy(this.gameObject);
        powerUpEffect.Apply(other.gameObject);
    }
}