using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
   
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Start()
    {
        bgmSource = GameObject.FindWithTag("BGMSource")?.GetComponent<AudioSource>();
        sfxSource = GameObject.FindWithTag("SFXSource")?.GetComponent<AudioSource>();
    }

    public void OnValueChangedBgmSlider()
    {
       SoundManager.Instance.SetVolume(bgmSource, bgmSlider.value);
    }

    public void OnValueChangedSfxSlider()
    {
        SoundManager.Instance.SetVolume(sfxSource, sfxSlider.value);
    }

    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

    public void OnClickLogoutButton()
    {
        MainManager.Instance.ShowLogoutPanel();
    }
}
