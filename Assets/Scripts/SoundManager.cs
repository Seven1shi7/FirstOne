using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop = false;
        [HideInInspector] public AudioSource source;
    }

    [Header("音效设置")]
    public Sound[] sounds;

    [Header("背景音乐设置")]
    public Sound[] musicTracks;
    private string currentMusicName;

    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSounds();
        InitializeMusic();
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            soundDictionary.Add(s.name, s);
        }
    }

    private void InitializeMusic()
    {
        foreach (Sound m in musicTracks)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
            musicDictionary.Add(m.name, m);
        }
    }
    // 播放音效
    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    // 停止音效
    public void StopSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Stop();
        }
    }

    // 播放背景音乐
    public void PlayMusic(string name, bool fade = false, float fadeDuration = 1f)
    {
        if (musicDictionary.TryGetValue(name, out Sound music))
        {
            if (currentMusicName == name && music.source.isPlaying)
                return;

            if (fade && !string.IsNullOrEmpty(currentMusicName))
            {
                StartCoroutine(FadeMusic(currentMusicName, 0f, fadeDuration, () => {
                    musicDictionary[currentMusicName].source.Stop();
                    currentMusicName = name;
                    music.source.Play();
                    if (fade) StartCoroutine(FadeMusic(name, music.volume, fadeDuration));
                }));
            }
            else
            {
                if (!string.IsNullOrEmpty(currentMusicName))
                    musicDictionary[currentMusicName].source.Stop();

                currentMusicName = name;
                music.source.Play();
                if (fade)
                {
                    music.source.volume = 0f;
                    StartCoroutine(FadeMusic(name, music.volume, fadeDuration));
                }
            }
        }
        else
        {
            Debug.LogWarning("Music: " + name + " not found!");
        }
    }

    // 停止背景音乐
    public void StopMusic(bool fade = false, float fadeDuration = 1f)
    {
        if (!string.IsNullOrEmpty(currentMusicName))
        {
            if (fade)
            {
                StartCoroutine(FadeMusic(currentMusicName, 0f, fadeDuration, () => {
                    musicDictionary[currentMusicName].source.Stop();
                    currentMusicName = null;
                }));
            }
            else
            {
                musicDictionary[currentMusicName].source.Stop();
                currentMusicName = null;
            }
        }
    }

    // 淡入淡出效果
    private IEnumerator FadeMusic(string name, float targetVolume, float duration, System.Action onComplete = null)
    {
        if (musicDictionary.TryGetValue(name, out Sound music))
        {
            float startVolume = music.source.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                music.source.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
                yield return null;
            }

            music.source.volume = targetVolume;
            onComplete?.Invoke();
        }
    }

    // 设置全局音量
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    // 设置音效音量
    public void SetSoundsVolume(float volume)
    {
        foreach (var sound in soundDictionary.Values)
        {
            sound.source.volume = sound.volume * Mathf.Clamp01(volume);
        }
    }

    // 设置音乐音量
    public void SetMusicVolume(float volume)
    {
        foreach (var music in musicDictionary.Values)
        {
            music.source.volume = music.volume * Mathf.Clamp01(volume);
        }
    }

    // 暂停所有声音
    public void PauseAll()
    {
        AudioListener.pause = true;
    }

    // 恢复所有声音
    public void ResumeAll()
    {
        AudioListener.pause = false;
    }
}