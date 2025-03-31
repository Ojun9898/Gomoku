using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KiboPanelController : MonoBehaviour
{
    [SerializeField] private GameObject KiboPanel;
    [SerializeField] private TMP_Text UpdateTimeText;
    [SerializeField] private GameObject Kibo;
    [SerializeField] private Transform KibosTransform;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject FadePanel;


    void OnEnable()
    {
        // 기보 : 기보 수 만큼 Kibo 생성 초기화
        NotationManager.Instance.PrintRecentFileNames();
        var fileNames = NotationManager.Instance.recentFileNames;
        for (int i = 0; i < NotationManager.Instance.recentFiles.Count; i++) {
            GameObject kibo = Instantiate(Kibo, KibosTransform);
            kibo.name = i.ToString();
            var texts = kibo.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0) { 
                texts[0].text = fileNames[i].Item1;
                texts[1].text = fileNames[i].Item2;
            }
        }

    }

    private void OnDisable()
    {
        foreach (Transform child in KibosTransform)
        {
            Destroy(child.gameObject);
        }
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

    public void OnClickChangeSceneButton() { 
        int clickedButtonIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.name);
        if(clickedButtonIndex >= 0 && clickedButtonIndex <10)
        {
            canvasTransform = FindAnyObjectByType<Canvas>().transform;
            // 누른 버튼의 인덱스로 해당하는 파일의 데이터를 읽어와 NotationManager에 저장
            NotationManager.Instance.LoadDataFromFile(clickedButtonIndex);
          
            var _FadePanel = Instantiate(FadePanel, canvasTransform);
             
            _FadePanel.GetComponent<CanvasGroup>().alpha = 0;

            _FadePanel.GetComponent<CanvasGroup>().DOFade(1, 1.5f).OnComplete(() =>
            {
                SceneManager.LoadScene("Notation");
            });
        }
    }
}
