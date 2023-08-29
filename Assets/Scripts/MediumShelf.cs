using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumShelf : Shelf
{
    protected override void Rotate(int rewind)
    {
        base.Rotate(rewind);
        transform.position = (Vector2)transform.position - Vector2.one / 2; //이미지가 유닛칸에 딱 맞게 조정
    }
}
