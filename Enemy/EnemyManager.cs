using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    List<BoxCollider2D> rangeColliderList = new List<BoxCollider2D>();

    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private List<Enemy> bossList;
    [SerializeField] private Transform enemyParent;

    private float timer;
    [SerializeField]
    private float delay;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            rangeColliderList.Add(transform.GetChild(i).GetComponent<BoxCollider2D>());
        }
    }

    void Start()
    {
        for (int i = 0;i < enemies.Count; i++)
        {
            PoolKey poolKey = (PoolKey)Enum.Parse(typeof(PoolKey), $"Enemy{i}");
            ObjectPoolManager.Instance.Init(poolKey, enemyParent, enemies[i].gameObject);
        }

        //HACK: Scriptable 값이 반영되지 않아서 Start 에서 처리.. 
        enemies[0].data.spawnRate = 100;
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        if (!TimeManager.Instance.isBossTime) 
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0;
                SpawnEnemy();
            }
        }        
    }
    
    int GetTotalRate()
    {
        int totalRate = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            totalRate += enemies[i].data.spawnRate;
        }

        return totalRate;
    }

    void SpawnEnemy()
    {
        int randomValue = UnityEngine.Random.Range(0, GetTotalRate());
        int targetIndex = -1;

        for (int i = 0; i < enemies.Count; i++)
        {
            int targetWeight = enemies[i].data.spawnRate;

            if (randomValue <= targetWeight)
            {
                targetIndex = i;
                break;
            }

            randomValue -= targetWeight;
        }

        //Enemy0 e = Instantiate(enemies[targetIndex], Return_RandomPosition(), Quaternion.identity).GetComponent<Enemy0>();
        
        PoolKey poolKey = (PoolKey)Enum.Parse(typeof(PoolKey), $"Enemy{targetIndex}");
        ObjectPoolManager.Instance.ShowObjectPool(poolKey, Return_RandomPosition(), Quaternion.identity);
    }

    /// <summary>
    /// 스테이지 거듭할수록, 높은 등급 Enemy 의 소환 확률을 높임
    /// </summary>
    public void RefreshSpawnRate(int curStage)
    {
        if (curStage - 1 >= enemies.Count)
            return;

        int defaultRate = 65;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i == curStage - 1)
            {
                enemies[i].data.spawnRate += defaultRate - (10 * i);
                break;
            }
        }
    }

    public void SpawnBoss()
    {
        int rand = UnityEngine.Random.Range(0, bossList.Count);
        Instantiate(bossList[rand], Return_RandomPosition(), Quaternion.identity);
    }

    Vector3 Return_RandomPosition()
    {
        int spawnRand = UnityEngine.Random.Range(0, rangeColliderList.Count);
        BoxCollider2D rangeCollider = rangeColliderList[spawnRand];
        GameObject rangeObject = rangeCollider.gameObject;

        Vector3 originPosition = rangeObject.transform.position;

        float rangeX = rangeCollider.bounds.size.x;
        float rangeY = rangeCollider.bounds.size.y;

        rangeX = UnityEngine.Random.Range((rangeX / 2) * -1, rangeX / 2);
        rangeY = UnityEngine.Random.Range((rangeY / 2) * -1, rangeY / 2);
        Vector3 randomPos = new Vector3(rangeX, rangeY, 0f);
        Vector3 spawnPos = originPosition + randomPos;

        return spawnPos;
    }
}
