using UnityEngine;
using EnumManager;
using System.Collections.Generic;

public class UnitStat : MonoBehaviour
{
    //������ġ
    int purchase;           //���ż�ġ

    //���Ǽ�ġ
    //�۾���ġ
    int carrying;
    int felling;
    int mining;
    int collecting;
    int hunting;
    int fishing;


    public void SetStat(Tribe tribe)
    {
        Dictionary<Tribe, Dictionary<StatBind, int>> stat = UnitManager.instance.tribe;
        purchase = Random.Range(stat[tribe][StatBind.PurchaseMin], stat[tribe][StatBind.PurchaseMax] + 1);
        carrying = Random.Range(stat[tribe][StatBind.CarryingMin], stat[tribe][StatBind.CarryingMax] + 1);
        felling = Random.Range(stat[tribe][StatBind.FellingMin], stat[tribe][StatBind.FellingMax] + 1);
        mining = Random.Range(stat[tribe][StatBind.MiningMin], stat[tribe][StatBind.MiningMax] + 1);
        collecting = Random.Range(stat[tribe][StatBind.CollectingMin], stat[tribe][StatBind.CollectingMax] + 1);
        hunting = Random.Range(stat[tribe][StatBind.HuntingMin], stat[tribe][StatBind.HuntingMax] + 1);
        fishing = Random.Range(stat[tribe][StatBind.FishingMin], stat[tribe][StatBind.FishingMax] + 1);

        //TODO:: �۾��߰�
    }

    /// <summary>
    /// ���Ŵ��ð�
    /// </summary>
    /// <returns>���Ŵ��ð� ����</returns>
    public float GetPurchaseTime()
    {
        return Mathf.Lerp(10, 1, purchase / 100f);
    }

    /// <summary>
    /// 1ȸ �ִ� ���Է�(0�� ������ ���̼��θ� �Ѵ�.)
    /// </summary>
    /// <returns>���Է� ����</returns>
    public int GetPurchaseAmount()
    {
        return Random.Range(0,3) + Mathf.Max(purchase / 10 - 5, 0);
    }

    /// <summary>
    /// �۾��ð�,���ð�
    /// </summary>
    /// <param name="command"> ���� �۾�</param>
    /// <returns>�ش� �۾��� �۾��ð��� ����</returns>
    public float GetWorkingTime(WorkType command)
    {
        //�۾��ð�,���ð�
        int work = ReplaceFromWorkTypeToInt(command);
        if (work == 0) return 0;
        return Mathf.Lerp(30, 1, work / 100f);
    }

    /// <summary>
    /// �ִ� �۾���(��ݷ�,���귮)
    /// </summary>
    /// <param name="command"> Carrying : ��ݷ�, ������ �۾��� ���귮</param>
    /// <returns>�۾��� ����, 0�̸� �ش� �۾� �Ұ�</returns>
    public int GetWorkingAmount(WorkType command)
    {
        //�۾���
        int work = ReplaceFromWorkTypeToInt(command);
        if (work == 0) return 0;
        return Mathf.RoundToInt(Mathf.Lerp(1, 50, work / 100f));
    }

    public int ReplaceFromWorkTypeToInt(WorkType command)
    {
        return command switch
        {
            WorkType.Purchase => purchase,
            WorkType.Carrying => carrying,
            WorkType.Felling => felling,
            WorkType.Mining => mining,
            WorkType.Collecting => collecting,
            WorkType.Hunting => hunting,
            WorkType.Fishing => fishing,
            WorkType => -1

            //TODO:: �۾� �߰�

        };
    }

    ////�۾��ð�
    //int carryingTime;
    //int fellingTime;
    //int miningTime;
    //int collectingTime;
    //int huntingTime;
    //int fishingTime;

    ////�۾���
    //int amountOfCarrying;
    //int amountOfFelling;
    //int amountOfMining;
    //int amountOfCollecting;
    //int amountOfHunting;
    //int amountOfFishing;
}