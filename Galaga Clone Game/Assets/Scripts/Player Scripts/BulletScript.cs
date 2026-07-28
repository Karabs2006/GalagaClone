using UnityEngine;
using TMPro;

public class BulletScript : MonoBehaviour
{

    public Rigidbody2D bullet;
    public TMP_Text score;
    public int scoreInt = 1 ;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bullet = this.gameObject.GetComponent<Rigidbody2D>();
        score = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        score.text = $"{PlayerScore.finalScore}";
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
            PlayerScore.finalScore += 30;
            score.text = $"{PlayerScore.finalScore}";

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
