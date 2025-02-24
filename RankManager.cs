using System.Collections.Generic;
using UnityEngine;

public class RankManager : Singleton<RankManager>
{
    private int recordCount = 4;
    private List<string> recordList = new List<string>();

    public void SetRecord(int killCount, float timer)
    {        
        // ���� ��� & �� ��� Dictionary �� ���� 
        for (int rank = 0; rank < recordCount; rank++) 
        {
            if (PlayerPrefs.HasKey($"RankRecord{rank}"))
                recordList.Add(PlayerPrefs.GetString($"RankRecord{rank}"));
        }
                
        /* Json ���� ������ �ε��ϴ� �ڵ�
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

        // �������� ����
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
                // killCount�� ���� ���, timer�� ���� �� �켱
                float timer_A = float.Parse(aParts[1]);
                float timer_B = float.Parse(bParts[1]);

                if (timer_A > timer_B)
                    return -1;
                else
                    return 1;
            }
        });
                
        // ���ο� ������ PlayerPrefs �� ���� 
        for (int rank = 0; rank < recordList.Count; rank++)
        {
            if (rank >= recordCount)
                break;

            PlayerPrefs.SetString($"RankRecord{rank}", recordList[rank]);
        }        

        // Json ���� ������ �����ϴ� �ڵ�
        //JsonLoader.Instance.SaveJsonData_Rank(recordList, recordCount);

        // UI
        UI.Instance.RefreshRankUI(recordList);
    }
}
