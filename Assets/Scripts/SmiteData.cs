using UnityEngine;

[CreateAssetMenu(fileName = "SmiteData", menuName = "Scriptable Objects/SmiteData")]
public class SmiteData : ScriptableObject
{

    // Los ScripteableObject de tipo SmiteData nos permitirá crear distinotos tipos de Smites
    // con distintas características detrivadas de las siguientes variables

    public int Damage;          // Daño del proyectil
    public float ReloadingTime; // Tiempo de Recarga
    public float Range;         // Rango de alcance
    public float AreaRadius;    // Tamaño del area de efecto

}
