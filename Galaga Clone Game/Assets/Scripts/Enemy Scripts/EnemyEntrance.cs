using UnityEngine;

public class EnemyEntrance : MonoBehaviour
{
    [Header("Entrance Settings")]
    public Vector2 targetPosition;
    public float entranceSpeed = 4f;
    public float swoopDepth = 2f;

    private enum EntranceState { FlyingIn, Swooping, FormingUp, InFormation }
    private EntranceState currentState = EntranceState.FlyingIn;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 swoopTarget;
    private float progress = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        startPosition = transform.position;
        swoopTarget = new Vector2(targetPosition.x, targetPosition.y - swoopDepth);

        // Disable all movement scripts temporarily
        Enemy enemyScript = GetComponent<Enemy>();
        if (enemyScript != null) enemyScript.enabled = false;

        EnemyDive diveScript = GetComponent<EnemyDive>();
        if (diveScript != null) diveScript.enabled = false;

        EnemyShoot shoot = GetComponent<EnemyShoot>();
        if (shoot != null) shoot.enabled = false;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case EntranceState.FlyingIn:
                FlyIn();
                break;
            case EntranceState.Swooping:
                Swoop();
                break;
            case EntranceState.FormingUp:
                FormUp();
                break;
            case EntranceState.InFormation:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void FlyIn()
    {
        rb.linearVelocity = new Vector2(0, -entranceSpeed);

        if (transform.position.y <= swoopTarget.y + 0.3f)
        {
            currentState = EntranceState.Swooping;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Swoop()
    {
        progress += Time.deltaTime * 1.5f;
        float t = Mathf.Clamp01(progress);

        float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
        float y = Mathf.Lerp(swoopTarget.y, targetPosition.y, t);

        float curve = Mathf.Sin(t * Mathf.PI) * 0.8f;
        x += curve;

        rb.MovePosition(new Vector2(x, y));

        if (t >= 1f)
        {
            currentState = EntranceState.FormingUp;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void FormUp()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * entranceSpeed * 0.5f;

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            rb.MovePosition(targetPosition);
            rb.linearVelocity = Vector2.zero;
            currentState = EntranceState.InFormation;

            // Enable the appropriate script based on what type of enemy this is
            Enemy enemyScript = GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enabled = true;
                enemyScript.Initialize();
            }

            EnemyDive diveScript = GetComponent<EnemyDive>();
            if (diveScript != null)
            {
                diveScript.enabled = true;
                diveScript.Initialize();
            }

            EnemyShoot shoot = GetComponent<EnemyShoot>();
            if (shoot != null) shoot.enabled = true;
        }
    }
}