using System.Collections.Generic;
using UnityEngine;

public class ResManager : Singleton<ResManager>
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    public class CharSprite
    {
        public Sprite[] deadSprite;
        public Sprite[] hitSprite;
        public Sprite[] runSprite;
        public Sprite[] standSprite;
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public string name;
        public CharSprite charSprite;
        public Color color;
        public string stat;
        public int targetNum;
        public bool isDefaultCharacter;
        public string unlockCondition;
    }

    public CharacterInfo[] charSprite;
    public CharSprite[] enemySprite;

    public List<Sprite> uiProfileIcon;

    [System.Serializable]
    public class BoxSprite
    {
        public Sprite openSprite;
        public Sprite closeSprite;
    }

    public BoxSprite boxSprite;
}
