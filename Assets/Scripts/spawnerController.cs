using UnityEngine;
using System.Collections;

public class spawnerController : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject player;
    [SerializeField] float spawnRange;

    [SerializeField] float minFreq, maxFreq;
    float timer = 0;
    void Update()
    {
        if (timer <= 0)
        {
            timer = Random.Range(minFreq, maxFreq);
            SpawnEnemy();
        }
        else timer -= Time.deltaTime;
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float offset = Random.Range(0, spawnRange);

        Vector3 offsetDirection = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)).normalized;

        GameObject lastEnemy = Instantiate(enemyPrefab, transform.position + offsetDirection * offset, Quaternion.identity);

        lastEnemy.GetComponent<EnemyController>().target = player;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
