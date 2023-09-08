using UnityEngine;
using EnumManager;
using System.Collections.Generic;

public class UnitStat : MonoBehaviour
{
    //고객수치
    int purchase;           //구매수치

    //스탭수치
    //작업수치
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

        //TODO:: 작업추가
    }

    /// <summary>
    /// 구매대기시간
    /// </summary>
    /// <returns>구매대기시간 리턴</returns>
    public float GetPurchaseTime()
    {
        return Mathf.Lerp(10, 1, purchase / 100f);
    }

    /// <summary>
    /// 1회 최대 구입량(0이 나오면 아이쇼핑만 한다.)
    /// </summary>
    /// <returns>구입량 리턴</returns>
    public int GetPurchaseAmount()
    {
        return Random.Range(0,3) + Mathf.Max(purchase / 10 - 5, 0);
    }

    /// <summary>
    /// 작업시간,대기시간
    /// </summary>
    /// <param name="command"> 현재 작업</param>
    /// <returns>해당 작업의 작업시간을 리턴</returns>
    public float GetWorkingTime(WorkType command)
    {
        //작업시간,대기시간
        int work = ReplaceFromWorkTypeToInt(command);
        if (work == 0) return 0;
        return Mathf.Lerp(30, 1, work / 100f);
    }

    /// <summary>
    /// 최대 작업량(운반량,생산량)
    /// </summary>
    /// <param name="command"> Carrying : 운반량, 나머지 작업은 생산량</param>
    /// <returns>작업량 리턴, 0이면 해당 작업 불가</returns>
    public int GetWorkingAmount(WorkType command)
    {
        //작업량
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

            //TODO:: 작업 추가

        };
    }

    ////작업시간
    //int carryingTime;
    //int fellingTime;
    //int miningTime;
    //int collectingTime;
    //int huntingTime;
    //int fishingTime;

    ////작업량
    //int amountOfCarrying;
    //int amountOfFelling;
    //int amountOfMining;
    //int amountOfCollecting;
    //int amountOfHunting;
    //int amountOfFishing;
}
