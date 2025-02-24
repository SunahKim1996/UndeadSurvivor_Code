using System.Collections.Generic;
using UnityEngine;

public class ExpBoxManager : Singleton<ExpBoxManager>
{
    public Transform expParent;
    public List<Exp> exps;

    [SerializeField] private BoxItem boxItem;

    private void Start()
    {
        ObjectPoolManager.Instance.Init(PoolKey.Exp_Bronze, expParent, exps[0].gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.Exp_Silver, expParent, exps[1].gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.Exp_Gold, expParent, exps[2].gameObject);
    }

    public void DropExp(int index, float exp, Vector3 pos)
    {
        //Exp e = Instantiate(exps[index], pos, Quaternion.identity);

        PoolKey poolkey = exps[index].poolKey;
        Exp e = ObjectPoolManager.Instance.ShowObjectPool(poolkey, pos, Quaternion.identity).GetComponent<Exp>();
        e.SetExp(exp);
    }

    public void DropBox(Vector3 pos)
    {
        BoxItem box = Instantiate(boxItem, pos, Quaternion.identity);
    }
}
