using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsEffect : MonoBehaviour
{
    public AudioClip[] attackSounds;
    public float[] volumes;
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySounds(int index, float volume)
    {
        audioSource.clip = attackSounds[index];
        audioSource.volume = volume;
        audioSource.PlayOneShot(attackSounds[index]);
    }
    public void StopSounds(int index)
    {
        audioSource.clip = attackSounds[index];
        audioSource.Stop();
    }
    void PlayRandomAttackSound()
    {
        if (!isPlaying)
        {
            int randomIndex = Random.Range(0, attackSounds.Length);
            AudioClip randomClip = attackSounds[randomIndex];
            float volume = volumes[randomIndex];

            StartCoroutine(PlaySoundForDuration(randomClip, volume, 2f));
        }
    }

    IEnumerator PlaySoundForDuration(AudioClip clip, float volume, float duration)
    {
        isPlaying = true;
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
        isPlaying = false;
    }

}
