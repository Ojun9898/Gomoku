using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private CanvasGroup canvasGroup;

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

    public void OnClickYesButton()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(0, 0.5f).OnComplete(() => //투명
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });

        MainManager.Instance.ShowEndGamePanel();
    }
    public void OnClickNoButton()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(0, 0.5f).OnComplete(() => //투명
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }



}
