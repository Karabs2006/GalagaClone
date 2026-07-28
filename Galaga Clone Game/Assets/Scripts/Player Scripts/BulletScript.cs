using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public Rigidbody2D bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bullet = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Bullet hit enemy: " + collision.gameObject.name);

            // Destroy the enemy
            Destroy(collision.gameObject);

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
