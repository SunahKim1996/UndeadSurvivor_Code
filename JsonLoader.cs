using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class RankDataInfo
{
    public int rank;
    public int killCount;
    public float timeRecord;
}

[System.Serializable]
public class RankData
{
    public List<RankDataInfo> ranking = new List<RankDataInfo>();
}

public class JsonLoader : Singleton<JsonLoader>
{    
    string rankDataPath = Path.Combine(Application.streamingAssetsPath, "RankData.json");

    // Rank Data --------------------------------------------------------------
    public void SaveJsonData_Rank(List<string> recordList, int recordCount)
    {
        RankData rankData = new RankData();

        for (int rank = 0; rank < recordList.Count; rank++)
        {
            if (rank >= recordCount)
                break;

            RankDataInfo saveData = new RankDataInfo();
            string[] recordArray = recordList[rank].Split('/');

            saveData.rank = rank + 1;
            saveData.killCount = int.Parse(recordArray[0]);
            saveData.timeRecord = float.Parse(recordArray[1]);

            rankData.ranking.Add(saveData);
        }

        string json = JsonUtility.ToJson(rankData, true);
        File.WriteAllText(rankDataPath, json);
    }

    public RankData LoadRankData_Rank()
    {
        if (File.Exists(rankDataPath))
        {
            string json = File.ReadAllText(rankDataPath);
            return JsonUtility.FromJson<RankData>(json);
        }
        else
        {
            return new RankData();
        }
    }
}
