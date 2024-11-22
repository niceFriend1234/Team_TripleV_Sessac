using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    [SerializeField] private AudioClip springMusic;
    [SerializeField] private AudioClip summerMusic;
    [SerializeField] private AudioClip autumnMusic;
    [SerializeField] private AudioClip winterMusic;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float crossfadeDuration;
    private SeasonsSystemURP.seasons currentSeason;

    void Start()
    {
        audioSource.loop = true;
        SeasonsSystemURP.onSeasonChange += ChangeSeasonMusic;
    }

    void OnDestroy()
    {
        SeasonsSystemURP.onSeasonChange -= ChangeSeasonMusic;
    }
    private async void ChangeSeasonMusic(SeasonsSystemURP.seasons newSeason)
    {
        AudioClip newClip = GetAudioClipForSeason(newSeason);
        await CrossfadeMusicAsync(newClip);
    }

    private async Task CrossfadeMusicAsync(AudioClip newClip)
    {
        float timeElapsed = 0;
        float startVolume = audioSource.volume;

        // Fade out
        while (timeElapsed < crossfadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / crossfadeDuration);
            await Task.Yield();
        }

        // Change the audio clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Reset timer
        timeElapsed = 0;

        // Fade in
        while (timeElapsed < crossfadeDuration)
        {
            timeElapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, startVolume, timeElapsed / crossfadeDuration);
            await Task.Yield();
        }
    }

    private AudioClip GetAudioClipForSeason(SeasonsSystemURP.seasons season)
    {

        switch (season)
        {
            case SeasonsSystemURP.seasons.spring:
                return springMusic;
            case SeasonsSystemURP.seasons.summer:
                return summerMusic;
            case SeasonsSystemURP.seasons.autumn:
                return autumnMusic;
            case SeasonsSystemURP.seasons.winter:
                return winterMusic;
            default:
                return null;
        }
    }


}