using UnityEngine;

public class MediumShelf : Shelf
{
    protected override void Rotate(int rewind)
    {
        base.Rotate(rewind);
        transform.position = (Vector2)transform.position - Vector2.one / 2; //�̹����� ����ĭ�� �� �°� ����
    }
}
