using System.Collections;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioSource roarAudioSource;
    [SerializeField] private AudioClip roarSound;
    [SerializeField] private AudioClip atack1Sound;
    [SerializeField] private AudioClip atack2Sound;
    [SerializeField] private AudioClip atack3Sound;
    [SerializeField] private AudioClip atack35Sound;
    [SerializeField] private AudioClip atack4Sound;
    [SerializeField] private AudioClip atack45Sound;
    [SerializeField] private AudioClip idle2Sound;

    private void PlaySound(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);  // slight pitch variation
        audioSource.PlayOneShot(clip);
    }

    public void PlayWalkSound()
    {
        PlaySound(walkSound);
    }

    public void PlayRunSound()
    {
        PlaySound(runSound);
    }

    public void roar()
    {
        roarAudioSource.pitch = Random.Range(0.9f, 1.1f);
        roarAudioSource.clip = roarSound;
        roarAudioSource.Play(); //roar was getting cutoff so change it
        AlertNearbyEnemies();
    }

    private void AlertNearbyEnemies()
    {
        Enemy thisEnemy = GetComponent<Enemy>();
        Collider[] hits = Physics.OverlapSphere(transform.position, thisEnemy.hearingRange);
        foreach (Collider hit in hits)
        {
            Enemy nearbyEnemy = hit.GetComponent<Enemy>();
            if (nearbyEnemy != null && nearbyEnemy != thisEnemy)
            {
                nearbyEnemy.AlertFromRoar();
            }
        }
    }

    public void PlayAtack1Sound()
    {
        PlaySound(atack1Sound);
    }
    public void PlayAtack2Sound()
    {
        PlaySound(atack2Sound);
    }
    public void PlayAtack3Sound()
    {
        PlaySound(atack3Sound);
    }
    public void PlayAtack35Sound()
    {
        PlaySound(atack35Sound);
        StartCoroutine(StopAfterTime(1.5f));
    }
    public void PlayAtack4Sound()
    {
        PlaySound(atack4Sound);
    }
    public void PlayAtack45Sound()
    {
        PlaySound(atack45Sound);
        StartCoroutine(StopAfterTime(2f));
    }

    public void PlayIdle2Sound()
    {
        PlaySound(idle2Sound);
        StartCoroutine(StopAfterTime(3.1f));
    }

    private IEnumerator StopAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.Stop();
    }
}