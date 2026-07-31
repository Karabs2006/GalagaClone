using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveSpanwer : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject diveEnemyPrefab;

    [Header("Spawn Settings")]
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 4f;
    public float spawnDelay = 0.5f;

    [Header("Spawn Position")]
    public float spawnXMin = -4f;
    public float spawnXMax = 4f;
    public float spawnY = 7f;

    [Header("Target Formation")]
    public float targetY = 3f;
    public float spacingX = 2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (!isSpawning && activeEnemies.Count == 0)
        {
            Debug.Log("All dive enemies destroyed! Next wave in " + timeBetweenWaves + " seconds...");
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        if (isSpawning) yield break;

        isSpawning = true;
        currentWave++;

        Debug.Log("===== DIVE WAVE " + currentWave + " =====");

        // Calculate spacing
        float totalWidth = (enemiesPerWave - 1) * spacingX;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            float xPos = startX + i * spacingX;
            // Add some randomness to position
            xPos += Random.Range(-0.3f, 0.3f);

            Vector2 spawnPos = new Vector2(xPos, spawnY);
            Vector2 targetPos = new Vector2(xPos, targetY);

            GameObject enemy = Instantiate(diveEnemyPrefab, spawnPos, Quaternion.identity);
            enemy.name = "DiveEnemy_" + i;

            // Add and setup entrance
            EnemyEntrance entrance = enemy.GetComponent<EnemyEntrance>();
            if (entrance == null)
            {
                entrance = enemy.AddComponent<EnemyEntrance>();
            }
            entrance.targetPosition = targetPos;
            entrance.entranceSpeed = 4f + Random.Range(-0.5f, 0.5f);
            entrance.swoopDepth = 2f + Random.Range(-0.5f, 0.5f);

            // Setup dive enemy
            EnemyDive diveScript = enemy.GetComponent<EnemyDive>();
            if (diveScript != null)
            {
                diveScript.patrolSpeed = 3f + Random.Range(-0.5f, 0.5f);
                diveScript.timeBeforeDive = 2f + Random.Range(0f, 2f);
            }

            // Disable shooting until in formation
            EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
            if (shoot != null)
            {
                shoot.enabled = false;
            }

            // Make sure regular enemy script is disabled
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enabled = false;
            }

            activeEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log("Dive wave " + currentWave + " complete! " + enemiesPerWave + " dive enemies spawning.");
        isSpawning = false;
    }

    void OnDrawGizmosSelected()
    {
        // Show spawn area
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(spawnXMin, spawnY), new Vector2(spawnXMax, spawnY));

        // Show target formation line
        Gizmos.color = Color.blue;
        float totalWidth = (enemiesPerWave - 1) * spacingX;
        float startX = -totalWidth / 2f;
        Gizmos.DrawLine(new Vector2(startX, targetY), new Vector2(startX + totalWidth, targetY));
    }
}