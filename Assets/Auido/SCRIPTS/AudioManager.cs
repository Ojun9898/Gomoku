using UnityEngine;
using UnityEngine.SceneManagement; // 씬 변경 감지

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("🔊 Audio Sources")]
    public AudioSource bgmSource; // 배경음악용 (하나만 사용)
    public AudioSource sfxSource; // 효과음용

    [Header("🎵 BGM Clips")]
    public AudioClip mainBGM;
    public AudioClip gameBGM;

    [Header("🎶 SFX Clips")]
    public AudioClip buttonClickSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip gameStartSound;
    public AudioClip gameOverSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경돼도 유지
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            Destroy(listeners[1]); // 두 번째 AudioListener 제거
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void UpdateBGM(string sceneName)
    {
        if (sceneName == "MainScene")
        {
            PlayBGM(mainBGM);
        }
        else if (sceneName == "GameScene")
        {
            PlayBGM(gameBGM);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBGM(scene.name);
    }


    // 🎵 배경음악 변경 (기존 오디오 소스 재사용)
    public void PlayMainBGM()
    {
        PlayBGM(mainBGM);
    }

    public void PlayGameBGM()
    {
        PlayBGM(gameBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return; // 같은 BGM이면 변경하지 않음

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // 🔊 효과음 (SFX) 재생 메서드
    public void PlayButtonSound()
    {
        sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayAttackSound()
    {
        sfxSource.PlayOneShot(attackSound);
    }

    public void PlayHitSound()
    {
        sfxSource.PlayOneShot(hitSound);
    }

    public void PlayGameStartSound()
    {
        sfxSource.PlayOneShot(gameStartSound);
    }

    public void PlayGameOverSound()
    {
        sfxSource.PlayOneShot(gameOverSound);
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
