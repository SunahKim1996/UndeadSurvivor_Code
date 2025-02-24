using System.Collections.Generic;
using UnityEngine;

public class RankManager : Singleton<RankManager>
{
    private int recordCount = 4;
    private List<string> recordList = new List<string>();

    public void SetRecord(int killCount, float timer)
    {        
        // 기존 기록 & 새 기록 Dictionary 에 저장 
        for (int rank = 0; rank < recordCount; rank++) 
        {
            if (PlayerPrefs.HasKey($"RankRecord{rank}"))
                recordList.Add(PlayerPrefs.GetString($"RankRecord{rank}"));
        }
                
        /* Json 으로 데이터 로드하는 코드
        RankData data = JsonLoader.Instance.LoadRankData_Rank();

        for (int rank = 0; rank < data.ranking.Count; rank++)
        {
            RankDataInfo targetInfo = data.ranking[rank];
            string record = $"{targetInfo.killCount}/{targetInfo.timeRecord}";
            recordList.Add(record);
        }
        */

        string newRecord = $"{killCount}/{timer}";
        recordList.Add(newRecord);

        // 내림차순 정렬
        recordList.Sort((a, b) =>
        {
            string[] aParts = a.Split('/');
            string[] bParts = b.Split('/');

            int killCountRecord_A = int.Parse(aParts[0]);
            int killCountRecord_B = int.Parse(bParts[0]);

            if (killCountRecord_A > killCountRecord_B)
                return -1;
            else if (killCountRecord_A < killCountRecord_B)
                return 1;
            else
            {
                // killCount가 같을 경우, timer가 작은 게 우선
                float timer_A = float.Parse(aParts[1]);
                float timer_B = float.Parse(bParts[1]);

                if (timer_A > timer_B)
                    return -1;
                else
                    return 1;
            }
        });
                
        // 새로운 순서를 PlayerPrefs 에 저장 
        for (int rank = 0; rank < recordList.Count; rank++)
        {
            if (rank >= recordCount)
                break;

            PlayerPrefs.SetString($"RankRecord{rank}", recordList[rank]);
        }        

        // Json 으로 데이터 저장하는 코드
        //JsonLoader.Instance.SaveJsonData_Rank(recordList, recordCount);

        // UI
        UI.Instance.RefreshRankUI(recordList);
    }
}
