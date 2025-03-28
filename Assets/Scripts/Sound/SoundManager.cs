using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private GameObject bgmPrefab;
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] public AudioClip sfxClip;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Start()
    {
        // 프리팹을 DontDestroy에 생성
        GameObject bgmObject = Instantiate(bgmPrefab);
        GameObject sfxObject = Instantiate(sfxPrefab);

        DontDestroyOnLoad(bgmObject);
        DontDestroyOnLoad(sfxObject);
        
        // AudioSource 가져오기
        bgmSource = bgmObject.GetComponent<AudioSource>();
        sfxSource = sfxObject.GetComponent<AudioSource>();

        SceneManager.sceneLoaded += ChangeBGM;
        ChangeBGM(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null)
            return;

        if (bgmSource.clip == bgmClip) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null)
            return;

        sfxSource.PlayOneShot(sfxClip);
    }

    public void ChangeBGM(Scene scene, LoadSceneMode mode)
    {
        if (bgmClips.Length == 0)
            return;

        if (scene.name == "Login")
        {
            PlayBGM(bgmClips[0]);
        }
        else if (scene.name == "Main")
        {
            PlayBGM(bgmClips[0]);
        }
        else if (scene.name == "Game")
        {
            if (bgmClips.Length > 1)
                PlayBGM(bgmClips[1]);
        }
    }

    public void SetVolume(AudioSource source, float volume)
    {
        source.volume = volume;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeBGM;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
