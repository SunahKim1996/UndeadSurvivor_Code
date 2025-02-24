using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum PoolKey
{
    ToolBullet,
    GunBullet,
    Shield,

    Enemy0,
    Enemy1,
    Enemy2,
    Enemy3,
    Enemy4,

    Exp_Gold,
    Exp_Silver,
    Exp_Bronze,
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public struct PoolInfo
    {
        public Transform parent;
        public GameObject originObj;
        public Queue<GameObject> pool;
    }

    public Dictionary<PoolKey, PoolInfo> poolList = new Dictionary<PoolKey, PoolInfo>();

    public void Init(PoolKey poolKey, Transform parent, GameObject originObj)
    {
        PoolInfo poolInfo = new PoolInfo();
        poolInfo.parent = parent;
        poolInfo.originObj = originObj;
        poolInfo.pool = new Queue<GameObject>();

        poolInfo.originObj.SetActive(false);
        poolList.Add(poolKey, poolInfo);
    }

    public GameObject ShowObjectPool(PoolKey poolKey, Vector2 targetPos, quaternion targetRot, bool isAutoHide = false)
    {
        GameObject obj;
        PoolInfo poolInfo = poolList[poolKey];

        if (poolInfo.pool.Count == 0)
        {
            obj = Instantiate(poolInfo.originObj, targetPos, targetRot, poolInfo.parent);
        }
        else
        {
            obj = poolInfo.pool.Dequeue();
            obj.transform.position = targetPos;
            obj.transform.rotation = targetRot;
        }

        obj.SetActive(true);

        if (isAutoHide)
        {
            GunBullet gb = obj.GetComponent<GunBullet>();

            if (gb.autoHideCor != null)
            {
                StopCoroutine(gb.autoHideCor);
                gb.autoHideCor = null;
            }

            gb.autoHideCor = StartCoroutine(AutoHide(poolKey, obj));
        }

        return obj;
    }

    public void Hide(PoolKey poolKey, GameObject obj)
    {
        PoolInfo poolInfo = poolList[poolKey];

        obj.SetActive(false);
        poolInfo.pool.Enqueue(obj);
    }

    IEnumerator AutoHide(PoolKey poolKey, GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        Hide(poolKey, obj);
    }
}
