using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDive : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 5f;
    public float diveSpeed = 12f;
    public float returnSpeed = 8f;

    [Header("Dive Settings")]
    public float timeBeforeDive = 2f;
    public float diveDelayRange = 2f;

    private enum State { Patrolling, Diving, Returning }
    private State currentState = State.Patrolling;

    private Rigidbody2D rb;
    private Transform player;
    private bool movingRight = true;

    private float patrolTimer = 0f;
    private float diveTimer = 0f;

    private Vector2 diveStartPos;
    private Vector2 diveTargetPos;
    private float diveDuration = 1.2f;

    // Track if any dive enemy is currently diving
    public static bool isAnyEnemyDiving = false;
    // Track the last dive enemy that went
    public static GameObject lastDiveEnemy = null;

    public PlayerDeath playerDeath;
    private bool isDestroyed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        movingRight = Random.Range(0, 2) == 0;

        // Start with a delay before diving
        timeBeforeDive = 3f + Random.Range(0f, 2f);
        patrolTimer = timeBeforeDive;
    }

    void FixedUpdate()
    {
        if (isDestroyed) return;

        if (player == null)
        {
            Patrol();
            return;
        }

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                CheckIfShouldDive();
                break;
            case State.Diving:
                Dive();
                break;
            case State.Returning:
                ReturnToPosition();
                break;
        }

        // Check if enemy is off screen (destroy if too far)
        CheckOffScreen();
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, 0);
    }

    void CheckIfShouldDive()
    {
        patrolTimer += Time.deltaTime;

        // Only dive if:
        // 1. No other enemy is diving
        // 2. This enemy isn't the last one that dove
        // 3. Timer is ready
        if (isAnyEnemyDiving) return;
        if (lastDiveEnemy == gameObject) return;

        if (patrolTimer >= timeBeforeDive + Random.Range(0f, diveDelayRange))
        {
            Debug.Log(gameObject.name + " starting dive!");
            StartDive();
        }
    }

    void StartDive()
    {
        isAnyEnemyDiving = true;
        lastDiveEnemy = gameObject;

        currentState = State.Diving;
        diveStartPos = transform.position;
        diveTargetPos = player.position;
        diveTimer = 0f;
        patrolTimer = 0f;
        diveDuration = Random.Range(0.8f, 1.5f);

        // Stop slightly above the player
        diveTargetPos.y = player.position.y + 0.5f;
    }

    void Dive()
    {
        diveTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(diveTimer / diveDuration);

        // Smooth movement toward target
        float x = Mathf.Lerp(diveStartPos.x, diveTargetPos.x, progress);
        float y = Mathf.Lerp(diveStartPos.y, diveTargetPos.y, progress);

        // Add a slight curve
        float curve = Mathf.Sin(progress * Mathf.PI) * 1.5f;
        x += curve;

        rb.MovePosition(new Vector2(x, y));

        if (progress >= 1f)
        {
            Debug.Log(gameObject.name + " dive complete, returning!");
            StartReturn();
        }
    }

    void StartReturn()
    {
        isAnyEnemyDiving = false;
        currentState = State.Returning;
        diveTimer = 0f;
        diveTargetPos = new Vector2(transform.position.x, diveStartPos.y);
    }

    void ReturnToPosition()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = new Vector2(currentPos.x, diveStartPos.y);

        // Move upward
        rb.linearVelocity = new Vector2(0, returnSpeed);

        if (Mathf.Abs(currentPos.y - diveStartPos.y) < 0.3f)
        {
            rb.MovePosition(new Vector2(currentPos.x, diveStartPos.y));
            rb.linearVelocity = Vector2.zero;

            currentState = State.Patrolling;
            patrolTimer = 0f;
            timeBeforeDive = Random.Range(1.5f, 4f);
            isAnyEnemyDiving = false;

            Debug.Log(gameObject.name + " returned to formation!");
        }
    }

    void CheckOffScreen()
    {
        // Destroy if enemy goes too far off screen
        if (transform.position.y < -8f || transform.position.y > 12f ||
            transform.position.x < -12f || transform.position.x > 12f)
        {
            Debug.Log(gameObject.name + " went off screen - destroying");
            isDestroyed = true;
            isAnyEnemyDiving = false;
            if (lastDiveEnemy == gameObject)
            {
                lastDiveEnemy = null;
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDestroyed) return;

        if (col.gameObject.CompareTag("Wall") && currentState == State.Patrolling)
        {
            movingRight = !movingRight;
        }

        if (col.gameObject.CompareTag("Player") && currentState == State.Diving)
        {
            isAnyEnemyDiving = false;

            PlayerDeath playerDeath = col.gameObject.GetComponent<PlayerDeath>();

            Debug.Log(gameObject.name + " CRASHED INTO PLAYER!");

            playerDeath.lifeCounter--;
            isDestroyed = true;
            Destroy(gameObject);

            if (playerDeath.lifeCounter == 1)
            {
                playerDeath.firstLife.SetActive(false);
                playerDeath.StartCoroutine(playerDeath.Respawn());
            }

            if (playerDeath.lifeCounter == 0)
            {
                playerDeath.secondLife.SetActive(false);
                playerDeath.StartCoroutine(playerDeath.Respawn());
            }

            if (playerDeath.lifeCounter == -1)
            {
                SceneManager.LoadSceneAsync("Game Over");
            }
        }

        // If enemy is destroyed by bullet, reset the dive flag
        if (col.gameObject.CompareTag("Bullet"))
        {
            isAnyEnemyDiving = false;
            if (lastDiveEnemy == gameObject)
            {
                lastDiveEnemy = null;
            }
        }
    }

    void OnDestroy()
    {
        // Clean up static variables when enemy is destroyed
        if (lastDiveEnemy == gameObject)
        {
            lastDiveEnemy = null;
        }
        isAnyEnemyDiving = false;
    }
}