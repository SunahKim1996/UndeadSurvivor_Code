using UnityEngine;

public class FollowShield : MonoBehaviour
{
    Player p;

    // Update is called once per frame
    void Update()
    {
        if (p == null)
        {
            p = GameManager.Instance.P;
            return;
        }            

        transform.position = p.transform.position;
    }
}
