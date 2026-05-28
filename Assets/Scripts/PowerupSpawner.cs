using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class PowerupSpawner : MonoBehaviour
{
    [Header("Power Ups")]
    public PowerUpEntry[] powerUpEntries;

    [Header("Distancias")]
    public float minDistance = 8f;    // No spawnea muy cerca del jugador
    public float maxDistance = 40f;   // No spawnea muy lejos (fuera de vista)
    public float despawnDistance = 60f; // Se destruye si el jugador se aleja mucho

    [Header("Límites")]
    public int maxActivePowerUps = 15;
    public float spawnInterval = 4f;
    public int maxAttemptsPerFrame = 10;

    [Header("Requisitos de terreno")]
    public bool requireNavMesh = true;
    public LayerMask groundLayer;
    public float groundCheckDistance = 5f;

    private Transform player;
    private List<GameObject> activePowerUps = new();

    [System.Serializable]
    public struct PowerUpEntry
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float weight;
    }

    // ─────────────────────────────────────────
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnLoop());
        StartCoroutine(DespawnLoop());
    }

    // ─────────────────────────────────────────
    // LOOP PRINCIPAL DE SPAWN
    // ─────────────────────────────────────────
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            activePowerUps.RemoveAll(p => p == null);

            if (activePowerUps.Count < maxActivePowerUps)
            {
                Vector3 pos = FindValidPosition();
                if (pos != Vector3.zero)
                    SpawnPowerUp(pos);
            }
        }
    }

    // ─────────────────────────────────────────
    // LOOP DE LIMPIEZA (si el jugador se aleja)
    // ─────────────────────────────────────────
    IEnumerator DespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval * 2f);

            for (int i = activePowerUps.Count - 1; i >= 0; i--)
            {
                if (activePowerUps[i] == null) continue;

                float dist = Vector3.Distance(player.position,
                                              activePowerUps[i].transform.position);
                if (dist > despawnDistance)
                {
                    Destroy(activePowerUps[i]);
                    activePowerUps.RemoveAt(i);
                }
            }
        }
    }

    // ─────────────────────────────────────────
    // BÚSQUEDA DE POSICIÓN EN EL ANILLO
    // ─────────────────────────────────────────
    Vector3 FindValidPosition()
    {
        for (int i = 0; i < maxAttemptsPerFrame; i++)
        {
            // Punto aleatorio en un anillo alrededor del jugador
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);

            Vector3 candidate = player.position + new Vector3(
                randomCircle.x * distance,
                0f,
                randomCircle.y * distance
            );

            if (!IsPositionValid(candidate)) continue;

            return candidate;
        }

        return Vector3.zero; // No encontró posición válida
    }

    bool IsPositionValid(Vector3 pos)
    {
        // 1. Verificar que hay suelo debajo
        if (!Physics.Raycast(pos + Vector3.up * 2f, Vector3.down,
                             out RaycastHit hit, groundCheckDistance, groundLayer))
            return false;

        pos = hit.point; // Ajustar al suelo real

        // 2. Verificar NavMesh (si está activado)
        if (requireNavMesh &&
            !NavMesh.SamplePosition(pos, out _, 1.5f, NavMesh.AllAreas))
            return false;

        // 3. Verificar que no hay otro power up muy cerca
        foreach (var p in activePowerUps)
        {
            if (p != null && Vector3.Distance(p.transform.position, pos) < minDistance * 0.5f)
                return false;
        }

        return true;
    }

    // ─────────────────────────────────────────
    // SPAWN CON PESO (rareza)
    // ─────────────────────────────────────────
    void SpawnPowerUp(Vector3 pos)
    {
        GameObject prefab = GetWeightedPrefab();
        if (prefab == null) return;

        // Ajustar altura exacta al suelo
        if (Physics.Raycast(pos + Vector3.up * 2f, Vector3.down,
                            out RaycastHit hit, groundCheckDistance, groundLayer))
            pos = hit.point;

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        activePowerUps.Add(obj);
    }

    GameObject GetWeightedPrefab()
    {
        float totalWeight = 0f;
        foreach (var e in powerUpEntries) totalWeight += e.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in powerUpEntries)
        {
            cumulative += e.weight;
            if (roll <= cumulative) return e.prefab;
        }

        return powerUpEntries[0].prefab;
    }

    // ─────────────────────────────────────────
    // GIZMOS (visualización en editor)
    // ─────────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || player == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawWireSphere(player.position, maxDistance);

        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawWireSphere(player.position, minDistance);

        Gizmos.color = new Color(1f, 1f, 0f, 0.08f);
        Gizmos.DrawWireSphere(player.position, despawnDistance);
    }
}
