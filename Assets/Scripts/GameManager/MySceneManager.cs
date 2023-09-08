using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    static MySceneManager instance;

    public static MySceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MySceneManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        StartCoroutine("LoadScene", "AnotherWorldMarket");
    }
    
    IEnumerator LoadScene(string sceneName)
    {
        //LoadSceneMode.Single : ���� ���� ������Ʈ���� ��� Destroy�ϰ� ���Ӱ� ���� �ε�, Default
        //LoadSceneMode.Additive : ���� ���� ���ο���� �߰������� ����� �ε�
        //LoadScene�Լ��� ȣ���ϸ� �ش� ���� ��� ������ �޸𸮿� �������� ������ �ٸ� �۾��� ���� ���Ѵ�. �׷��� ������ ���߰� ��(�۾�ó����������) �߻�
        //�� ������ �ذ��ϱ� ���� AsyncOperation�̶�� �񵿱����� ������ ���� �ڷ�ƾ�� ����
        //AsyncOperation ���� ���
        //bool allowSceneActivation : ����� �غ�Ǵ� ��� ����� Ȱ��ȭ��ų�� ��뿩��, false�� ��Ȱ��ȭ, true�� �Ǹ� �ٷ� ��� Ȱ��ȭ��
        //bool isDone : �ش� ����(��)�� �غ�Ǿ������� ����, true�� �� �غ� �Ϸ�
        //float progress : �۾��� ���� ������ 0�� 1������ ������ Ȯ�� ����, �� ������ �̿��ؼ� "�ε� ��" ���� ����
        //int priority : ��Ƽ���� �ε��� �� �� ȣ���ϴ� ����
        
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
    
        while (!asyncOper.isDone)
        {
            yield return null;
        }
    }
}