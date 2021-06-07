using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource audio;
    Animator animator;
    Transform playerTr;
    Transform enemyTr;
    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");
    float nextFire = 0.0f; //다음 발사할 시간 계산용 변수
    readonly float fireRate = 0.1f; //총알 발사 간격
    readonly float damping = 10.0f; //주인공을 향해 회전할 속도 계수

    readonly float reloadTime = 2.0f;    //재장전시간
    readonly int maxBullet = 10;         //탄창의 최대 총알 수
    int currBullet = 10;                 //초기 총알 수
    bool isReload = false;               // 재장전 여부
    WaitForSeconds wsReload;             //재장전 시간 동안 기다릴 변수 선언

    //총알 발사 여부를 판단할 변수
    public bool isFire = false;
    //총알 발사 사운드를 저장할 변수
    public AudioClip fireSfx;
    public AudioClip reloadSfx;

    public GameObject Bullet; //적 캐릭터의 총알 프리팹
    public Transform firePos; //총알의 발사 위치 정보
    //MuzzleFlash의 MeshRenderer 컴포넌트를 저장할 변수
    //public MeshRenderer muzzleFlash;
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        wsReload = new WaitForSeconds(reloadTime);
        //muzzleFlash.enabled = false;
    }

    void Update()
    {
        if (!isReload && isFire)
        {
            //현재 시간이 다음 발사 시간보다 큰지를 확인
            if (Time.time >= nextFire)
            {
                Fire();
                //다음 발사 시간 계산
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }
            //주인공이 있는 위치까지의 회전 각도 계산
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }
    void Fire()
    {
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1.0f);
        //총구 화염 효과 코루틴 호출
        StartCoroutine(ShowMuzzleFlash());
        //총알을 생성
        GameObject _bullet = Instantiate(Bullet, firePos.position, firePos.rotation);
        //일정 시간이 지난 후 삭제
        Destroy(_bullet, 3.0f);
        //남은 총알로 재장전 여부를 계산
        isReload = (--currBullet % maxBullet == 0);
        if (isReload)
        {
            StartCoroutine(Reloading());
        }
    }
    IEnumerator Reloading()
    {
        //muzzleFlash.enabled = false;
        //재장전 애니메이션 실행
        animator.SetTrigger(hashReload);
        //재장전 사운드 발생
        audio.PlayOneShot(reloadSfx, 1.0f);

        //재장전 시간만큼 대기하는 동안 제어권 양보
        yield return wsReload;

        //총알의 개수를 초기화
        currBullet = maxBullet;
        isReload = false;

    }
    IEnumerator ShowMuzzleFlash()
    {
        //MUzzle Flash 활성화
        //muzzleFlash.enabled = true;
        //불규칙한 회전 각도를 계산
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        //MuzzleFlasg를 z축 방향으로 회전
        //muzzleFlash.transform.localRotation = rot;
        //MuzzleFlasg의 스케일을 불규칙하게 조정
        //muzzleFlash.transform.localScale = Vector3.one * Random.Range(0.2f, 0.4f);
        //텍스처의 offset 속성에 적용할 불규칙한 값을 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 3)) * 0.5f;
        //MuzzleFlash의 머터리얼의 Offset 값을 적용 ///"_MainTex" -> Diffuse, "_BumpMap" -> Normal map, "_Cube" -> Cubemap
        //muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        //MuzzleFlasg가 보일 동안 잠시 대기
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        //MuzzleFlash를 다시 비활성화
        //muzzleFlash.enabled = false;

    }
}
