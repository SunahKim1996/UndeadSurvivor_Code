using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy0 : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        int curStage = TimeManager.Instance.CurStage;
        curStage = (curStage == 0) ? 1 : curStage - 1; 

        data.speed = isBoss? ableData.Speed + 0.5f * curStage : ableData.Speed;
        data.hp = data.maxHp = isBoss ? ableData.Hp + 50 * curStage : ableData.Hp;
        data.exp = isBoss? ableData.Exp + 50 * curStage : ableData.Exp;
        data.attackRange = ableData.AttackRange;
        data.attackSpeed = ableData.AttackSpeed;
        data.power = isBoss ? ableData.Power + 3 * curStage : ableData.Power;
        data.spawnRate = ableData.SpawnRate;

        data.enemyIndex = isBoss ? Random.Range(0, 5): ableData.EnemyIndex;

        sa = GetComponent<SpriteAnimation>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        List<Sprite> sprites = ResManager.Instance.enemySprite[data.enemyIndex].runSprite.ToList();
        sa.SetSprite(sprites, 0.25f / data.speed);
    }
}
