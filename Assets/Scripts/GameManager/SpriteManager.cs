using EnumManager;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    //Sprite DB;
    public static SpriteManager instance;

    //작업 이미지
    Dictionary<WorkType, Sprite> workImageDict = new Dictionary<WorkType, Sprite>();
    //차원 이미지
    Dictionary<Dimension, Sprite> dimensionImageDict = new Dictionary<Dimension, Sprite>();

    //인벤 사용불가 이미지
    public Sprite unusableSlot { get; private set; }
    void Awake()
    {
        instance = this;

        SetWorkImageDictionary();
        SetDimensionImageDictionary();

        unusableSlot = Resources.Load<Sprite>("Sprite/Item/UnusableSlot");
    }

    public Sprite GetWorkImage(WorkType workType)
    {
        if (workImageDict.ContainsKey(workType)) { return workImageDict[workType]; }
        else return workImageDict[WorkType.Checking];
    }

    public Sprite GetDimensionImage(Dimension dimension)
    {
        if (dimensionImageDict.ContainsKey(dimension)) return dimensionImageDict[dimension];
        else return dimensionImageDict[Dimension.Astaria];
    }

    void SetWorkImageDictionary()
    {
        string spritePath = "Sprite/WorkIcon/";
        workImageDict.Add(WorkType.Checking, Resources.Load<Sprite>(spritePath + "Checking"));
        workImageDict.Add(WorkType.Purchase, Resources.Load<Sprite>(spritePath + "Purchase"));
        workImageDict.Add(WorkType.Carrying, Resources.Load<Sprite>(spritePath + "Carrying"));
        workImageDict.Add(WorkType.Felling, Resources.Load<Sprite>(spritePath + "Felling"));
        workImageDict.Add(WorkType.Mining, Resources.Load<Sprite>(spritePath + "Mining"));
        workImageDict.Add(WorkType.Collecting, Resources.Load<Sprite>(spritePath + "Collecting"));
        workImageDict.Add(WorkType.Hunting, Resources.Load<Sprite>(spritePath + "Hunting"));
        workImageDict.Add(WorkType.Fishing, Resources.Load<Sprite>(spritePath + "Fishing"));

        //TODO:: 작업 아이콘 추가
    }

    void SetDimensionImageDictionary()
    {
        string spritePath = "Sprite/Dimension/";
        dimensionImageDict.Add(Dimension.Astaria, Resources.Load<Sprite>(spritePath + "Astaria"));
        dimensionImageDict.Add(Dimension.Animaia, Resources.Load<Sprite>(spritePath + "Animaia"));
        dimensionImageDict.Add(Dimension.Manujhar, Resources.Load<Sprite>(spritePath + "Manujhar"));
        dimensionImageDict.Add(Dimension.Navarore, Resources.Load<Sprite>(spritePath + "Navarore"));
        dimensionImageDict.Add(Dimension.Hyloth, Resources.Load<Sprite>(spritePath + "Hyloth"));
        dimensionImageDict.Add(Dimension.Voltroth, Resources.Load<Sprite>(spritePath + "Voltroth"));
        dimensionImageDict.Add(Dimension.Genierth, Resources.Load<Sprite>(spritePath + "Genierth"));
        dimensionImageDict.Add(Dimension.Dreatera, Resources.Load<Sprite>(spritePath + "Dreatera"));
        dimensionImageDict.Add(Dimension.Devlearn, Resources.Load<Sprite>(spritePath + "Devlearn"));
        dimensionImageDict.Add(Dimension.Holysacria, Resources.Load<Sprite>(spritePath + "Holysacria"));

        //TODO:: 차원 아이콘 추가
    }
}
