using UnityEngine;

public class FormationTarget : MonoBehaviour
{
    public Vector2 targetPosition;
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private bool reachedTarget = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Enemy targeting formation position: " + targetPosition);
    }

    void FixedUpdate()
    {
        if (reachedTarget) return;

        // Move toward target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Check if reached target
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            reachedTarget = true;
            transform.position = targetPosition;
            Debug.Log("Enemy reached formation!");

            // Start shooting
            EnemyShoot shoot = GetComponent<EnemyShoot>();
            if (shoot != null)
            {
                shoot.enabled = true;
            }
        }
    }
}