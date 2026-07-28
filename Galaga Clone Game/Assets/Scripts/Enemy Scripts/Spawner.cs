using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;

    [Header("Spawn Area")]
    public float spawnXMin = -5f;
    public float spawnXMax = 5f;
    public float spawnY = 7f;

    [Header("Difficulty Scaling")]
    public bool increaseDifficulty = true;
    public int extraEnemiesPerWave = 1;
    public float speedIncreasePerWave = 0.2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawning = false;
    private float currentSpeed = 3f;
    private bool isWaiting = false; 

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        int beforeCount = activeEnemies.Count;
        activeEnemies.RemoveAll(enemy => enemy == null);
        int afterCount = activeEnemies.Count;

        if (!isSpawning && !isWaiting && activeEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated! Starting next wave in " + timeBetweenWaves + " seconds...");
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        isWaiting = true;

        yield return new WaitForSeconds(timeBetweenWaves);

        isWaiting = false;

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        if (isSpawning) yield break;

        isSpawning = true;
        currentWave++;

        int enemiesThisWave = enemiesPerWave;
        if (increaseDifficulty)
        {
            enemiesThisWave += (currentWave - 1) * extraEnemiesPerWave;
        }


        if (increaseDifficulty)
        {
            currentSpeed = 3f + (currentWave - 1) * speedIncreasePerWave;
        }

        Debug.Log("===== WAVE " + currentWave + " =====");
        Debug.Log("Spawning " + enemiesThisWave + " enemies at speed " + currentSpeed);

        // Spawn enemies
        for (int i = 0; i < enemiesThisWave; i++)
        {
            float xPos = Random.Range(spawnXMin, spawnXMax);
            Vector2 spawnPos = new Vector2(xPos, spawnY);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Set the enemy's speed
            Enemy simpleEnemy = enemy.GetComponent<Enemy>();
            if (simpleEnemy != null)
            {
                simpleEnemy.moveSpeed = currentSpeed + Random.Range(-0.5f, 0.5f);
                Debug.Log("Enemy " + (i + 1) + " speed: " + simpleEnemy.moveSpeed);
            }

            activeEnemies.Add(enemy);

            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Wave " + currentWave + " complete! " + activeEnemies.Count + " enemies active.");

        isSpawning = false;
    }

}

