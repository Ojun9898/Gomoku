using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KiboPanelController : MonoBehaviour
{
    [SerializeField] private GameObject KiboPanel;
    [SerializeField] private TMP_Text UpdateTimeText;

    private NotationManager nm;

    void Start()
    {

    }

    public void UpdateTime()
    {
        UpdateTimeText.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        UpdateTimeText.ForceMeshUpdate();
    }

    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false));
    }
}
