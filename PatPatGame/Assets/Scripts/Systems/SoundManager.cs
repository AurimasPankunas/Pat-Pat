using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Central sound player and volume manager
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup sfxGroup;
    private AudioMixerGroup uiGroup;

    [Header("SFX Pool")]
    public int poolSize = 10;
    private List<AudioSource> sfxSources;
    private AudioSource musicSource;

    private int currentSourceIndex = 0;
    public bool isTesting = false;

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        InitializeMixerGroups();
        InitializeAudioPool();
        InitializeMusicSource();
    }

    private void InitializeAudioPool()
    {
        sfxSources = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject("SFX_Source_" + i);
            obj.transform.parent = transform;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.spatialBlend = 1f; // 3D sound

            sfxSources.Add(source);
        }
    }

    private void InitializeMusicSource()
    {
        if (musicSource != null)
            return;
        GameObject musicObj = new GameObject("Music_Source");
        musicObj.transform.parent = transform;

        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 2D sound
    }

    private void InitializeMixerGroups()
    {
        musicGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
        sfxGroup = audioMixer.FindMatchingGroups("Master/SFX")[0];
        uiGroup = audioMixer.FindMatchingGroups("Master/UI")[0];
    }

    private AudioSource GetNextSource()
    {
        currentSourceIndex = (currentSourceIndex + 1) % poolSize;
        return sfxSources[currentSourceIndex];
    }

    public AudioSource getMusicSource()
    {
        InitializeMusicSource();
        return musicSource;
    }

    /// <summary>
    /// Play 3D SFX at position (SFX audio group)
    /// </summary>
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource source = GetNextSource();

        source.transform.position = position;
        source.outputAudioMixerGroup = sfxGroup;
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(0.95f, 1.05f);
        source.Play();
    }

    /// </summary>
    /// Play a 2D SFX (UI audio group)
    /// </summary>
    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        AudioSource source = GetNextSource();

        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = uiGroup;
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }
    /// </summary>
    /// Play a music clip (Music audio group)
    /// </summary>
    public void PlayMusic(AudioClip music, float volume = 1f, bool loop = true)
    {
        // SetMusicVolume(0.6f);
        musicSource.clip = music;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(musicSource, 1f));
        musicSource.Stop();
    }

    /// <summary>
    /// Switch music source to 3D and set its location
    /// </summary>
    public void SetMusicLocation(Vector3 position)
    {
        musicSource.transform.position = position;
        musicSource.rolloffMode = AudioRolloffMode.Logarithmic;
        musicSource.maxDistance = 1000;
        musicSource.spatialBlend = 1f;
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume("MasterVolume", volume);
    }


    public void SetMusicVolume(float volume)
    {
        SetVolume("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume("SFXVolume", volume);
    }

    public void SetUIVolume(float volume)
    {
        SetVolume("UIVolume", volume);
    }

    private void SetVolume(string parameter, float volume)
    {
        // float minDb = -80f;
        // float maxDb = 0f;
        // float db = Mathf.Lerp(minDb, maxDb, volume);

        volume = Mathf.Pow(volume, 4f); // tweak exponent for volume curve (2–4 works well)
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        audioMixer.SetFloat(parameter, db);
    }

    private static IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop ();
        audioSource.volume = startVolume;
    }
}
