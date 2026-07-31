using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyShoot : MonoBehaviour
/*{
    public GameObject enemyBullet;

    public Transform bulletSpawn;

    public float nextFire = 1.0f;
    public float currentTime = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletSpawn = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        enemyShoot();
    }

    public void enemyShoot()
    {
        currentTime += Time.deltaTime;

        if (currentTime > nextFire)
        {
            nextFire += currentTime;

            Instantiate (enemyBullet, bulletSpawn.position, Quaternion.identity);

            nextFire -= currentTime;

            currentTime = 0.0f;
        }
    }
}
*/
{
    [Header("Shooting")]
    public float fireRate = 1.5f;
    public float bulletSpeed = 8f;
    private float timer = 0f;
    private AudioControl audioControl;

    void Start()
    {
        enabled = true;
        audioControl = FindFirstObjectByType<AudioControl>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        if (audioControl != null) audioControl.PlayEnemyAudio();

        GameObject bullet = new GameObject("EnemyBullet");
        bullet.transform.position = transform.position;
        bullet.tag = "Enemy Bullet";

        // Set the layer for enemy bullets
        bullet.layer = LayerMask.NameToLayer("EnemyBullet");

        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(0, -bulletSpeed);

        CircleCollider2D collider = bullet.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.2f;

        SpriteRenderer sprite = bullet.AddComponent<SpriteRenderer>();
        sprite.color = Color.red;
        sprite.sprite = CreateCircleSprite();

        enemyBullet bulletScript = bullet.AddComponent<enemyBullet>();

        // Destroy after 3 seconds
        Destroy(bullet, 3f);
    }

    Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];

        for (int i = 0; i < 32 * 32; i++)
        {
            colors[i] = Color.red;
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 15, 15), new Vector2(0.5f, 0.3f));
    }
}

