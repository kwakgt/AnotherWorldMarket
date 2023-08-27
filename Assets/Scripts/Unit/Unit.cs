using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour //IPointerClickHandler //UI�� �ƴϸ� ī�޶� Physics2DRaycater ������Ʈ �ʿ�
{
    public      Vector2 target;             //�̵���ǥ
    protected   Shelf shelf;                //ã�� �ǸŴ�
                Vector2[] path;             //ã�� ���
                int pathIndex;              //��� �ε���, ��� �׸����

    
    protected   Vector2 respawn;            //ź��,�Ҹ���ġ
    public      int buyCount = 5;           //����Ƚ��
    public      int money = 1000;           //������
                float speed = 5;            //�ӵ�

    
    protected   Type type;                  //����
                int invenSize = 12;
    public      int invenSizeAvailable { get; private set; } = ((int)consumables.PlasticBag);  //��밡�����κ��丮
    public      Item[] inventory { get; private set; }  //�κ��丮
    protected   Heap<Index> invenIndex;     //Index.value�� inventory �ε���

                TextMeshProUGUI nameText;   //�ڽ��ε��� 0
    protected   TextMeshProUGUI priceText;  //�ڽ��ε��� 1;
                Slider slider;              //�ڽ��ε��� 2;

    protected virtual void Awake()
    {
        inventory = new Item[invenSize];
        nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        slider = transform.GetChild(0).GetChild(2).GetComponent<Slider>();
        FullChagingHeapIndex();
    }
    protected virtual void Start()
    {
        respawn = UnitManager.instance.GetRespawn();    //test
        StartCoroutine(RefreshPath());
    }

    void Update()
    {

    }

    IEnumerator RefreshPath()
    {
        yield return null;      //GoMarket()�Լ��� Start()���� ó�� �����ϴµ� �ٸ� Ŭ���� �ʱ�ȭ���� ���� �����ϸ� NULL�������� �߻��ϹǷ� �� ����Ŭ ������ ����
        GoMarket();
        Vector2 targetPositionOld = target + Vector2.up; // ó���� target.position�� != ����

        while (true)
        {
            if (targetPositionOld != target)    //Ÿ���� �ٲ�� �ٷ� ����
            {
                targetPositionOld = target;     //OLD Ÿ�������ǰ� Ÿ���������� ���� ����� �ѹ��� ����ǰ� ����, Ÿ���� ����Ǹ� �ٽ� ����

                path = Pathfinding.RequestPath(transform.position, target);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            pathIndex = 0;    //path �ε���
            Vector2 currentWaypoint = path[0];

            while (true)
            {
                if ((Vector2)transform.position == currentWaypoint) //��ǥ��ġ�� �����Ϸ��ϸ� ���� ��������Ʈ�� �̵�
                {
                    pathIndex++;
                    if (pathIndex >= path.Length)     //path �ε����� path ���̸�ŭ ���� ����
                    {
                        break;
                    }
                    currentWaypoint = path[pathIndex];
                }

                //Vector2.MoveTowards(current,target,MaxDistanceDelta) : current���� target���� MaxDistanceDelta��ŭ ������ ��ǥ
                transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }

            //���� target�� ������ ����, �Ʒ��� �� �� �ൿ ����
            if (type == Type.Customer)
                yield return StartCoroutine("CustomerRoutine");
            else
                yield return StartCoroutine("StaffRoutine");
        }
    }

    

    IEnumerator Waiting(float waitTime)
    {
        slider.gameObject.SetActive(true);
        slider.maxValue = waitTime;
        while (slider.value < slider.maxValue)
        {
            slider.value += Time.deltaTime;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        slider.value = 0;
    }

    protected void GoMarket(int index = -1)
    {
        do
        {
            shelf = ShelfManager.instance.RequestRandomShelf();
            if (shelf == null)
                Debug.Log("���Ͽ� �ƹ��͵� ����");
            else
                target = shelf.GetShelfFrontPosition(index);
        } while ((Vector2)transform.position == target);    //����ġ�� Ÿ����ġ�� ������ Ÿ�� �缼��
    }

    public void GoHome()
    {
        target = respawn;
    }

    void FullChagingHeapIndex() //����� �κ��丮 �ε��� �ֱ�
    {
        invenIndex = new Heap<Index>(invenSize);
        for (int i = 0; i < invenSize; i++)
        {
            Index value = new Index(i);
            invenIndex.Add(value);
        }
    }

    

    protected int FindKeyByValue(Item _item)
    {
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null && inventory[i].Equals(_item))
                return i;
        }
        return -1;
    }

    protected bool IsInventoryFull()
    {
        int count = 0;
        for(int i = 0; i < invenSizeAvailable; i++)
        {
            if (inventory[i] != null)
                ++count;
        }

        if (count >= invenSizeAvailable)
            return true;
        else
            return false;
    }

    protected bool IsInventoryEmpty()
    {
        int count = 0;
        for (int i = 0; i < invenSizeAvailable; i++) 
        {
            if (inventory[i] == null)
                ++count;
        }

        if (count >= invenSizeAvailable)
            return true;
        else
            return false;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = pathIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

                if (i == pathIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerClick = this.gameObject)
        {
            GameManager.instance.selectedUnit = this;
        }

        Debug.Log(GameManager.instance.selectedUnit);
    }*/

    public enum consumables { TwoHands = 2, PlasticBag = 4, Basket = 8, Cart = 12 }
    public enum Type { Customer, Staff}
}