using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] private float stageTime;

    private int curStage;
    public int CurStage
    {
        get { return curStage; }
        set
        {
            curStage = value;
            UI.Instance.StageUI(curStage);
        }
    }

    private float curStageTime;
    
    private float curTime;
    public float CurTime
    {
        get { return curTime; }
        set 
        { 
            curTime = value;
            UI.Instance.TimeUI(curTime);
        }
    }

    [HideInInspector] public bool isBossTime = false;

    void Start()
    {
        CurStage = 1;
    }

    private void Update()
    {      
        if (GameManager.Instance.gameState == GameState.Stop)
            return;

        if (!isBossTime)
        {
            CurTime += Time.deltaTime;
            curStageTime += Time.deltaTime;
        }

        if ((int)curStageTime != 0 && (int)curStageTime % stageTime == 0)
            StartBossTime();
    }

    void StartBossTime()
    {
        isBossTime = true;
        curStageTime = 0;

        UI.Instance.ShowBossUI();
        UI.Instance.TimeUI(-1, true);

        EnemyManager.Instance.SpawnBoss();
    }

    public void EndBossTime()
    {
        UI.Instance.ToggleBossUI(false);

        isBossTime = false;
        CurStage++;

        EnemyManager.Instance.RefreshSpawnRate(CurStage);
    }
}
