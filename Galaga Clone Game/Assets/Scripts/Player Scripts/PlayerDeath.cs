using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{   

    public int lifeCounter = 2;
    public GameObject firstLife;
    public GameObject secondLife;
    public SpriteRenderer spriteRenderer;

    public bool isPlayerInvincible = false;

    public AudioSource audioSource;
    public AudioClip audioClip;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

    
        if(collision.CompareTag("Enemy Bullet") && !isPlayerInvincible)
        {   

            Debug.Log("IM DEAD BRUHHHH");
            lifeCounter--;
            Destroy(collision.gameObject);

            if(lifeCounter == 1)
            {   
                audioSource.PlayOneShot(audioClip);
                firstLife.SetActive(false);
                StartCoroutine(Respawn());

            }

            if (lifeCounter == 0)
            {
                audioSource.PlayOneShot(audioClip);
                secondLife.SetActive(false);
                StartCoroutine(Respawn());
            }

            if (lifeCounter == -1)
            {
                SceneManager.LoadSceneAsync("Game Over");
            }
            
        }
    }


    /* public IEnumerator Respawn()
     {

         isPlayerInvincible = true;

         yield return new WaitForSeconds(0.2f);
         gameObject.SetActive(true);
         transform.position = new Vector2(-0.09f, -3.84f);

         for(int i = 0; i < 8; i++)
         {   
             spriteRenderer.enabled = false;
             yield return new WaitForSeconds(0.2f);

             spriteRenderer.enabled = true;
             yield return new WaitForSeconds(0.2f);

         }

         isPlayerInvincible = false;

         //Vector2 spawnPosition = new Vector2(-0.09f, -3.84f);
         //gameObject.SetActive(true);
         //Instantiate(gameObject, spawnPosition, Quaternion.identity);
     }*/

    public IEnumerator Respawn()
    {
        enemyBullet.isPlayerRespawning = true;

        // Wait a moment for bullets to clear
        yield return new WaitForSeconds(0.2f);

        isPlayerInvincible = true;

        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(true);
        transform.position = new Vector2(-0.09f, -3.84f);

        for (int i = 0; i < 8; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }

        // Wait before allowing enemy bullets to hit player again
        yield return new WaitForSeconds(1.5f); 
        enemyBullet.isPlayerRespawning = false;

        

        isPlayerInvincible = false;
    }
}
