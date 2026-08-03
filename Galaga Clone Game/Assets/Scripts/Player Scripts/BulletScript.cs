using UnityEngine;
using TMPro;

public class BulletScript : MonoBehaviour
{

    public Rigidbody2D bullet;
    public TMP_Text score;
    public int scoreInt = 1 ;
    


    private AudioControl audioControl;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioControl = FindFirstObjectByType<AudioControl>();
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
            /*

            if(collision.gameObject.name == "Dive Enemy")
            {
                 PlayerScore.finalScore += 45;
            }
            PlayerScore.finalScore += 30;

            */
            
            if (collision.gameObject.GetComponent<EnemyDive>() != null)
            {
                PlayerScore.finalScore += 45;
            }
            else
            {
                PlayerScore.finalScore += 30;
            }



            score.text = $"{PlayerScore.finalScore}";

            //AudioControl audioControl = GetComponent<AudioControl>();
            audioControl.PlayAudio();

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
