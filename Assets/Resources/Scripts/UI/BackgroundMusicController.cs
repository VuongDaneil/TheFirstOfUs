using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioSource backgroundMusicSource;
    public AudioClip[] MainMenuMusic;
    private int currentTrack = 0;

    private void OnValidate()
    {
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = GetComponent<AudioSource>();
        }
        currentTrack = 0;
    }

    private void Awake()
    {
        MainMenuMusic.Shuffle();
        if (backgroundMusicSource != null && MainMenuMusic.Length > 0)
        {
            backgroundMusicSource.PlayOneShot(MainMenuMusic[currentTrack]);
        }
    }

    private void Update()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            currentTrack = (currentTrack + 1) % MainMenuMusic.Length;
            backgroundMusicSource.PlayOneShot(MainMenuMusic[currentTrack]);
        }
    }
}
