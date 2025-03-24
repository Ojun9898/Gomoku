using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsPanel : MonoBehaviour
{
    public AudioMixer audioMixer; // Audio Mixer 참조
    public Slider musicSlider;
    public Slider effectSlider;

    void Start()
    {
        // 슬라이더 값 변경 시 볼륨 적용
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);

        // 저장된 볼륨 값 불러오기
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float effectVol = PlayerPrefs.GetFloat("EffectVolume", 0.75f);
        musicSlider.value = musicVol;
        effectSlider.value = effectVol;
        SetMusicVolume(musicVol);
        SetEffectVolume(effectVol);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
}