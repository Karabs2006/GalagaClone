using UnityEngine;

public class enemyBullet : MonoBehaviour
{

    public Rigidbody2D Bullet;

    public float moveSpeed = 11.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bullet = this.gameObject.GetComponent <Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Bullet.linearVelocity = new Vector2 (0, -1) * moveSpeed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SetActive(false);
          //  Destroy(Bullet, 2f);
        }
      /*  if (collision.gameObject.name == "Bottom")
        {
            Object.Destroy (this.gameObject);
        }*/
    }
}
