using UnityEngine;

public class EnemyDive : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 5f;
    public float diveSpeed = 15f;
    public float returnSpeed = 10f;

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
    private bool isDiving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("EnemyDive: Player found! " + player.name);
        }

        movingRight = Random.Range(0, 2) == 0;
        patrolTimer = Random.Range(0f, timeBeforeDive);

    }

    void FixedUpdate()
    {
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
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, 0);
    }

    void CheckIfShouldDive()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= timeBeforeDive + Random.Range(0f, diveDelayRange))
        {
            Debug.Log("EnemyDive: Starting dive now!");
            StartDive();
        }
    }

    void StartDive()
    {
        currentState = State.Diving;
        isDiving = true;
        diveStartPos = transform.position;
        diveTargetPos = player.position;
        diveTimer = 0f;
        patrolTimer = 0f;
        diveDuration = Random.Range(0.8f, 1.5f);
    }

    void Dive()
    {
        diveTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(diveTimer / diveDuration);

        // Calculate position
        float x = Mathf.Lerp(diveStartPos.x, diveTargetPos.x, progress);
        float y = Mathf.Lerp(diveStartPos.y, diveTargetPos.y, progress);

        // Add curve
        float curve = Mathf.Sin(progress * Mathf.PI * 2) * 0.5f * (1 - progress);
        x += curve;

        // Move the enemy
        rb.MovePosition(new Vector2(x, y));

        if (progress >= 1f)
        {
            Debug.Log("EnemyDive: Dive complete, returning!");
            StartReturn();
        }
    }

    void StartReturn()
    {
        currentState = State.Returning;
        isDiving = false;
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
            isDiving = false;
            patrolTimer = 0f;
            timeBeforeDive = Random.Range(1.5f, 4f);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall") && currentState == State.Patrolling)
        {
            movingRight = !movingRight;
        }
        if (col.gameObject.CompareTag("Player") && currentState == State.Diving)
        {
            Debug.Log("EnemyDive: CRASHED INTO PLAYER!");
            col.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}