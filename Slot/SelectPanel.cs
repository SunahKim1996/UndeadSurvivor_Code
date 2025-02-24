using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectPanel : MonoBehaviour
{
    public int targetNum;

    public Image icon;
    public TMP_Text nameText;
    public TMP_Text statText;
    public TMP_Text selectText;
    public TMP_Text lockConditionText;

    public void OnGame()
    {
        // 카드 선택 가능 
        if (!selectText.IsActive())
        {
            transform.DOShakePosition(0.5f, new Vector3(15f, 0, 0), 10, 90f, false, true);
            return;
        }

        //카드 선택 불가 
        else
        {
            //SoundManager.instance.PlaySFX("Select");
            SoundManager.Instance.PlaySFX(SoundClip.Select);
            transform.SetSiblingIndex(10);

            transform.DOScale(Vector3.one * 1.2f, 0.5f)
                .OnComplete(() =>
                {
                    GameManager.Instance.selectNum = targetNum;

                    // Game 으로 이동
                    SceneChanger.Instance.Game();
                    GameManager.Instance.gameState = GameState.Play;
                });
        }
    }
}
