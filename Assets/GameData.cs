using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public Vector3 playerPosition;
    public float playerSpeedMultiplier = 1f;

    public float enemySpeedMultiplier = 1f;


    public bool playerSpeedCollected = false;
    public bool enemySlowCollected = false;

    public void Reset()
    {
        playerPosition = Vector3.zero;
        playerSpeedMultiplier = 1f;
        enemySpeedMultiplier = 1f;
        playerSpeedCollected = false;
        enemySlowCollected = false;
    }
}
