using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject regularEnemyPrefab;
    public GameObject diveEnemyPrefab;

    [Header("Wave Settings")]
    public int regularEnemiesPerWave = 5;
    public int diveEnemiesPerWave = 2;
    public float timeBetweenWaves = 4f;
    public float spawnDelay = 0.3f;

    [Header("Spawn Position")]
    public float spawnY = 7f;

    [Header("Play Area Boundaries")]
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;
    public float targetY = 3f;

    [Header("Enemy Movement")]
    public float regularMoveSpeed = 2f;
    public float regularDescentSpeed = 0.3f;
    public float divePatrolSpeed = 3f;

    [Header("Wave Progression")]
    public bool increaseDifficulty = true;
    public int extraRegularPerWave = 1;
    public int extraDivePerWave = 1;
    public float speedIncreasePerWave = 0.2f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentWave = 0;
    private bool isSpawning = false;

    // Current wave stats (for difficulty scaling)
    private int currentRegularCount;
    private int currentDiveCount;
    private float currentRegularSpeed;
    private float currentDiveSpeed;

    void Start()
    {
        currentRegularCount = regularEnemiesPerWave;
        currentDiveCount = diveEnemiesPerWave;
        currentRegularSpeed = regularMoveSpeed;
        currentDiveSpeed = divePatrolSpeed;

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

        // Update difficulty
        if (increaseDifficulty)
        {
            currentRegularCount = regularEnemiesPerWave + (currentWave - 1) * extraRegularPerWave;
            currentDiveCount = diveEnemiesPerWave + (currentWave - 1) * extraDivePerWave;
            currentRegularSpeed = regularMoveSpeed + (currentWave - 1) * speedIncreasePerWave;
            currentDiveSpeed = divePatrolSpeed + (currentWave - 1) * speedIncreasePerWave * 0.5f;
        }

        Debug.Log($"===== WAVE {currentWave} =====");
        Debug.Log($"Spawning {currentRegularCount} regular enemies and {currentDiveCount} dive enemies");

        int totalEnemies = currentRegularCount + currentDiveCount;

        float availableWidth = rightBoundary - leftBoundary;

        float calculatedSpacing = availableWidth / (totalEnemies + 1);

        float finalSpacing = Mathf.Clamp(calculatedSpacing, 0.8f, 3f);

        float totalWidth = (totalEnemies - 1) * finalSpacing;
        float startX = -totalWidth / 2f;

        float minX = leftBoundary + 0.5f;
        float maxX = rightBoundary - 0.5f;

        if (startX < minX)
        {
            startX = minX;
        }
        if (startX + totalWidth > maxX)
        {
            startX = maxX - totalWidth;
        }

        // Create lists to track positions
        List<Vector2> targetPositions = new List<Vector2>();

        for (int i = 0; i < totalEnemies; i++)
        {
            float xPos = startX + i * finalSpacing;
            float randomOffset = Random.Range(-0.2f, 0.2f);
            xPos = Mathf.Clamp(xPos + randomOffset, minX, maxX);
            targetPositions.Add(new Vector2(xPos, targetY));
        }

        for (int i = 0; i < targetPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, targetPositions.Count);
            Vector2 temp = targetPositions[i];
            targetPositions[i] = targetPositions[randomIndex];
            targetPositions[randomIndex] = temp;
        }

        for (int i = 0; i < currentRegularCount; i++)
        {
            Vector2 targetPos = targetPositions[i];
            float spawnXOffset = Random.Range(-0.3f, 0.3f);
            Vector2 spawnPos = new Vector2(
                Mathf.Clamp(targetPos.x + spawnXOffset, minX, maxX),
                spawnY + Random.Range(-0.5f, 0.5f)
            );

            GameObject enemy = Instantiate(regularEnemyPrefab, spawnPos, Quaternion.identity);
            enemy.name = $"RegularEnemy_{i}";

            SetupRegularEnemy(enemy, targetPos);
            activeEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnDelay);
        }

        // Spawn dive enemies
        for (int i = 0; i < currentDiveCount; i++)
        {
            int index = currentRegularCount + i;
            Vector2 targetPos = targetPositions[index];
            float spawnXOffset = Random.Range(-0.3f, 0.3f);
            Vector2 spawnPos = new Vector2(
                Mathf.Clamp(targetPos.x + spawnXOffset, minX, maxX),
                spawnY + Random.Range(-0.5f, 0.5f)
            );

            GameObject enemy = Instantiate(diveEnemyPrefab, spawnPos, Quaternion.identity);
            enemy.name = $"DiveEnemy_{i}";

            SetupDiveEnemy(enemy, targetPos);
            activeEnemies.Add(enemy);

            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"Wave {currentWave} complete! {activeEnemies.Count} enemies active.");
        isSpawning = false;
    }

    void SetupRegularEnemy(GameObject enemy, Vector2 targetPos)
    {
        EnemyEntrance entrance = enemy.GetComponent<EnemyEntrance>();
        if (entrance == null)
        {
            entrance = enemy.AddComponent<EnemyEntrance>();
        }
        entrance.targetPosition = targetPos;
        entrance.entranceSpeed = 4f + Random.Range(-0.5f, 0.5f);
        entrance.swoopDepth = 2f + Random.Range(-0.5f, 0.5f);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            enemyScript = enemy.AddComponent<Enemy>();
        }
        enemyScript.moveSpeed = currentRegularSpeed + Random.Range(-0.3f, 0.3f);
        enemyScript.descentSpeed = regularDescentSpeed + Random.Range(-0.1f, 0.1f);
        enemyScript.enabled = false;

        // Set patrol boundaries for regular enemies
        enemyScript.minX = leftBoundary;
        enemyScript.maxX = rightBoundary;

        EnemyDive diveScript = enemy.GetComponent<EnemyDive>();
        if (diveScript != null)
        {
            diveScript.enabled = false;
        }

        EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
        if (shoot != null)
        {
            shoot.enabled = false;
        }

        enemy.layer = LayerMask.NameToLayer("Enemy");
    }

    void SetupDiveEnemy(GameObject enemy, Vector2 targetPos)
    {
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
        if (diveScript == null)
        {
            diveScript = enemy.AddComponent<EnemyDive>();
        }
        diveScript.patrolSpeed = currentDiveSpeed + Random.Range(-0.3f, 0.3f);
        diveScript.timeBeforeDive = 2f + Random.Range(0f, 2f);
        diveScript.enabled = false;

        // Set patrol boundaries for dive enemies
        diveScript.leftBoundary = leftBoundary;
        diveScript.rightBoundary = rightBoundary;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.enabled = false;
        }

        EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
        if (shoot != null)
        {
            shoot.enabled = false;
        }

        // Set layer
        enemy.layer = LayerMask.NameToLayer("DiveEnemy");
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(leftBoundary, spawnY - 2), new Vector2(leftBoundary, spawnY + 1));
        Gizmos.DrawLine(new Vector2(rightBoundary, spawnY - 2), new Vector2(rightBoundary, spawnY + 1));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(leftBoundary, spawnY), new Vector2(rightBoundary, spawnY));

        Gizmos.color = Color.blue;
        int totalEnemies = regularEnemiesPerWave + diveEnemiesPerWave;
        float availableWidth = rightBoundary - leftBoundary;
        float spacing = availableWidth / (totalEnemies + 1);
        float totalWidth = (totalEnemies - 1) * spacing;
        float startX = -totalWidth / 2f;
        Gizmos.DrawLine(new Vector2(startX, targetY), new Vector2(startX + totalWidth, targetY));

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireCube(
            new Vector3((leftBoundary + rightBoundary) / 2f, targetY, 0),
            new Vector3(rightBoundary - leftBoundary, 1f, 0)
        );
    }
}