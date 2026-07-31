using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float descentSpeed = 0.3f;

    [Header("Boundaries")]
    public float minX = -7.5f;
    public float maxX = 7.5f;

    private Rigidbody2D rb;
    public int sortingOrder = 0;

    private bool movingRight = true;
    private bool isInitialized = false;

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

        movingRight = Random.Range(0, 2) == 0;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;
        }

        isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isInitialized) return;

        float xMovement = movingRight ? moveSpeed : -moveSpeed;

        float yMovement = -descentSpeed;

        rb.linearVelocity = new Vector2(xMovement, yMovement);

        if (transform.position.x > maxX)
        {
            movingRight = false;
        }
        else if (transform.position.x < minX)
        {
            movingRight = true;
        }

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