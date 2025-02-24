using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected bool isBoss;
    [SerializeField] protected EnemyableData ableData;

    protected Player p;
    protected SpriteAnimation sa;
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    [HideInInspector] public EnemyData data = new EnemyData();

    float hitTimer = 0;
    float attackTimer = 0;


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
        {
            rb.velocity = Vector3.zero;
            return;
        }            

        if (p == null)
        {
            p = GameManager.Instance.P;
            return;
        }

        if (data.hp <= 0)
            return;

        if (hitTimer >= 0) 
        {
            hitTimer -= Time.deltaTime;
            return;
        }

        Vector2 pos = p.transform.position - transform.position;
        Vector2 dir = pos.normalized * Time.deltaTime * data.speed;

        //방향 체크
        sr.flipX = dir.normalized.x > 0 ? false : true;

        //거리 체크
        float dis = Vector2.Distance(p.transform.position, transform.position);
        if (dis > data.attackRange)
        {
            transform.Translate(dir);
        }
        else
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= data.attackSpeed)
            {
                attackTimer = 0;
                p.Damage(data.power);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (data.hp <= 0)
            return;

        //플레이어 공격인 경우,
        Bullet e = collision.GetComponent<Bullet>();
        if (e != null) 
        {
            //Destroy(collision.gameObject);

            CheckThrough(e);
            Hit(p.data.Power);
        }

        //플레이어 쉴드인 경우,
        Shield s = collision.GetComponentInParent<Shield>();
        if (s != null)
        {
            //Destroy(collision.gameObject);
            //p.RemoveShield(s);
            Hit(p.data.ShieldPower);
        }
    }

    private void Init()
    {
        List<Sprite> sprites = ResManager.Instance.enemySprite[data.enemyIndex].runSprite.ToList();
        sa.SetSprite(sprites, 0.25f / data.speed);

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().sortingOrder = 10;

        data.hp = ableData.Hp;
    }

    /// <summary>
    /// 관통 체크
    /// </summary>
    void CheckThrough(Bullet e)
    {
        e.ThroughCount++;

        int targetThroughValue = (e.gunType == GunType.Tool) ? p.data.ToolThroughValue : p.data.GunThroughValue;
        PoolKey poolKey = (e.gunType == GunType.Tool) ? PoolKey.ToolBullet : PoolKey.GunBullet;

        if (e.ThroughCount >= targetThroughValue)
        {
            e.ThroughCount = 0;
            ObjectPoolManager.Instance.Hide(poolKey, e.gameObject);
        }
    }

    void KnockBack()
    {
        Vector2 dir = p.transform.position - transform.position;

        rb.velocity = new Vector2(0, 0);
        rb.AddForce(-dir.normalized * 1, ForceMode2D.Impulse);
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(1f);

        PoolKey poolKey = (PoolKey)Enum.Parse(typeof(PoolKey), $"Enemy{data.enemyIndex}");
        ObjectPoolManager.Instance.Hide(poolKey, gameObject);

        Init();
    }

    void Hit(int power)
    {
        int randIndex = UnityEngine.Random.Range(0, 2);
        //SoundManager.instance.PlaySFX($"Hit{randIndex}");

        SoundClip targetClip = (SoundClip)Enum.Parse(typeof(SoundClip), $"Hit{randIndex}");
        SoundManager.Instance.PlaySFX(targetClip);
        
        float hitTime = 0.1f;

        //체력 깎음 
        data.hp -= power;
        if (isBoss)
            UI.Instance.RefreshBossHp(data.hp, data.maxHp);

        if (data.hp <= 0)
        {
            Dead();

            if (isBoss)
                Destroy(gameObject, 1f);
            else
                StartCoroutine(Remove());
            
            return;
        }

        //맞는 애니
        List<Sprite> sprite = ResManager.Instance.enemySprite[data.enemyIndex].hitSprite.ToList();
        sa.SetSprite(sprite, 0.1f, Run);

        KnockBack();

        hitTimer = hitTime;
    }

    void Run()
    {
        if (data.hp <= 0)
            return;

        //달리는 애니
        List<Sprite> sprite = ResManager.Instance.enemySprite[data.enemyIndex].runSprite.ToList();
        sa.SetSprite(sprite, 0.25f / data.speed);
    }

    void Dead()
    {
        //SoundManager.instance.PlaySFX("Dead");
        SoundManager.Instance.PlaySFX(SoundClip.Dead);

        rb.velocity = new Vector2(0, 0);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        //죽는 애니
        List<Sprite> sprite = ResManager.Instance.enemySprite[data.enemyIndex].deadSprite.ToList();
        sa.SetSprite(sprite, 0.2f);
               
        if (isBoss)
        {
            //보스 타임 종료 & 경험치 박스 드랍 
            TimeManager.Instance.EndBossTime();
            ExpBoxManager.Instance.DropBox(transform.position);
            
            //보스몹 KillCount ++
            GameManager.Instance.P.data.KillCount_Boss++;
        }            
        else
        {
            //경험치 드랍 
            int index = data.exp < 50 ? 0 : data.exp < 100 ? 1 : 2;
            ExpBoxManager.Instance.DropExp(index, data.exp, transform.position);

            //일반몹 KillCount ++
            GameManager.Instance.P.data.KillCount++;
        }
    }
}
