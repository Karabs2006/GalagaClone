using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource audioSource;
    public  AudioClip audioClip;

    public AudioSource enemyAudioSource;

    public AudioClip enemyShoot;

    public AudioSource diveEnemyAudio;
    public AudioClip diveClip;

    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayEnemyAudio()
    {
        enemyAudioSource.PlayOneShot(enemyShoot);
    }

    public void PlayDiveAudio()
    {
        diveEnemyAudio.PlayOneShot(diveClip);
    }
}



