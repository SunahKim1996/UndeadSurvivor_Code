using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenSlot : MonoBehaviour
{
    public int itemIndex;

    public Image icon;
    public TMP_Text lvText;

    void Start()
    {
        itemIndex = -1;
    }
}
