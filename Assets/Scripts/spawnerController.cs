using UnityEngine;
using System.Collections.Generic;

public class spawnerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject player;
    [SerializeField] float spawnRange;

    [Header("Spawn entre enemigos")]
    [SerializeField] float minFreq = 0.5f;
    [SerializeField] float maxFreq = 1.5f;

    [Header("Oleadas")]
    [SerializeField] int startingEnemiesPerWave = 5;
    [SerializeField] int enemiesPerWaveIncrease = 2;
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] bool waitUntilWaveCleared = true;
    [SerializeField] bool startFirstWaveImmediately = true;

    [Header("Dificultad por oleada")]
    [SerializeField] int baseEnemyHealth = 30;
    [SerializeField] int healthIncreasePerWave = 5;
    [SerializeField] bool decreaseSpawnInterval = true;
    [SerializeField] float spawnIntervalDecreasePerWave = 0.1f;

    int currentWaveNumber;
    int enemiesToSpawnThisWave;
    int enemiesSpawnedThisWave;
    float timeUntilNextWave;
    bool isCurrentlySpawning;
    float spawnTimer;
    float currentSpawnMinDelay;
    float currentSpawnMaxDelay;

    readonly List<GameObject> activeWaveEnemies = new List<GameObject>();

    public int CurrentWave => currentWaveNumber;

    void Start()
    {
        enemiesToSpawnThisWave = startingEnemiesPerWave;
        currentSpawnMinDelay = minFreq;
        currentSpawnMaxDelay = maxFreq;
        timeUntilNextWave = startFirstWaveImmediately ? 0f : timeBetweenWaves;
    }

    void Update()
    {
        UpdateWaveTimer();

        if (isCurrentlySpawning)
            UpdateEnemySpawning();
    }

    void UpdateWaveTimer()
    {
        if (isCurrentlySpawning)
            return;

        if (waitUntilWaveCleared && CountLivingWaveEnemies() > 0)
            return;

        if (timeUntilNextWave > 0f)
        {
            timeUntilNextWave -= Time.deltaTime;
            return;
        }

        StartNewWave();
    }

    void StartNewWave()
    {
        currentWaveNumber++;
        enemiesSpawnedThisWave = 0;
        isCurrentlySpawning = true;
        spawnTimer = 0f;
        activeWaveEnemies.Clear();

        Debug.Log($"Oleada {currentWaveNumber}: {enemiesToSpawnThisWave} enemigos, {GetHealthForWave(currentWaveNumber)} HP cada uno.");
    }

    void UpdateEnemySpawning()
    {
        if (spawnTimer > 0f)
        {
            spawnTimer -= Time.deltaTime;
            return;
        }

        if (enemiesSpawnedThisWave >= enemiesToSpawnThisWave)
        {
            EndCurrentWave();
            return;
        }

        if (enemyPrefab == null || player == null)
        {
            Debug.LogWarning("Spawner: falta enemyPrefab o player.");
            EndCurrentWave();
            return;
        }

        SpawnSingleEnemy();
        enemiesSpawnedThisWave++;
        spawnTimer = Random.Range(currentSpawnMinDelay, currentSpawnMaxDelay);
    }

    void SpawnSingleEnemy()
    {
        Vector3 spawnOffset = Random.insideUnitSphere * spawnRange;
        Vector3 spawnPosition = transform.position + spawnOffset;
        spawnPosition.y = transform.position.y;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeWaveEnemies.Add(newEnemy);

        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetTarget(player);
            enemyController.InitializeForWave(GetHealthForWave(currentWaveNumber));
        }
        else
        {
            Debug.LogWarning("Enemigo spawneado sin EnemyController.");
        }
    }

    int GetHealthForWave(int wave)
    {
        return baseEnemyHealth + (wave - 1) * healthIncreasePerWave;
    }

    int CountLivingWaveEnemies()
    {
        activeWaveEnemies.RemoveAll(e => e == null);
        return activeWaveEnemies.Count;
    }

    void EndCurrentWave()
    {
        isCurrentlySpawning = false;
        timeUntilNextWave = timeBetweenWaves;

        if (decreaseSpawnInterval)
        {
            currentSpawnMinDelay = Mathf.Max(0.1f, currentSpawnMinDelay - spawnIntervalDecreasePerWave);
            currentSpawnMaxDelay = Mathf.Max(0.1f, currentSpawnMaxDelay - spawnIntervalDecreasePerWave);
        }

        enemiesToSpawnThisWave += enemiesPerWaveIncrease;

        int nextHealth = GetHealthForWave(currentWaveNumber + 1);
        Debug.Log($"Oleada {currentWaveNumber} completada. Siguiente: {enemiesToSpawnThisWave} enemigos, {nextHealth} HP.");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);

        if (isCurrentlySpawning)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnRange * 0.9f);
        }
    }
}
