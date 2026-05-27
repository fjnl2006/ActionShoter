using UnityEngine;

[CreateAssetMenu(fileName = "LaserData", menuName = "Scriptable Objects/LaserData")]
public class LaserData : ScriptableObject
{
    public int Damage;          // Daño por golpe
    public float ReloadTime;    // Tiempo de espera para recargar la municion
    public float Range;         // Rango de alcance del laser
    public float Radius;        // Ancho del Laser
    public float FireRating;    // Tiempo de espera entre disparos
    public int MaxAmo;          // Munición máxima 
    public int AmoCost;         // Coste de lanzar un laser
}
