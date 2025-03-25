using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
   
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;

    public void OnValueChangedMusicSlider()
    {
       // music volume 조절
    }

    public void OnValueChangedEffectSlider()
    {
        // effect volume 조절
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
