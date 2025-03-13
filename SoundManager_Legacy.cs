using System.Collections.Generic;
using UnityEngine;

public class SoundManager_Legacy : MonoBehaviour
{
    //내가 작성한 버전
    public static SoundManager_Legacy instance;

    [SerializeField] private List<AudioClip> bgmClips;
    [SerializeField] private List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> bgmList = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxList = new Dictionary<string, AudioClip>();

    [SerializeField] private AudioSource curBgm;
    [SerializeField] private AudioSource curSfx;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitSoundList();
        PlayBGM("BGM");
    }

    private void InitSoundList()
    {
        for (int i = 0; i < bgmClips.Count; i++)
            bgmList[bgmClips[i].name] = bgmClips[i];

        for (int j = 0; j < sfxClips.Count; j++)
            sfxList[sfxClips[j].name] = sfxClips[j];
    }

    public void PlayBGM(string bgmName, bool isLoop = true)
    {
        if (bgmList.ContainsKey(bgmName))
        {
            if (curBgm.clip != bgmList[bgmName])
            {
                curBgm.clip = bgmList[bgmName];
                curBgm.loop = isLoop;
                curBgm.Play();
            }
        }
        else
            Debug.LogError($"bgmList 에 {bgmName}에 해당하는 Audio 키 없음");
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxList.ContainsKey(sfxName))
        {
            curSfx.clip = sfxList[sfxName];
            curSfx.Play();
        }            
        else
            Debug.LogError($"sfxList 에 {sfxName}에 해당하는 Audio 키 없음");
    }
}
