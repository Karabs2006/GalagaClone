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

    public static bool isAnyEnemyDiving = false;
    public static GameObject lastDiveEnemy = null;

    public PlayerDeath playerDeath;
    private bool isDestroyed = false;

    private AudioControl audioControl;
    private bool isInitialized = false;

    [Header("Boundaries")]
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        // Set the layer
        gameObject.layer = LayerMask.NameToLayer("DiveEnemy");

        audioControl = FindFirstObjectByType<AudioControl>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        movingRight = Random.Range(0, 2) == 0;
        timeBeforeDive = 3f + Random.Range(0f, 2f);
        patrolTimer = timeBeforeDive;

        isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isInitialized || isDestroyed) return;

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

        CheckOffScreen();
    }
    void Patrol()
    {
        rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, 0);

        // Bounce off boundaries
        if (transform.position.x > rightBoundary)
        {
            movingRight = false;
        }
        else if (transform.position.x < leftBoundary)
        {
            movingRight = true;
        }
    }

    void CheckIfShouldDive()
    {
        patrolTimer += Time.deltaTime;
        if (isAnyEnemyDiving) return;
        if (lastDiveEnemy == gameObject) return;
        if (patrolTimer >= timeBeforeDive + Random.Range(0f, diveDelayRange))
        {
            StartDive();
        }
    }

    void StartDive()
    {
        if (audioControl != null) audioControl.PlayDiveAudio();
        isAnyEnemyDiving = true;
        lastDiveEnemy = gameObject;

        currentState = State.Diving;
        diveStartPos = transform.position;

        if (player != null)
        {
            diveTargetPos = player.position;
            diveTargetPos.y = player.position.y + 0.5f;
        }
        else
        {
            diveTargetPos = new Vector2(transform.position.x, -2f);
        }

        diveTimer = 0f;
        patrolTimer = 0f;
        diveDuration = Random.Range(0.8f, 1.5f);
    }

    void Dive()
    {
        diveTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(diveTimer / diveDuration);

        float x = Mathf.Lerp(diveStartPos.x, diveTargetPos.x, progress);
        float y = Mathf.Lerp(diveStartPos.y, diveTargetPos.y, progress);

        float curve = Mathf.Sin(progress * Mathf.PI) * 1.5f;
        x += curve;
        rb.MovePosition(new Vector2(x, y));

        if (progress >= 1f)
        {
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
        rb.linearVelocity = new Vector2(0, returnSpeed);

        if (Mathf.Abs(currentPos.y - diveStartPos.y) < 0.3f)
        {
            rb.MovePosition(new Vector2(currentPos.x, diveStartPos.y));
            rb.linearVelocity = Vector2.zero;

            currentState = State.Patrolling;
            patrolTimer = 0f;
            timeBeforeDive = Random.Range(1.5f, 4f);
            isAnyEnemyDiving = false;
        }
    }

    void CheckOffScreen()
    {
        if (transform.position.y < -8f || transform.position.y > 12f ||
            transform.position.x < -12f || transform.position.x > 12f)
        {
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
        if (isDestroyed || !isInitialized) return;

        // Don't collide with other enemies
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            col.gameObject.layer == LayerMask.NameToLayer("DiveEnemy"))
        {
            // Ignore collision with other enemies
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col.collider);
            return;
        }

        if (col.gameObject.CompareTag("Wall") && currentState == State.Patrolling)
        {
            movingRight = !movingRight;
        }

        if (col.gameObject.CompareTag("Player") && currentState == State.Diving)
        {
            isAnyEnemyDiving = false;
            PlayerDeath playerDeath = col.gameObject.GetComponent<PlayerDeath>();

            if (playerDeath != null)
            {
                playerDeath.lifeCounter--;
                isDestroyed = true;
                Destroy(gameObject);

                if (playerDeath.lifeCounter == 1)
                {
                    playerDeath.audioSource.PlayOneShot(playerDeath.audioClip);
                    playerDeath.firstLife.SetActive(false);
                    playerDeath.StartCoroutine(playerDeath.Respawn());
                }
                else if (playerDeath.lifeCounter == 0)
                {
                    playerDeath.audioSource.PlayOneShot(playerDeath.audioClip);
                    playerDeath.secondLife.SetActive(false);
                    playerDeath.StartCoroutine(playerDeath.Respawn());
                }
                else if (playerDeath.lifeCounter == -1)
                {
                    SceneManager.LoadSceneAsync("Game Over");
                }
            }
        }

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
        if (lastDiveEnemy == gameObject)
        {
            lastDiveEnemy = null;
        }
        isAnyEnemyDiving = false;
    }
}