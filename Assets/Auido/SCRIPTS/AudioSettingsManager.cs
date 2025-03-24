using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider effectSlider;

    private const string MUSIC_PARAM = "Music";
    private const string EFFECT_PARAM = "Effect";

    void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);

        LoadSettings();
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(MUSIC_PARAM, volume);
        PlayerPrefs.SetFloat(MUSIC_PARAM, value);
        PlayerPrefs.Save();

        // 🔍 디버그 로그 추가
        Debug.Log($"[SetMusicVolume] Slider Value: {value}, Converted dB: {volume}");
    }

    public void SetEffectVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(EFFECT_PARAM, volume);
        PlayerPrefs.SetFloat(EFFECT_PARAM, value);
        PlayerPrefs.Save();

        // 🔍 디버그 로그 추가
        Debug.Log($"[SetEffectVolume] Slider Value: {value}, Converted dB: {volume}");
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PARAM, 1f);
        float effectVolume = PlayerPrefs.GetFloat(EFFECT_PARAM, 1f);

        musicSlider.value = musicVolume;
        effectSlider.value = effectVolume;

        SetMusicVolume(musicVolume);
        SetEffectVolume(effectVolume);

        // 🔍 초기값 로드 확인
        Debug.Log($"[LoadSettings] Music: {musicVolume}, Effect: {effectVolume}");
    }
}