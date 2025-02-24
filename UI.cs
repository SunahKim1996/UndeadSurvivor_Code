using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;
using System;

public class UI : Singleton<UI>
{
    [Header("Player HUD")]
    public Image icon;
    public TMP_Text lvTxt;
    public TMP_Text nameTxt;
    public Image hpImg;
    public Image hpdumyImg;
    public Image expImg;

    public TMP_Text killTxt;

    [Header("Poppup")]
    public GameObject popup;
    public GameObject lvPopup;
    public GameObject overPopup;
    public Image overTitle;
    public GameObject endUI;

    [Header("Toast Popup")]
    public Transform toastTrans;
    public TMP_Text toastTxt;

    public Queue<string> quStr = new Queue<string>();
    private bool toastPlay = false;

    [Header("Levelup Popup")]
    [SerializeField] private List<ItemSelectSlot> itemSelectSlotList;

    [Header("Inventory")]
    public List<InvenSlot> invenSlotList;
    public List<ItemData> items;
    private List<ItemData> dumyItem = new List<ItemData>();

    [Header("Timer")]
    [SerializeField] private TMP_Text timerTxt;
    [SerializeField] private TMP_Text stageTxt;
    [SerializeField] private Image bossTimeImg;

    [Header("Joystick")]
    public FloatingJoystick joystick;

    [Header("Boss")]
    [SerializeField] private GameObject bossHpUI;
    [SerializeField] private Image bossHp;
    [SerializeField] private Image bossDummyHp;
    [SerializeField] private Image bossNotice;

    [Header("Ranking")]
    [SerializeField] private List<RecordSlot> recordSlotList;

