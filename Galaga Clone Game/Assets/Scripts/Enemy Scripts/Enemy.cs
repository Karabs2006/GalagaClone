using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float horizontalSway = 0.9f;

    private Rigidbody2D rb;
    private float startX;
    private float timer = 1f;
    public int sortingOrder = 0; // For layer sorting
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startX = transform.position.x;

        // NEW: Set sorting order on sprite renderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;
        }

        // Start shooting
        EnemyShoot shoot = GetComponent<EnemyShoot>();
        if (shoot != null)
        {
            shoot.enabled = true;
        }
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        float xMovement = Mathf.Sin(timer * 2f) * horizontalSway; // Slight sway
        float yMovement = -moveSpeed;

        rb.linearVelocity = new Vector2(xMovement, yMovement);

        // Destroy if goes off screen (bottom)
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }
}


