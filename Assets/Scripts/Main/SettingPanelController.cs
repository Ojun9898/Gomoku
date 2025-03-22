using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
   
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private Button closeButton;

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
}
