using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(SpriteRenderer))]

public class SpriteAnimation : MonoBehaviour
{
    SpriteRenderer sr;
    List<Sprite> sprites;
    UnityAction action;

    int index;
    float delay;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sprites == null)
        {
            return;
        }
        
        if (sprites.Count == 0)
        {
            return;
        }

        timer += Time.deltaTime;
         
        if(timer >= delay)
        {
            timer = 0;

            if (index >= sprites.Count - 1)
            {
                index = 0;

                if (action != null)
                {
                    sprites = null;
                    action();
                    action = null;
                    return;
                }
            }
            else
            {
                index++;
            }
            
            if (sprites.Count == 1)
            {
                sprites = null;
                action();
                action = null;
            }
            else
            {
                sr.sprite = sprites[index];
            }
        }
    }

    //���� �ݺ�
    public void SetSprite(List<Sprite> sprites, float delay)
    {
        this.sprites = sprites;
        this.delay = delay;
        timer = 0;
        index = 0;

        //Start ���� sr �� ����Ǳ� ���� sr.sprite �� ���� �Ҹ��Ƿ�, �Ʒ��� ���� ó��
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        sr.sprite = this.sprites[0];
    }

    //�������� �̺�Ʈ 
    public void SetSprite(List<Sprite> sprites, float delay, UnityAction action)
    {
        this.sprites = sprites;
        this.delay = delay;
        this.action = action;
        timer = 0;
        index = 0;

        //Start ���� sr �� ����Ǳ� ���� sr.sprite �� ���� �Ҹ��Ƿ�, �Ʒ��� ���� ó��
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        sr.sprite = this.sprites[0];
    }
}
