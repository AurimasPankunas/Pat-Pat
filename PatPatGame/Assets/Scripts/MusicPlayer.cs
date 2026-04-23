using UnityEngine;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource source;
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private bool spatialMusic;
    [SerializeField] private float minWaitBetweenPlays = 1f;
    [SerializeField] private float maxWaitBetweenPlays = 5f;
    private float waitTimeCountdown = -1f;

    void Start()
    {
        soundManager = GameManager.Instance.soundManager;
        source = soundManager.getMusicSource();
        
        if (spatialMusic)
            soundManager.SetMusicLocation(transform.position);
    }

    // when music stops playing, pick a new random song
    void Update()
    {
        if (!source.isPlaying)
        {
            if (waitTimeCountdown < 0f && audioClips.Count != 0)
            {
                AudioClip currentClip = audioClips[Random.Range(0, audioClips.Count)];
                soundManager.PlayMusic(currentClip, 1, false);
                waitTimeCountdown = Random.Range(minWaitBetweenPlays, maxWaitBetweenPlays);
            }
            else
            {
                waitTimeCountdown -= Time.deltaTime;
            }
        }
    }

}
