using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager instance;

    Dictionary<int, Shelf> shelfDictionary = new Dictionary<int, Shelf>();  //�Ŵ� �����ε���, Shelf
    Queue<int> uniqueIndexQue = new Queue<int>();                           //������ �Ŵ��� �����ε��� ����
    int uniqueIndex;                                                        //�����ε���
    
    void Awake()
    {
        instance = this;
    }
   
    public int RequestShelfIndex() //�����ε��� �ο�
    {
        if(uniqueIndexQue.Count > 0)    //�ε��� ť�� ���� �ε����� ������ ��Ȱ��
        {
            return uniqueIndexQue.Dequeue();
        }

        return uniqueIndex++;
    }

    public Shelf RequestRandomShelfPoint()          //���� �Ŵ� Ÿ�� �ο�
    {
        if (shelfDictionary.Count == 0) return null; //�Ŵ��Ͽ� ���� ������ ����

        int index = Random.Range(0, uniqueIndex);
        while(!shelfDictionary.ContainsKey(index))  //index�� ���ٸ� ���û
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