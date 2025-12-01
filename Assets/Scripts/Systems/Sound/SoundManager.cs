using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip wheelStartSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip dragTileSound;
    [SerializeField] private AudioClip mergeSound;
    [SerializeField] private AudioClip rewardSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            float db = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat("MusicVolume", db);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            float db = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat("SFXVolume", db);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnNumberMerged += PlayMergeSound;
        GameEvents.OnGoalNumberReached += PlayGoalSound;
    }

    private void OnDisable()
    {
        GameEvents.OnNumberMerged -= PlayMergeSound;
        GameEvents.OnGoalNumberReached -= PlayGoalSound;
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayWheelStartSound()
    {
        PlaySFX(wheelStartSound);
    }

    public void PlayDragTileSound()
    {
        PlaySFX(dragTileSound);
    }

    public void PlayRewardSound()
    {
        PlaySFX(rewardSound);
    }

    private void PlayMergeSound(NumberTile tile)
    {
        PlaySFX(mergeSound);
    }

    private void PlayGoalSound(NumberTile tile)
    {
        PlaySFX(goalSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
