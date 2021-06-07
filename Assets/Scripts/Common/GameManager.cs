using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //게임 종료 여부를 판단할 변수
    public bool isGameOver = false;

    //싱글턴에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;
    [Header("Object Pool")]
    //생성할 총알 프리팹
    public GameObject bulletPrefab;
    //오브젝트 풀에 생성할 개수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    //일시정지 여부를 판단하는 변수
    bool isPaused;
   
  
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) //할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미함
        {
            Destroy(this.gameObject);
        }
        //다른 씬으로 넘어가더라도 삭제하지 않고 유지함
        DontDestroyOnLoad(this.gameObject);
        CreatePooling();//오브젝트 풀링 생성함수 호출
        //LoadGameData(); //게임의 초기 데이터 로드
    }
    //void LoadGameData()
    //{
    //    //KILL_COUNT 키로 저장된 값을 로드
    //    killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
    //    killCountTxt.text = "KILL " + killCount.ToString("0000");
    //}
    //public void IncKillCount()//적 캐릭터가 죽을 떄마다 호출될 함수
    //{
    //    ++killCount;
    //    killCountTxt.text = "KILL " + killCount.ToString("0000");
    //    //죽인횟수를 저장
    //    PlayerPrefs.SetInt("KILL_COUNT", killCount);
    //}
    void Start()
    {
        //처음 인벤토리를 비활성화
        OnInventoryOpen(false);
        //하이러키 뷰의 SpawnPointGroup을 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        //points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //if (points.Length > 0)
        //{
        //    StartCoroutine(this.CreateEnemy());
        //}

    }
    //IEnumerator CreateEnemy() //적 캐릭터를 생성하는 코루틴 함수
    //{
    //    //게임 종료 시 까지 무한루프
    //    while (!isGameOver)
    //    {
    //        //현재 생성된 적 캐릭터의 개수 산출
    //        int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;

    //        //적 캐릭터의 최대 생성 개수보다 작을 때만 적 캐릭터를 생성
    //        if (enemyCount < maxEnemy)
    //        {
    //            //적 캐릭터의 생성 주기 시간만큼 대기
    //            yield return new WaitForSeconds(createTime);
    //            //불규칙적인 위치 산출
    //            int idx = Random.Range(1, points.Length);
    //            //적 캐릭터의 동적 생성
    //            Instantiate(enemy, points[idx].position, points[idx].rotation);
    //        }
    //        else
    //        {
    //            yield return null;
    //        }
    //    }
    //}

    //오브젝트 풀에 총알을 생성하는 함수
    public void CreatePooling()
    {
        //총알을 생성해 차일드화할 페어런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPools");
        //폴링 개수만큼 미리 총알을 생성
        for (int i = 0; i < maxPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 생성한 총알 추가
            bulletPool.Add(obj);
        }
    }
    //오브젝트 풀에서 사용가능한 총알을 가져오는 함수
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }
    public void OnPauseClik()
    {
        isPaused = !isPaused;
        //Time Scale이 0이면 정지 1이면 정상속도
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;
        //주인공 객체를 추출
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트를 추출함
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트를 활성화 / 비활성화
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }
    //인벤토리를 활성화 / 비활성화 하는 함수
    public void OnInventoryOpen(bool isOpened)
    {
        //inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        //inventoryCG.interactable = isOpened;
        //inventoryCG.blocksRaycasts = isOpened;
    }
}
