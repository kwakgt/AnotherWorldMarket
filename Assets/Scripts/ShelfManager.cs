using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager instance;

    Dictionary<int, Shelf> shelfDictionary = new Dictionary<int, Shelf>();  //매대 고유인덱스, Shelf
    Queue<int> uniqueIndexQue = new Queue<int>();                           //삭제된 매대의 고유인덱스 저장
    int uniqueIndex;                                                        //고유인덱스
    
    void Awake()
    {
        instance = this;
    }
   
    public int RequestShelfIndex() //고유인덱스 부여
    {
        if(uniqueIndexQue.Count > 0)    //인덱스 큐에 남은 인덱스가 있으면 재활용
        {
            return uniqueIndexQue.Dequeue();
        }

        return uniqueIndex++;
    }

    public Shelf RequestRandomShelfPoint()          //랜덤 매대 타겟 부여
    {
        if (shelfDictionary.Count == 0) return null; //매대목록에 값이 없으면 중지

        int index = Random.Range(0, uniqueIndex);
        while(!shelfDictionary.ContainsKey(index))  //index가 없다면 재요청
        {
            index = Random.Range(0, uniqueIndex);
        }
        Shelf shelf;
        shelfDictionary.TryGetValue(index, out shelf);

        return shelf;
    }

    public void AddShelfDictionary(int index, Shelf shelf)
    {
        shelfDictionary.Add(index, shelf);
    }

    public void RemoveShelfDictionary(int index)
    {
        shelfDictionary.Remove(index);
        uniqueIndexQue.Enqueue(index);
    }

    public void UpdateShelfDictionary(int index, Shelf shelf)
    {
        shelfDictionary[index] = shelf;
    }


}
