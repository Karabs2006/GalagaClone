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
    private bool isInFormation = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        swoopTarget = new Vector2(targetPosition.x, targetPosition.y - swoopDepth);

        Debug.Log(gameObject.name + " spawning, target: " + targetPosition);
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
            Debug.Log(gameObject.name + " starting swoop!");
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
            isInFormation = true;

            Debug.Log(gameObject.name + " in formation!");

            EnemyShoot shoot = GetComponent<EnemyShoot>();
            if (shoot != null)
            {
                shoot.enabled = true;
            }
        }
    }
}