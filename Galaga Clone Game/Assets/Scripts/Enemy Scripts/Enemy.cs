using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 15.0f;
    private Rigidbody2D rb;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        movingRight = Random.Range(0, 2) == 0;
    }

    void FixedUpdate()
    {
        if (movingRight)
            rb.linearVelocity = new Vector2(moveSpeed, 0);
        else
            rb.linearVelocity = new Vector2(-moveSpeed, 0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            // Flip direction randomness
            movingRight = !movingRight;
            moveSpeed = Random.Range(6f, 12f);
        }
    }
}

