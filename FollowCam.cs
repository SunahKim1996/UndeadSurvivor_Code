using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private Player p;

    // Start is called before the first frame update
    void Start()
    {
        p = GameManager.Instance.P;
    }

    // Update is called once per frame
    void Update()
    {
        if (p == null)
        {
            p = GameManager.Instance.P;
            return;
        }

        Vector3 pos = p.transform.position;
        float cX = Mathf.Clamp(pos.x, -5.5f, 5.5f);
        float cY = Mathf.Clamp(pos.y, -15f, 15f);

        //transform.position = new Vector3(cX, cY, -10);
        transform.position = Vector3.Lerp(transform.position, new Vector3(cX, cY, -10), Time.deltaTime * 1f);
    }
}
