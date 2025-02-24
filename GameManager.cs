using UnityEngine;

public enum CharacterState
{
    Stand,
    Run,
    Dead,
}

public class PlayerData
{
    public int Power {  get; set; }

    public float ToolAtkDelay { get; set; }
    public float BaseToolAtkDelay { get; set; }
    public int ToolThroughValue { get; set; }

    public float GunAtkDelay { get; set; }
    public float BaseGunAtkDelay { get; set; }
    public int GunThroughValue { get; set; }

    private int gunEntranceCount;
    public int GunEntranceCount 
    {
        get { return gunEntranceCount; }
        set 
        {
            gunEntranceCount = value;
            GameManager.Instance.P.RefreshGunEntrance();
        }
    }


    private float speed;
    public float Speed
    {
        get { return speed; }
        set
        {
            speed = value;
        }
    }
    public float BaseSpeed { get; set; }


    private int hp;
    public int Hp
    {
        get { return hp; }
        set
        {
            hp = value;

            UI ui = FindUI();
            if (ui != null)
            {
                ui.RefreshHP();
            }
        }
    }

    private int maxHp;
    public int MaxHp
    {
        get { return maxHp; }
        set
        {
            maxHp = value;

            UI ui = FindUI();
            if (ui != null)
            {
                ui.RefreshHP(false);
            }
        }
    }


    private float exp;
    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;

            while (exp >= MaxExp)
            {
                Level++;
                exp -= MaxExp;
                MaxExp += 20;
            }

            UI ui = FindUI();
            if (ui != null)
                ui.UIExp(exp, MaxExp);
        }
    }
    public float MaxExp { get; set; }


    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;

            if (level > 1)
            {
                //SoundManager.instance.PlaySFX("LevelUp");
                SoundManager.Instance.PlaySFX(SoundClip.LevelUp);

                UI ui = FindUI();
                if (ui != null)
                {
                    GameManager.Instance.gameState = GameState.Stop;
                    
                    ui.UILevel(level);
                    ui.ShowLevelUp();
                }
            }
        }
    }


    private string name;
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
        }
    }


    private int killCount = 0;
    public int KillCount 
    {
        get { return killCount; }
        set 
        {
            killCount = value;

            //°¨ÀÚ ³óºÎ ÇØ±Ý 
            if (killCount >= 30)
                PlayerPrefs.SetString($"UnlockCharacter2", "True");

            UI ui = FindUI();
            if (ui != null)
            {
                ui.UIKillCount(killCount);
            }
        } 
    }

    private int killCount_Boss = 0;
    public int KillCount_Boss
    {
        get { return killCount_Boss; }
        set
        {
            killCount_Boss = value;

            //Äá ³óºÎ ÇØ±Ý 
            if (killCount_Boss >= 5)
                PlayerPrefs.SetString($"UnlockCharacter3", "True");
        }
    }

    public int ShieldSpeed { get; set; }
    public int ShieldPower { get; set; }    

    UI FindUI()
    {
        return GameObject.FindObjectOfType<UI>();
    }
}
public class EnemyData
{
    public int enemyIndex;
    public float speed;
    public int hp;
    public int maxHp;
    public float exp;
    public float attackRange;
    public float attackSpeed;
    public int power;
    public int spawnRate;
}
public enum GameState
{
    Play, Stop
}

public class GameManager : Singleton<GameManager>
{
    public int selectNum = 0;

    public CharacterState cState = CharacterState.Stand;
    public GameState gameState = GameState.Play;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private Player p;
    public Player P
    {
        get
        {
            if(p == null)
            {
                p = FindObjectOfType<Player>();
            }
            return p;
        }
    }

    public UI FindUI()
    {
        return GameObject.FindObjectOfType<UI>();
    }
}
