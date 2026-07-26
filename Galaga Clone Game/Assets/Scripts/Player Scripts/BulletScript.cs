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
            collision.gameObject.SetActive(false);
        }
    }
}
