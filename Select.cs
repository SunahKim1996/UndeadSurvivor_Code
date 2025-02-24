using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Select : MonoBehaviour
{
    [SerializeField] private Image titleImg;
    [SerializeField] private Image[] panels;

    // Start is called before the first frame update
    void Start()
    {
        //SoundManager.instance.PlayBGM("BGM");
        SoundManager.Instance.PlayBGM(SoundClip.BGM, false);

        GenerateCard();

        titleImg.transform.DOLocalMoveX(0, 2f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => { 
                StartCoroutine(MovePanel());
            });
    }

    void GenerateCard()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            ResManager rm = ResManager.Instance;

            // �⺻ ĳ���Ͱ� �ƴ� ���
            if (!rm.charSprite[i].isDefaultCharacter)
            {
                string key = $"UnlockCharacter{i}";
                bool isUnlocked = PlayerPrefs.HasKey(key) && bool.Parse(PlayerPrefs.GetString(key));

                // �رݵ��� ���� ���
                if (!isUnlocked)
                {
                    LockedCard(i);
                    continue;
                }
            }

            // �⺻ ĳ���� or �رݵ� ��� 
            UnlockedCard(i);
        }
    }
    
    void LockedCard(int index)
    {
        ResManager rm = ResManager.Instance;
        Image panel = panels[index];
        SelectPanel targetPanel = panel.gameObject.GetComponent<SelectPanel>();

        panel.color = new Color32(32, 32, 32, 255);
        targetPanel.icon.sprite = rm.charSprite[index].charSprite.standSprite[0];
        targetPanel.icon.color = Color.black;
        targetPanel.nameText.gameObject.SetActive(false);
        targetPanel.lockConditionText.gameObject.SetActive(true);
        targetPanel.lockConditionText.text = rm.charSprite[index].unlockCondition;
        targetPanel.targetNum = rm.charSprite[index].targetNum;

        //targetPanel.selectButton.interactable = false;
        targetPanel.selectText.gameObject.SetActive(false);
    }
    void UnlockedCard(int index)
    {
        ResManager rm = ResManager.Instance;
        Image panel = panels[index];
        SelectPanel targetPanel = panel.gameObject.GetComponent<SelectPanel>();

        panel.color = rm.charSprite[index].color;
        targetPanel.icon.sprite = rm.charSprite[index].charSprite.standSprite[0];
        targetPanel.icon.color = Color.white;
        targetPanel.nameText.gameObject.SetActive(true);
        targetPanel.nameText.text = rm.charSprite[index].name;
        targetPanel.statText.text = rm.charSprite[index].stat;
        targetPanel.lockConditionText.gameObject.SetActive(false);
        targetPanel.targetNum = rm.charSprite[index].targetNum;

        //targetPanel.selectButton.interactable = true;
        targetPanel.selectText.gameObject.SetActive(true);
    }

    IEnumerator MovePanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].transform.DOLocalMoveY(-75, 1f)
                .SetEase(Ease.OutCirc);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
