using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Player : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private Transform toolFirePos;
    [SerializeField] private Bullet toolBullet;

    [Space(1)]
    public Transform[] gunFirePos;
    [SerializeField] private Bullet gunBullet;

    [Space(1)]
    [SerializeField] private Transform bulletTrans;

    [Header("Shield")]
    [SerializeField] private Transform shieldParent;
    [SerializeField] private Shield shield;

    [HideInInspector] public List<Shield> shieldList = new List<Shield>();

    [Header("Others")]
    [HideInInspector] public PlayerData data = new PlayerData();
    SpriteAnimation sa;
    GameManager gm;

    int charNnum = 0;
    float toolFireTimer, gunFireTimer = 0;

    [Header("Weapon Hand")]
    public GameObject gunHand;
    [SerializeField] private GameObject toolHand;

    // Start is called before the first frame update
    void Start()
    {
        charNnum = GameManager.Instance.selectNum;
        sa = GetComponent<SpriteAnimation>();
        sa.SetSprite(ResManager.Instance.charSprite[charNnum].charSprite.standSprite.ToList(), 0.2f);

        InitData();
        InitObjPool();
        InitAbility();
        RefreshGunEntrance();
    }  

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        //이동 
        Move();

        //도구 공격 
        toolFireTimer += Time.deltaTime;
        if (toolFireTimer > GameManager.Instance.P.data.ToolAtkDelay)
        {
            toolFireTimer = 0;
            ToolAttack();
        }

        //총 공격 
        if (gunHand.activeSelf)
        {
            gunFireTimer += Time.deltaTime;

            if (gunFireTimer > GameManager.Instance.P.data.GunAtkDelay)
            {
                gunFireTimer = 0;
                GunAttack();
            }
        }

        //실드 
        if (shieldList.Count > 0)
            shieldParent.Rotate(Vector3.back * Time.deltaTime * data.ShieldSpeed);
    }

    // 기초 세팅 -----------------------------------------------------------------------------------
    void InitData()
    {
        data.Speed = data.BaseSpeed = 3;
        data.Hp = data.MaxHp = 50;
        data.Exp = 0f;
        data.MaxExp = 50f;
        data.Level = 1;
        data.ToolAtkDelay = data.BaseToolAtkDelay = 1f;
        data.GunAtkDelay = data.BaseGunAtkDelay = 0.8f;
        data.Power = 20;
        data.ShieldSpeed = 80;
        data.ShieldPower = 1;
        data.ToolThroughValue = data.GunThroughValue = 1;
        data.GunEntranceCount = 1;
    }

    void InitObjPool()
    {
        ObjectPoolManager.Instance.Init(PoolKey.ToolBullet, bulletTrans, toolBullet.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.GunBullet, bulletTrans, gunBullet.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.Shield, shieldParent, shield.gameObject);
    }

    void InitAbility()
    {
        switch (charNnum)
        {
            case 0:
                data.Speed += (data.BaseSpeed * 0.1f);
                break;
            case 1:
                data.ToolAtkDelay -= (data.ToolAtkDelay * 0.1f);
                break;
            case 2:
                data.Power += (int)((float)data.Power * 0.2f);
                break;
            case 3:
                data.ToolThroughValue++;
                break;
        }
    }   

    // 공격 ----------------------------------------------------------------------------------------
    void ToolAttack()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies.Length == 0)
            return;

        //가까운 적 찾기
        float dis = 5; 
        Enemy e = null;
        foreach (var enemy in enemies)
        {
            if (enemy.data.hp <= 0)
                continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < dis)
            {
                dis = distance;
                e = enemy;
            }
        }

        //가장 가까운 적 찾은 후 공격 
        if (e == null)
            return;

        SetRotFirePos(e.transform);

        //Bullet b = Instantiate(bullet, firePos);
        //b.transform.SetParent(bulletTrans);

        int randIndex = UnityEngine.Random.Range(0, 2);
        //SoundManager.instance.PlaySFX($"Melee{randIndex}");

        SoundClip targetClip = (SoundClip)Enum.Parse(typeof(SoundClip), $"Melee{randIndex}");
        SoundManager.Instance.PlaySFX(targetClip);

        Bullet tb = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.ToolBullet, toolFirePos.position, toolFirePos.rotation).GetComponent<Bullet>();
        tb.Power = data.Power;
    }

    void GunAttack()
    {
        //SoundManager.instance.PlaySFX("Range");
        SoundManager.Instance.PlaySFX(SoundClip.Range);

        for (int i = 0; i < gunFirePos.Length; i++)
        {
            if (!gunFirePos[i].gameObject.activeSelf)
                continue;

            Bullet gb = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.GunBullet, gunFirePos[i].position, gunFirePos[i].rotation, true).GetComponent<Bullet>();
            gb.Power = data.Power;
            gb.fireDir = transform.localScale.x == -1 ? Vector2.left : Vector2.right;
        }
    }

    public void RefreshGunEntrance()
    {
        int count = 0;
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < gunFirePos.Length; i++)
        {
            if (count < data.GunEntranceCount)
            {
                gunFirePos[i].gameObject.SetActive(true);
                list.Add(gunFirePos[i]);
                count++;
            }                
            else
            {
                gunFirePos[i].gameObject.SetActive(false);
            }
        }

        //총구 개수에 따른 회전값 구성 
        int[][] rotations = {
            new int[] { 0 },
            new int[] { -20, 20 },
            new int[] { -30, 0, 30 },
            new int[] { -40, -20, 20, 40 },
            new int[] { -50, -30, 0, 30, 50 }
        };

        int[] targetRotations = rotations[count - 1];
        for (int i = 0; i < list.Count; i++)
            list[i].eulerAngles = new Vector3(0, 0, targetRotations[i]);
    }

    // 실드 ----------------------------------------------------------------------------------------
    public void AddShield()
    {
        if (shieldList.Count >= 6)
            return;

        //Shield newShield = Instantiate(shield, shieldParent);
        Shield newShield = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.Shield, shieldParent.position, shieldParent.rotation).GetComponent<Shield>();
        shieldList.Add(newShield);

        SetShieldRot();
    }

    public void RemoveShield(Shield s)
    {
        for (int i = shieldList.Count - 1; i >= 0; i--)
        {
            if (shieldList[i].gameObject == s.gameObject)
                shieldList.RemoveAt(i);
        }

        //Destroy(s.gameObject); 
        ObjectPoolManager.Instance.Hide(PoolKey.Shield, s.gameObject);
        SetShieldRot();
    }

    void SetShieldRot()
    {
        if (shieldList.Count <= 0)
            return;

        float offset = 360 / shieldList.Count;
        for (int i = 0; i < shieldList.Count; i++)
        {
            float z = offset * i;
            shieldList[i].transform.localRotation = Quaternion.Euler(0, 0, z);
        }
    }

    // 이동 ----------------------------------------------------------------------------------------
    void Move()
    {
        Vector2 dir = UI.Instance.joystick.Direction;
                
        //float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * GameManager.Instance.P.data.Speed;
        //float y = Input.GetAxisRaw("Vertical") * Time.deltaTime * GameManager.Instance.P.data.Speed;

        float x = dir.x * Time.deltaTime * GameManager.Instance.P.data.Speed;
        float y = dir.y * Time.deltaTime * GameManager.Instance.P.data.Speed;

        float cX = Mathf.Clamp(transform.position.x + x, -19.5f, 19.5f);
        float cY = Mathf.Clamp(transform.position.y + y, -19.5f, 19.5f);

        //transform.Translate(new Vector3(x, y, 0) * Time.deltaTime * GameManager.Instance.playerSpeed);
        transform.position = new Vector3(cX, cY, 0);
        transform.localScale = x < 0 ? new Vector3(-1, 1, 1) : x > 0 ? Vector3.one : transform.localScale;

        if (gm == null)
        {
            gm = GameManager.Instance;
        }
        else
        {
            //움직이고 있을 때, 
            if(x != 0 || y != 0)
            {
                if (gm.cState == CharacterState.Stand)
                {
                    gm.cState = CharacterState.Run;
                    List<Sprite> sprites = ResManager.Instance.charSprite[charNnum].charSprite.runSprite.ToList();
                    sa.SetSprite(sprites, 0.5f / GameManager.Instance.P.data.Speed);
                }
            } 

            //멈춰 있을 때, 
            else
            {
                if (gm.cState == CharacterState.Run)
                {
                    sa.SetSprite(ResManager.Instance.charSprite[charNnum].charSprite.standSprite.ToList(), 0.2f);
                    gm.cState = CharacterState.Stand;
                }
            }
        }
    }

    /// <summary>
    /// 타겟을 찾아 방향 전환
    /// </summary>
    void SetRotFirePos(Transform trans)
    {
        Vector2 vec = transform.position - trans.position;
        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        toolFirePos.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    // ----------------------------------------------------------------------------------------------
    public void SetExp(float exp)
    {
        data.Exp += exp;
    }

    public void Damage(int damage)
    {
        data.Hp -= damage;

        if(data.Hp <= 0)
        {
            //SoundManager.instance.PlayBGM("Lose", false);
            SoundManager.Instance.PlayBGM(SoundClip.Lose, false);

            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            GetComponent<Collider2D>().enabled = false;

            toolHand.SetActive(false);
            gunHand.SetActive(false);

            //Time.timeScale = 0;
            //죽는 애니
            List<Sprite> sprite = ResManager.Instance.charSprite[charNnum].charSprite.deadSprite.ToList();            
            sa.SetSprite(sprite, 0.5f);

            GameManager.Instance.FindUI().ShowDeadUI();
        }
    }
}
