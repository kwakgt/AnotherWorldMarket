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
        //LoadSceneMode.Single : 현재 씬의 오브젝트들을 모두 Destroy하고 새롭게 씬을 로드, Default
        //LoadSceneMode.Additive : 현재 씬에 새로운씬을 추가적으로 덧대어 로드
        //LoadScene함수를 호출하면 해당 씬의 모든 정보를 메모리에 가져오기 전까지 다른 작업을 하지 못한다. 그래서 게임이 멈추고 렉(작업처리지연현상) 발생
        //위 문제를 해결하기 위해 AsyncOperation이라는 비동기적인 연산을 위한 코루틴을 제공
        //AsyncOperation 변수 목록
        //bool allowSceneActivation : 장면이 준비되는 즉시 장면을 활성화시킬지 허용여부, false는 비활성화, true가 되면 바로 장면 활성화됨
        //bool isDone : 해당 동작(씬)이 준비되었는지의 여부, true면 씬 준비 완료
        //float progress : 작업의 진행 정도를 0과 1사이의 값으로 확인 가능, 이 변수를 이용해서 "로딩 바" 구현 가능
        //int priority : 멀티씬을 로드할 때 씬 호출하는 순서
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
    
        while (!asyncOper.isDone)
        {
            yield return null;
        }
    }
}
