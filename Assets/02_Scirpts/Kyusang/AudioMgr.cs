using System.Collections;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    [SerializeField] private AudioClip springMusic;
    [SerializeField] private AudioClip summerMusic;
    [SerializeField] private AudioClip autumnMusic;
    [SerializeField] private AudioClip winterMusic;
    [SerializeField] private AudioSource audioSource;
    private SeasonsSystemURP.seasons currentSeason;

    void Start()
    {
        audioSource.loop = true;
        SeasonsSystemURP.onSeasonChange += ChangeSeason;
        PlaySeasonMusic(SeasonsSystemURP.season);
    }

    void OnDestroy()
    {
        SeasonsSystemURP.onSeasonChange -= ChangeSeason;
    }

    private void ChangeSeason(SeasonsSystemURP.seasons newSeason)
    {
        if (currentSeason != newSeason)
        {
            currentSeason = newSeason;
            PlaySeasonMusic(newSeason);
        }
    }

    private void PlaySeasonMusic(SeasonsSystemURP.seasons season)
    {
        AudioClip clipToPlay = season switch
        {
            SeasonsSystemURP.seasons.spring => springMusic,
            SeasonsSystemURP.seasons.summer => summerMusic,
            SeasonsSystemURP.seasons.autumn => autumnMusic,
            SeasonsSystemURP.seasons.winter => winterMusic,
            _ => null
        };

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    // Optional: Add fade between songs
    [SerializeField] private float crossfadeDuration = 2.0f;
    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        float timeElapsed = 0;
        float startVolume = audioSource.volume;

        // Fade out
        while (timeElapsed < crossfadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / crossfadeDuration);
            yield return null;
        }

        // Change clip and fade in
        audioSource.clip = newClip;
        audioSource.Play();
        timeElapsed = 0;

        while (timeElapsed < crossfadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, startVolume, timeElapsed / crossfadeDuration);
            yield return null;
        }
    }
}