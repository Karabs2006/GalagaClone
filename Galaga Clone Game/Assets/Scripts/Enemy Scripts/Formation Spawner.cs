using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject regularEnemyPrefab;
    public GameObject diveEnemyPrefab;

    [Header("Formation Settings")]
    public int columns = 5;
    public int rows = 3;
    public float spacingX = 1.5f;
    public float spacingY = 1.2f;
    public Vector2 formationCenter = new Vector2(0, 4f);
    public float spawnY = 8f;

    [Header("Entrance Settings")]
    public float entranceSpeed = 4f;
    public float enemySpeed = 2f;
    public float spawnDelay = 0.2f;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f;

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
            Debug.Log("All enemies destroyed! Next wave in " + timeBetweenWaves + " seconds...");
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

        Debug.Log("===== WAVE " + currentWave + " =====");

        float startX = formationCenter.x - (columns - 1) * spacingX / 2;
        float startY = formationCenter.y + (rows - 1) * spacingY / 2;

        int enemyCount = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate target position
                Vector2 targetPos = new Vector2(
                    startX + col * spacingX,
                    startY - row * spacingY
                );

                GameObject prefabToUse = regularEnemyPrefab;
                bool isDiveEnemy = false;

                // 1 in 3 chance to be a dive enemy
                if (Random.Range(0, 3) == 0 && diveEnemyPrefab != null)
                {
                    prefabToUse = diveEnemyPrefab;
                    isDiveEnemy = true;
                }

                float xOffset = Random.Range(-0.5f, 0.5f);
                Vector2 spawnPos = new Vector2(targetPos.x + xOffset, spawnY);
                GameObject enemy = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
                enemy.name = (isDiveEnemy ? "DiveEnemy" : "RegularEnemy") + "_" + row + "_" + col;

                EnemyEntrance entrance = enemy.AddComponent<EnemyEntrance>();
                entrance.targetPosition = targetPos;
                entrance.entranceSpeed = entranceSpeed + Random.Range(-0.5f, 0.5f);
                entrance.swoopDepth = 2f + Random.Range(-0.5f, 0.5f);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.moveSpeed = enemySpeed + Random.Range(-0.3f, 0.3f);
                }

                EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
                if (shoot != null)
                {
                    shoot.enabled = false;
                }

                activeEnemies.Add(enemy);
                enemyCount++;

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        Debug.Log("Wave " + currentWave + " complete! " + enemyCount + " enemies spawning.");

        isSpawning = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float width = columns * spacingX;
        float height = rows * spacingY;
        Gizmos.DrawWireCube(formationCenter, new Vector2(width, height));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(formationCenter.x - width / 2, spawnY),
                        new Vector2(formationCenter.x + width / 2, spawnY));
    }
}