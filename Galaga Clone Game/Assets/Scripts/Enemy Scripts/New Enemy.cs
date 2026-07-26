using UnityEngine;

public class NewEnemy : MonoBehaviour
{
using UnityEngine;

public class EnemyDive : MonoBehaviour
{
    // Movement Settings
    public float patrolSpeed = 5f;
    public float diveSpeed = 12f;
    public float returnSpeed = 8f;

    // Diving Settings
    public float timeBeforeDive = 3f;      // How long to patrol before diving
    public float diveAngle = 45f;           // Steepness of the dive (degrees)
    public bool useSCurve = true;           // S-curve or straight dive?
    public float curveAmplitude = 2f;       // How wide the S-curve is

    // State Machine
    private enum EnemyState { Patrolling, Diving, Returning }
    private EnemyState currentState = EnemyState.Patrolling;

    // References
    private Rigidbody2D rb;
    private Transform playerTarget;
    private bool movingRight = true;

    // Dive tracking
    private float diveTimer = 0f;
    private Vector2 diveStartPosition;
    private Vector2 diveTargetPosition;
    private float diveProgress = 0f;
    private float curveOffset = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Start by moving right
        movingRight = Random.Range(0, 2) == 0;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Diving:
                Dive();
                break;
            case EnemyState.Returning:
                ReturnToFormation();
                break;
        }
    }

    // ==================== PATROL STATE ====================
    void Patrol()
    {
        // Move left/right
        if (movingRight)
            rb.linearVelocity = new Vector2(patrolSpeed, 0);
        else
            rb.linearVelocity = new Vector2(-patrolSpeed, 0);

        // Check if it's time to dive
        diveTimer += Time.deltaTime;
        if (diveTimer >= timeBeforeDive && playerTarget != null)
        {
            StartDive();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            // Flip direction when hitting a wall
            movingRight = !movingRight;
        }
    }

    // ==================== DIVE STATE ====================
    void StartDive()
    {
        currentState = EnemyState.Diving;
        diveTimer = 0f;
        diveStartPosition = transform.position;
        diveTargetPosition = playerTarget.position;
        diveProgress = 0f;

        // Randomize the curve offset for variety
        curveOffset = Random.Range(-1f, 1f);
    }

    void Dive()
    {
        diveProgress += Time.deltaTime * 0.8f; // Speed of the dive animation

        // Calculate the dive path
        Vector2 currentPos = transform.position;

        if (useSCurve)
        {
            // S-Curve: horizontal sway while descending
            float xTarget = Mathf.Lerp(diveStartPosition.x, diveTargetPosition.x, diveProgress);
            float yTarget = Mathf.Lerp(diveStartPosition.y, diveTargetPosition.y, diveProgress);

            // Add the S-curve sway
            float sway = Mathf.Sin(diveProgress * Mathf.PI * 2) * curveAmplitude * (1 - diveProgress);
            xTarget += sway + curveOffset * 0.5f;

            Vector2 targetPos = new Vector2(xTarget, yTarget);
            rb.MovePosition(targetPos);

            // If we've reached the target, start returning
            if (diveProgress >= 1f)
            {
                StartReturn();
            }
        }
        else
        {
            // Straight dive toward player
            Vector2 direction = (diveTargetPosition - diveStartPosition).normalized;
            rb.linearVelocity = direction * diveSpeed;

            // Stop diving when we reach the player's Y level
            if (Mathf.Abs(transform.position.y - diveTargetPosition.y) < 0.5f)
            {
                StartReturn();
            }
        }
    }

    // ==================== RETURN STATE ====================
    void StartReturn()
    {
        currentState = EnemyState.Returning;
        diveTimer = 0f;
    }

    void ReturnToFormation()
    {
        // Fly back up to the original Y position
        Vector2 targetPos = new Vector2(transform.position.x, diveStartPosition.y);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * returnSpeed;

        // Check if we've returned
        if (Mathf.Abs(transform.position.y - diveStartPosition.y) < 0.3f)
        {
            // Snap to position and go back to patrolling
            transform.position = new Vector2(transform.position.x, diveStartPosition.y);
            currentState = EnemyState.Patrolling;
            diveTimer = 0f;

            // Give a random delay before diving again
            timeBeforeDive = Random.Range(2f, 5f);
        }
    }

    // ==================== VISUAL DEBUGGING ====================
    void OnDrawGizmosSelected()
    {
        if (playerTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTarget.position);
        }
    }
}
}
