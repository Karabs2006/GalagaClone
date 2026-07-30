using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public static bool isPlayerRespawning = false;
    private float spawnTimer = 0f; 
    

    void Update() 
    {
        spawnTimer += Time.deltaTime;

        // If player is respawning, destroy the bullet
        if (isPlayerRespawning)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //  Don't damage player during respawn
        if (isPlayerRespawning) return;

        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Wall") || collision.CompareTag("Bottom"))
        {
            Destroy(gameObject);
        }
    }
}