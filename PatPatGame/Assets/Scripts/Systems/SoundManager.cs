using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Unity.Mathematics;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup sfxGroup;
    private AudioMixerGroup uiGroup;

    [Header("SFX Pool")]
    public int poolSize = 10;
    public AudioClip musicTest;
    public AudioClip sfxTest;
    private List<AudioSource> sfxSources;
    private AudioSource musicSource;

    private int currentSourceIndex = 0;

    public void Initialize()
    {
        InitializeMixerGroups();
        InitializeAudioPool();
        InitializeMusicSource();
        PlayMusic(musicTest, 1, true);
        InvokeRepeating(nameof(PlaySoundTest), 1, 5);
    }

    private void PlaySoundTest()
    {
        SetMusicVolume(0.1f);
        PlaySoundAtPosition(sfxTest, new Vector3(0,0,0));
    }

    void InitializeAudioPool()
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

    void InitializeMusicSource()
    {
        GameObject musicObj = new GameObject("Music_Source");
        musicObj.transform.parent = transform;

        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 2D sound
    }

    void InitializeMixerGroups()
    {
        musicGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
        sfxGroup = audioMixer.FindMatchingGroups("Master/SFX")[0];
        uiGroup = audioMixer.FindMatchingGroups("Master/UI")[0];
    }

    AudioSource GetNextSource()
    {
        currentSourceIndex = (currentSourceIndex + 1) % poolSize;
        return sfxSources[currentSourceIndex];
    }

    // Play 3D SFX at position
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource source = GetNextSource();

        source.transform.position = position;
        source.outputAudioMixerGroup = sfxGroup;
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        AudioSource source = GetNextSource();

        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = uiGroup;
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public void PlayMusic(AudioClip music, float volume = 1f, bool loop = true)
    {
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.clip = music;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Master volume control (AudioMixer exposed param: "MasterVolume")
    public void SetMasterVolume(float volume)
    {
        // volume expected 0.0001f - 1f
        float db = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MasterVolume", db);
    }

    void SetVolume(string parameter, float volume)
    {
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(parameter, db);
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
}
