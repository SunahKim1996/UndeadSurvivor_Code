using UnityEngine;

public class ToolBullet : Bullet
{
    void Start()
    {
        fireDir = Vector2.up;
        fireSpeed = 6f;
        gunType = GunType.Tool;
    }
}
