using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ObjectPoolManager;

public enum SoundClip
{
    BGM,
    Dead,  
    Hit0, 
    Hit1,
    LevelUp,
    Lose,
    Melee0, 
    Melee1,
    Range, 
    Select,
    Win,
}

public class SoundManager : Singleton<SoundManager>
{
    public List<AudioClip> clips;

    public AudioSource bgmSound;
    public AudioSource sfxSound;

    public Transform sfxParent;

    public Queue<AudioSource> pool;

    //public List<AudioSource> effSoundList; 
    //int soundIndex = 0;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        pool = new Queue<AudioSource>();
    }

    // SFX ==================================================================================
    public void PlaySFX(SoundClip clip)
    {
        AudioSource audioSource;

        if (pool.Count == 0)
        {
            audioSource = Instantiate(sfxSound, sfxParent);
        }
        else
        {
            audioSource = pool.Dequeue();
            audioSource.transform.parent = sfxParent;
        }

        audioSource.clip = clips[(int)clip];
        audioSource.Play();

        StartCoroutine(SoundStop(audioSource));
    }

    IEnumerator SoundStop(AudioSource audioSource)
    {
        while (audioSource.isPlaying)
            yield return null;

        pool.Enqueue(audioSource);
    }

    // BGM ==================================================================================

    public void PlayBGM(SoundClip clip, bool isLoop)
    {
        if (bgmSound.clip == clips[(int)clip])
            return;

        bgmSound.clip = clips[(int)clip];
        bgmSound.loop = isLoop;

        bgmSound.Play();
    }

    /* LEGACY ÄÚµå
    public void PlaySFX(SoundClip clip)
    {
        effSoundList[soundIndex].clip = clips[(int)clip];
        effSoundList[soundIndex].Play();
        soundIndex++;

        if (soundIndex > effSoundList.Count) 
        {
            soundIndex = 0;
        }
    }
    */

    public void BGMSound(bool isOn, float volume)
    {
        if (isOn)
            bgmSound.volume = volume;
        else
            bgmSound.volume = 0;
    }
}
