using EnumManager;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteManager : MonoBehaviour
{
    //Sprite DB;
    public static SpriteManager instance;

    //작업 이미지
    Dictionary<StaffWork, Sprite> workImageDict = new Dictionary<StaffWork, Sprite>();
    //차원 이미지
    Dictionary<Dimension, Sprite> dimensionImageDict = new Dictionary<Dimension, Sprite>();
    //공장 이미지
    Dictionary<StaffWork, Sprite> factoryImageDict = new Dictionary<StaffWork, Sprite>();

    //인벤 사용불가 이미지
    public Sprite unusableSlot { get; private set; }
    void Awake()
    {
        instance = this;

        SetWorkImageDictionary();
        SetDimensionImageDictionary();
        SetFactoryImageDictionary();

        unusableSlot = Resources.Load<Sprite>("Sprite/Item/UnusableSlot");
    }

    //작업이미지 =======================================================================================================================================================================
    public Sprite GetWorkImage(StaffWork workType)
    {
        if (workImageDict.ContainsKey(workType)) { return workImageDict[workType]; }
        else return workImageDict[StaffWork.Checking];
    }

    void SetWorkImageDictionary()
    {
        string spritePath = "Sprite/WorkIcon/";
        foreach (StaffWork type in Enum.GetValues(typeof(StaffWork)))
        {
            Sprite sprite = Resources.Load<Sprite>(spritePath + Enum.GetName(typeof(StaffWork), type));
            if (sprite != null)
            {
                workImageDict.Add(type, sprite);
            }
        }
    }

    //차원이미지 =======================================================================================================================================================================
    public Sprite GetDimensionImage(Dimension dimension)
    {
        if (dimensionImageDict.ContainsKey(dimension)) return dimensionImageDict[dimension];
        else return dimensionImageDict[Dimension.Astaria];
    }


    void SetDimensionImageDictionary()
    {
        string spritePath = "Sprite/Dimension/";
        foreach(Dimension dimension in Enum.GetValues(typeof(Dimension)))
        {
            Sprite sprite = Resources.Load<Sprite>(spritePath + Enum.GetName(typeof(Dimension), dimension));
            if (sprite != null)
            {
                dimensionImageDict.Add(dimension, sprite);
            }
        }
    }

    //공장이미지 =======================================================================================================================================================================
    public Sprite GetFactoryImage(StaffWork workType)
    {
        if (factoryImageDict.ContainsKey(workType)) return factoryImageDict[workType];
        else return factoryImageDict[StaffWork.Emptying];
    }

    void SetFactoryImageDictionary()
    {
        string spritePath = "Sprite/Building/Factory/";
        foreach (StaffWork workType in Enum.GetValues(typeof(StaffWork)))
        {
            Sprite sprite = Resources.Load<Sprite>(spritePath + Enum.GetName(typeof(StaffWork), workType));
            if(sprite != null && IsFactoryWork(workType))
            {
                factoryImageDict.Add(workType, sprite);
            }
        }
    }

    bool IsFactoryWork(StaffWork workType) //공장작업 : 쿠킹,컷팅,드라잉,쥬싱,멜팅,믹싱,패키징
    {
        return workType switch
        {
            StaffWork.Cooking or StaffWork.Cutting or StaffWork.Drying or StaffWork.Juicing or StaffWork.Melting or StaffWork.Mixing or StaffWork.Packaging => true,
            _ => false
        };
    }
}
