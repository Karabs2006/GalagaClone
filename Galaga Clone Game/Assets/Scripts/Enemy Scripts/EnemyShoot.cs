using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyShoot : MonoBehaviour
{
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
