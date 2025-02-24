using UnityEngine;

public abstract class Exp : MonoBehaviour
{
    protected float exp;    
    public PoolKey poolKey;

    private Player p;

    private bool isFind = false;

    public void SetExp(float exp)
    {
        this.exp = exp;
    }

    //이렇게 한 줄로 쓸 수도 있음
    //public void SetExp(float exp) => this.exp = exp;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        if (p == null)
        {
            p = GameManager.Instance.P;
            return;
        }

        float dis = Vector2.Distance(transform.position, p.transform.position);
        if (dis < 3)
        {
            isFind = true;
        }

        if (isFind)
        {
            transform.position = Vector3.Lerp(transform.position, p.transform.position, Time.deltaTime * 3f);
        }

        if(isFind && dis < 1f)
        {
            p.SetExp(exp);
            ObjectPoolManager.Instance.Hide(poolKey, gameObject);
            //Destroy(gameObject);
        }
    }
}
