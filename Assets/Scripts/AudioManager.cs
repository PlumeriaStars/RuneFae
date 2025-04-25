using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    AudioSource _audioSource;

    private void Awake()
    {
        #region Singleton Pattern(Simple)
        if (Instance == null)
        {
            //no instance exists, so make it! woooo
            Instance = this;

            DontDestroyOnLoad(gameObject);

            //fill references
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void PlaySong(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip, 7.5f);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        _audioSource.PlayOneShot(clip, volume);
        StartCoroutine(StaggerAudio(clip.length + 0.1f));
    }

    public IEnumerator PauseSong(AudioClip clip)
    {
        yield return new WaitForSecondsRealtime(clip.length + 0.01f);

        _audioSource.Stop();
    }

    private IEnumerator StaggerAudio(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
    }

    public void LoseAudio(AudioClip clip)
    {
        StartCoroutine(DeathSFX(clip));

        _audioSource.Pause();
    }

    private IEnumerator DeathSFX(AudioClip clip)
    {
        PlaySFX(clip, 0.75f);

        yield return new WaitForSecondsRealtime(1f);

    }

}

