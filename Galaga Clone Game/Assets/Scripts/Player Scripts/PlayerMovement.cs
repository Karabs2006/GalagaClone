using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    public GameObject currentBulletPrefab;

    float horizontalMovement;

    public Transform gunPoint;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public AudioSource playerMoveAudio;

    public AudioClip playerMoveClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;

        if (horizontalMovement != 0)
        {
            playerMoveAudio.Play();
        }
        else
        {
            playerMoveAudio.Stop();
        }
    }


    public void Shoot(InputAction.CallbackContext context)
    {

        if (!context.performed)
        return;


        if (currentBulletPrefab != null && gunPoint != null)
    {
        audioSource.PlayOneShot(audioClip);
        GameObject bullet = Instantiate(currentBulletPrefab, gunPoint.position, gunPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = gunPoint.up * 10f;
            Destroy(bullet, 3f);
        }
    }

    }
}