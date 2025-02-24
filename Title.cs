using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField] private TMP_Text keyTxt;
    [SerializeField] private Image titleImg;
    [SerializeField] private GameObject btnObj;

    // Start is called before the first frame update
    void Start()
    {
        titleImg.transform.DOLocalMoveY(60, 1.5f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutElastic)
            .OnComplete(() => {
                StartCoroutine(CFade(true));
            });
    }

    IEnumerator CFade(bool isFade)
    {
        if (isFade)
        {
            Tween t = keyTxt.DOFade(0f, 1f);
            yield return t.WaitForCompletion(); //tween 이 끝날 때까지 기다림 
            btnObj.SetActive(true);

            StartCoroutine(CFade(false));
        }
        else
        {
            Tween t = keyTxt.DOFade(1f, 1f);
            yield return t.WaitForCompletion(); //tween 이 끝날 때까지 기다림 

            StartCoroutine(CFade(true));
        }
    }

    public void OnSelect()
    {
        SceneChanger.Instance.CharacterSelect();
    }
}
