using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public static AudioManager instance;
    public Sound[] sounds;

    private void Awake()
    {
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    private void Start()
    {
        Play(SceneManager.GetActiveScene().name);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        s.source.Play();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
}
