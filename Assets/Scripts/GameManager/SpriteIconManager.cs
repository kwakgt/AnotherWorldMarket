using EnumManager;
using System.Collections.Generic;
using UnityEngine;

public class SpriteIconManager : MonoBehaviour
{
    public static SpriteIconManager instance;

    //�۾� ������
    Dictionary<WorkType, Sprite> workIconDict = new Dictionary<WorkType, Sprite>();

    void Awake()
    {
        instance = this;

        SetWorkIconDictionary();
    }

    public Sprite GetWorkIcon(WorkType workType)
    {
        if (workIconDict.ContainsKey(workType)) { return workIconDict[workType]; }
        else return workIconDict[WorkType.Checking];
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
}