    // Start is called before the first frame update
    void Start()
    {
        GameManager gm = GameManager.Instance;
        ResManager rm = ResManager.Instance;

        //캐릭터 얼굴 변경
        int index = GameManager.Instance.selectNum;
        Sprite icon = ResManager.Instance.uiProfileIcon[index];
        this.icon.sprite = icon;

        killTxt.text = "0";

        for (int i = 0; i < invenSlotList.Count; i++)
            invenSlotList[i].lvText.gameObject.SetActive(false);

        for (int i = 0; i < items.Count; i++)
            items[i].Lv = 0;

        /*
        foreach (var invenItem in invenImgs)
        {
            invenItem.gameObject.SetActive(false);
        }
        */

        UILevel(gm.P.data.Level);
        nameTxt.text = $"{rm.charSprite[index].name}";
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        if (quStr.Count != 0)
        {
            if (toastPlay == false)
            {
                toastPlay = true;
                toastTxt.text = quStr.Dequeue();
                toastTrans.DOMoveY(80, 0.6f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        toastTrans.DOMoveY(-20f, 0.3f)
                        .SetEase(Ease.Linear)
                        .SetDelay(2f)
                        .OnComplete(() => { toastPlay = false; });
                    });
            }
        }
    }

    // 타이머 & 스테이지 --------------------------------------------------------------
    public void StageUI(int curStage)
    {
        stageTxt.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        stageTxt.text = $"{curStage} 스테이지";

        stageTxt.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce);
    }

    void ToggleBossTimerUI(bool isBossTime)
    {
        timerTxt.enabled = !isBossTime;
        bossTimeImg.enabled = isBossTime;
    }

    public void TimeUI(float curTime, bool isBossTime = false)
    {
        if (isBossTime) 
        {
            ToggleBossTimerUI(true);
            return;
        }

        ToggleBossTimerUI(false);

        int minutes = Mathf.FloorToInt(curTime / 60);
        int seconds = Mathf.FloorToInt(curTime % 60);

        timerTxt.text = $"{minutes:00}:{seconds:00}";
        timerTxt.color = Color.white;
    }

    // 플레이어 HUD -----------------------------------------------------------------
    public void UILevel(int level)
    {
        lvTxt.text = $"Lv.{level}";
    }

    public void UIExp(float exp, float maxExp)
    {
        float scale = (exp / maxExp) * 180f;
        expImg.rectTransform.sizeDelta = new Vector2(scale, 30f);
    }

    public void UIKillCount(int count)
    {
        killTxt.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        killTxt.text = $"{count}";

        killTxt.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce);
    }

    public void RefreshHP(bool isAnimation = true)
    {
        float hp = GameManager.Instance.P.data.Hp;
        float maxhp = GameManager.Instance.P.data.MaxHp;

        float scaleHP = (hp / maxhp) * 180f;
        hpImg.rectTransform.sizeDelta = new Vector2(scaleHP, 30f);

        if (isAnimation)
        {
            hpdumyImg.rectTransform.DOSizeDelta(new Vector2(scaleHP, 30f), 2f)
                .SetEase(Ease.Unset);
        }
        else
        {
            hpdumyImg.rectTransform.sizeDelta = new Vector2(scaleHP, 30f);
        }
    }

    // 아이템 관련 ------------------------------------------------------------------
    public void ShowLevelUp()
    {      
        Player p = GameManager.Instance.P;
        dumyItem = items.ToList();

        //0 - 실드 6개일 때 나오지 않게 처리
        if (p.shieldList.Count >= 6)
            dumyItem.RemoveAt(0);

        //5 - 총구 5개일 때 나오지 않게 처리
        if (p.data.GunEntranceCount >= 5)
            dumyItem.RemoveAt(5);

        /*
        //인벤 꽉 찼을 때 처리 
        bool isInvelFull = invenImgs[invenImgs.Count - 1].IsActive() ? true : false;

        if (isInvelFull)
        {
            List<ItemData> itemList = new List<ItemData>();

            for (int i = 0; i < invenImgs.Count; i++)
            {
                for (int j = 0; j < dumyItem.Count; j++) 
                { 
                    if (invenImgs[i].sprite.name.Equals(dumyItem[j].Icon.name))
                    {
                        itemList.Add(dumyItem[j]);
                        break;
                    }
                }   
            }

            dumyItem.Clear();
            dumyItem = itemList.ToList();
        }
        */
        while (dumyItem.Count > 3)
        {
            int rand = UnityEngine.Random.Range(0, dumyItem.Count);
            dumyItem.RemoveAt(rand);
        }

        //---------------리스트 안의 데이터 섞기---------------
        dumyItem = Shuffle(dumyItem);
        List<ItemData> Shuffle(List<ItemData> list)
        {
            return list.OrderBy(_ => UnityEngine.Random.Range(0, list.Count)).ToList();
        }
        //-----------------------------------------------------

        for (int i = 0; i < dumyItem.Count; i++)
        {
            ItemSelectSlot slot = itemSelectSlotList[i];
            slot.buttonIndex = i;
            slot.icon.sprite = dumyItem[i].Icon;
            slot.nameText.text = dumyItem[i].ItemName;
            slot.descText.text = dumyItem[i].Desc;

            if (dumyItem[i].ItemType == ItemType.Etc)
                slot.lvText.gameObject.SetActive(false);
            else
            {
                slot.lvText.gameObject.SetActive(true);
                slot.lvText.text = $"Lv.{dumyItem[i].Lv + 1}";
            }

            Button slotButton = slot.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => OnSelectItem(slot.buttonIndex));

            /*
            if (invenLevelDict.ContainsKey(dumyItem[i].Index))
            {
                itemSelectSlotList[i].lvTxt.text = $"Lv.{invenLevelDict[dumyItem[i].Index]}";
            }
            else
            {
                itemSelectSlotList[i].lvTxt.text = $"Lv.1";
            }
            */
        }

        popup.SetActive(true);
        overPopup.SetActive(false);
                
        lvPopup.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        lvPopup.SetActive(true);

        lvPopup.transform.DOScale(1f, 0.6f)
            .SetUpdate(true)
            .SetEase(Ease.OutBounce);
    }

    
    /// <summary>
    /// 아이템 선택
    /// </summary>
    public void OnSelectItem(int buttonIndex)
    {
        //SoundManager.instance.PlaySFX("Select");
        SoundManager.Instance.PlaySFX(SoundClip.Select);

        ApplyItemEffect(buttonIndex);
        RefreshInven(buttonIndex);

        lvPopup.transform.DOScale(0.4f, 0.15f)
            .SetUpdate(true)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameManager.Instance.gameState = GameState.Play;

                popup.SetActive(false);
                overPopup.SetActive(false);
                lvPopup.SetActive(false);
            });
    }

    /// <summary>
    /// 아이템 효과 적용 
    /// </summary>
    void ApplyItemEffect(int buttonIndex)
    {
        ItemData targetItem = dumyItem[buttonIndex];
        Player p = GameManager.Instance.P;

        if (targetItem.ItemType == ItemType.Gun)
            p.gunHand.SetActive(true);

        switch (targetItem.Index)
        {
            // 주인공 주위로 빙글빙글 1개추가
            case 0:
                p.AddShield();
                break;

            // 농기구 발사 속도 10% 증가
            case 1:
                p.data.ToolAtkDelay -= p.data.BaseToolAtkDelay * 0.1f;
                break;

            // 농기구 관통 1 증가
            case 2:
                p.data.ToolThroughValue++;
                break;

            // 총 관통 1 증가
            case 3:
                p.data.GunThroughValue++;
                break;

            // 총 발사 속도 10% 증가
            case 4:
                p.data.GunAtkDelay -= p.data.BaseGunAtkDelay * 0.1f;
                break;

            // 총구 1 증가
            case 5:
                p.data.GunEntranceCount++;
                break;

            // 최대 체력 10 증가
            case 6:
                p.data.MaxHp += 10;
                break;

            // 이동 속도 10% 증가
            case 7:
                p.data.Speed += (p.data.BaseSpeed * 0.1f);
                break;

            // 전체 체력 회복 
            case 8:
                p.data.Hp = p.data.MaxHp;
                break;
        }
    }

    InvenSlot GetTargetSlot(int itemIndex)
    {
        for (int i = 0; i < invenSlotList.Count; i++)
        {
            if (invenSlotList[i].itemIndex == itemIndex)
                return invenSlotList[i];
        }

        return null;
    }

    InvenSlot GetEmptySlot()
    {
        for (int i = 0; i < invenSlotList.Count; i++)
        {
            if (invenSlotList[i].itemIndex == -1)
                return invenSlotList[i];
        }

        return null;
    }

    /// <summary>
    /// 인벤 UI 갱신 
    /// </summary>
    void RefreshInven(int buttonIndex)
    {
        ItemData targetItem = dumyItem[buttonIndex];

        if (targetItem.ItemType == ItemType.Etc)
            return;

        InvenSlot targetSlot = GetTargetSlot(targetItem.Index);

        // 인벤토리에 없으면 생성
        if (targetSlot == null)
        {
            targetSlot = GetEmptySlot();            
            if (targetSlot == null)
            {
                Debug.LogError("Inventory Full");
                return;
            }

            targetSlot.itemIndex = targetItem.Index;
            targetSlot.icon.sprite = targetItem.Icon;
            targetSlot.lvText.gameObject.SetActive(true);
        }

        targetItem.Lv++;
        targetSlot.lvText.text = $"Lv.{targetItem.Lv}";

        //LEGACY Code
        /*        
        bool isCheck = false;

        // inventory 에 있는지 체크 
        foreach (var item in invenSlotList)
        {
            if (item.IsActive() && item.sprite.name.Equals(dumyItem[index].Icon.name))
            {
                int key = dumyItem[index].Index;
                invenLevelDict[key]++;

                item.sprite = dumyItem[index].Icon;
                item.gameObject.SetActive(true);
                isCheck = true;
                break;
            }
        }
        

        //없다면 새로 추가 
        if (isCheck == false)
        {
            foreach (var item in invenImgs)
            {
                if (!item.IsActive())
                {
                    int key = dumyItem[index].Index;
                    invenLevelDict.Add(key, 1);
                    invenLevelDict[key]++;

                    item.sprite = dumyItem[index].Icon;
                    item.gameObject.SetActive(true);

                    if (dumyItem[index].ItemType == ItemType.Gun)
                        GameManager.Instance.P.gunHand.SetActive(true);

                    break;
                }
            }
        }
        */
    }

    // 보스 -----------------------------------------------------------------------
    public void ShowBossUI()
    {
        bossNotice.rectTransform.anchoredPosition = new Vector3(-1000, 0, 0);
        bossNotice.gameObject.SetActive(true);
        bossNotice.transform.DOLocalMoveX(0, 1f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                bossNotice.transform.DOLocalMoveX(1200, 1f)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    bossNotice.gameObject.SetActive(false);
                });
            });

        //보스 HP 
        ToggleBossUI(true);
    }

    public void RefreshBossHp(float hp, float maxHp)
    {
        bossHp.fillAmount = hp / maxHp;
        bossDummyHp.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.Unset);
    }

    public void ToggleBossUI(bool state)
    {
        bossHpUI.SetActive(state);
        stageTxt.gameObject.SetActive(!state);

        if (state)
            RefreshBossHp(1, 1);
    }

    // 기타 -------------------------------------------------------------------------

    public void ToastShow(string str)
    {
        quStr.Enqueue(str);
    }

    public void ShowDeadUI()
    {
        popup.SetActive(true);
        overPopup.SetActive(true);
        lvPopup.SetActive(false);

        overTitle.rectTransform.anchoredPosition = new Vector2(0, 200);
        overTitle.transform.DOLocalMoveY(0, 1.5f)
            .OnComplete(() =>
            {
                endUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -700);
                endUI.SetActive(true);
                endUI.transform.DOLocalMoveY(0, 1f)
                    .SetDelay(0.2f)
                    .SetEase(Ease.OutCubic);
            });

        GameManager.Instance.gameState = GameState.Stop;

        int killCount = GameManager.Instance.P.data.KillCount;
        float timer = TimeManager.Instance.CurTime;
        RankManager.Instance.SetRecord(killCount, timer);
    }

    /// <summary>
    /// 랭킹 기록 
    /// </summary>
    public void RefreshRankUI(List<string> recordList)
    {
        for (int i = 0; i < recordSlotList.Count; i++) 
        { 
            if (i < recordList.Count)
            {
                string[] recordArray = recordList[i].Split('/');
                recordSlotList[i].killCount.text = $"{recordArray[0]}";

                int minutes = Mathf.FloorToInt(float.Parse(recordArray[1]) / 60);
                int seconds = Mathf.FloorToInt(float.Parse(recordArray[1]) % 60);
                recordSlotList[i].timerRecord.text = $"{minutes:00}:{seconds:00}";

                recordSlotList[i].gameObject.SetActive(true);
            }
            else
            {
                recordSlotList[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnGameEnd()
    {
        SceneChanger.Instance.CharacterSelect();
    }
}
