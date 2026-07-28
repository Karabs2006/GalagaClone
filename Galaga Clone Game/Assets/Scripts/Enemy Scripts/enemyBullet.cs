using UnityEngine;

public class enemyBullet : MonoBehaviour
{

 
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            // Destroy the player
            //collision.gameObject.SetActive(false);
            

            // Destroy this bullet
            //Destroy(gameObject);
        }

        // Destroy bullet if it hits a wall or bottom
        if (collision.CompareTag("Wall") || collision.CompareTag("Bottom"))
        {
            Destroy(gameObject);
        }
    }
}