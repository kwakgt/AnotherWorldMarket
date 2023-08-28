using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Index : IHeapItem<Index>
{
    int value;
    int heapIndex;

    public Index(int _value)
    {
        value = _value;
    }

    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }
   
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Index index)
    {
        int compare = value.CompareTo(index.value); //CompareTo 앞의 값이 작으면 -1, 크면 1, 같으면 0
        
        return -compare;
    }
}
