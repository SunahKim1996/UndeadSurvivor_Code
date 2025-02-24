using UnityEngine;

public enum GunType
{
    Tool,
    Gun
}

public class Bullet : MonoBehaviour
{
    [HideInInspector] public int ThroughCount { get; set; } = 0;
    [HideInInspector] public float Power { get; set; }

    [HideInInspector] public GunType gunType;

    public Vector2 fireDir;
    protected float fireSpeed;
    public Coroutine autoHideCor;
    public bool isAutoHiding;

    void Update()
    {
        Fire();
    }

    public void Fire()
    {
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        transform.Translate(fireDir * Time.deltaTime * fireSpeed);
    }
}
