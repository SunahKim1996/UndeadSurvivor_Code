using System.Collections.Generic;
using UnityEngine;

public class BoxItem : MonoBehaviour
{
    SpriteRenderer sr;

    private int itemCount = 15;
    private int itemPosRange = 2;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = ResManager.Instance.boxSprite.closeSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.GetComponent<Player>();

        if (p != null)
        {
            sr.sprite = ResManager.Instance.boxSprite.openSprite;
            UnboxItem();

            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 2f);
        }
    }

    void UnboxItem()
    {
        List<Exp> exps = ExpBoxManager.Instance.exps;

        for (int i = 0; i < itemCount; i++)
        {
            int randInt = Random.Range(0, exps.Count - 1);
            Exp exp = exps[randInt];

            //Instantiate(exp, GetRandomPos(), Quaternion.identity);
            PoolKey poolkey = exps[randInt].poolKey;
            ObjectPoolManager.Instance.ShowObjectPool(poolkey, GetRandomPos(), Quaternion.identity);
        }
    }

    Vector2 GetRandomPos()
    {
        Vector2 boxPos = transform.position;
        float x = Random.Range(boxPos.x - itemPosRange, boxPos.x + itemPosRange);
        float y = Random.Range(boxPos.y - itemPosRange, boxPos.y + itemPosRange);

        return new Vector2(x, y);
    }
}
