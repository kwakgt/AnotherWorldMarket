using EnumManager;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    //Sprite DB;
    public static SpriteManager instance;

    //�۾� �̹���
    Dictionary<WorkType, Sprite> workIconDict = new Dictionary<WorkType, Sprite>();
    //���� �̹���
    Dictionary<Dimension, Sprite> dimensionIconDict = new Dictionary<Dimension, Sprite>();

    //�κ� ���Ұ� �̹���
    public Sprite unusableSlot { get; private set; }
    void Awake()
    {
        instance = this;

        SetWorkIconDictionary();
        SetDimensionIconDictionary();

        unusableSlot = Resources.Load<Sprite>("Sprite/Item/UnusableSlot");
    }

    public Sprite GetWorkImage(WorkType workType)
    {
        if (workIconDict.ContainsKey(workType)) { return workIconDict[workType]; }
        else return workIconDict[WorkType.Checking];
    }

    public Sprite GetDimensionImage(Dimension dimension)
    {
        if (dimensionIconDict.ContainsKey(dimension)) return dimensionIconDict[dimension];
        else return dimensionIconDict[Dimension.Astaria];
    }

    void SetWorkIconDictionary()
    {
        string spritePath = "Sprite/WorkIcon/";
        workIconDict.Add(WorkType.Checking, Resources.Load<Sprite>(spritePath + "Checking"));
        workIconDict.Add(WorkType.Purchase, Resources.Load<Sprite>(spritePath + "Purchase"));
        workIconDict.Add(WorkType.Carrying, Resources.Load<Sprite>(spritePath + "Carrying"));
        workIconDict.Add(WorkType.Felling, Resources.Load<Sprite>(spritePath + "Felling"));
        workIconDict.Add(WorkType.Mining, Resources.Load<Sprite>(spritePath + "Mining"));
        workIconDict.Add(WorkType.Collecting, Resources.Load<Sprite>(spritePath + "Collecting"));
        workIconDict.Add(WorkType.Hunting, Resources.Load<Sprite>(spritePath + "Hunting"));
        workIconDict.Add(WorkType.Fishing, Resources.Load<Sprite>(spritePath + "Fishing"));

        //TODO:: �۾� ������ �߰�
    }

    void SetDimensionIconDictionary()
    {
        string spritePath = "Sprite/Dimension/";
        dimensionIconDict.Add(Dimension.Astaria, Resources.Load<Sprite>(spritePath + "Astaria"));
        dimensionIconDict.Add(Dimension.Animaia, Resources.Load<Sprite>(spritePath + "Animaia"));
        dimensionIconDict.Add(Dimension.Manujhar, Resources.Load<Sprite>(spritePath + "Manujhar"));
        dimensionIconDict.Add(Dimension.Navarore, Resources.Load<Sprite>(spritePath + "Navarore"));
        dimensionIconDict.Add(Dimension.Hyloth, Resources.Load<Sprite>(spritePath + "Hyloth"));
        dimensionIconDict.Add(Dimension.Voltroth, Resources.Load<Sprite>(spritePath + "Voltroth"));
        dimensionIconDict.Add(Dimension.Genierth, Resources.Load<Sprite>(spritePath + "Genierth"));
        dimensionIconDict.Add(Dimension.Dreatera, Resources.Load<Sprite>(spritePath + "Dreatera"));
        dimensionIconDict.Add(Dimension.Devlearn, Resources.Load<Sprite>(spritePath + "Devlearn"));
        dimensionIconDict.Add(Dimension.Holysacria, Resources.Load<Sprite>(spritePath + "Holysacria"));

        //TODO:: ���� ������ �߰�
    }
}