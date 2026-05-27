using System;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    /// Los ScripteableObject de tipo GunData nos permitirá crear distinotos tipos de pistolas
    /// con distintas características detrivadas de las siguientes variables

    public int Damage;          // Daño del arma
    public float ReloadTime;    // Tiempo de recarga
    public float Range;         // Rango del arma
    public float FireRating;    // Tiempo entre disparos
    public int MaxAmo;          // Municion máxima 
}
