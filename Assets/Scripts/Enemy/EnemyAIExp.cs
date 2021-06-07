using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIExp : MonoBehaviour
{
    //적 캐릭터의 상태를 표현하기 위한 열거형 변수 정의
    public enum State
    {
        PATROL, TRACE, ATTACK, DIE
    }
    //상태를 저장할 변수
    public State state = State.PATROL;
    //주인공의 위치를 저장할 변수
    Transform playerTr;
    //적 캐릭터의 위치를 저장할 변수
    Transform enemyTr;
    //공격 사정거리
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;
    //사망 여부를 판단할 변수
    public bool isDie = false;

    //코루틴에서 사용할 지연시간 변수
    WaitForSeconds ws;
    //이동을 제어하는 MoveAgent 클래스를 저장할 변수
    MoveAgent moveAgent;
    Animator animator;
    //총알 발사를 제어하는 EnemyFire 클래스를 저장할 변수
    EnemyFire enemyFire;
    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieidx = Animator.StringToHash("DieIdx");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    readonly int hashDive = Animator.StringToHash("DIVE");

    public GameObject expEffect;
    AudioSource _audio;
    //폭발 반경
    public float expRadius = 10.0f;
    //폭발음 오디오 클립
    public AudioClip expSfx;
    void Awake()
    {
        //플레이어 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>(); // 이동을 제어하는 MoveAgent 클래스를 추출
        enemyFire = GetComponent<EnemyFire>();
        //코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);

        ////Cycle Offset 값을 불규칙하게 변경
        //animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        ////Speed 값을 불규칙하게 변경
        //animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));

        _audio = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        //checkState 코루틴 함수 실행
        StartCoroutine(CheckState());
        //Action 코루틴 함수 실행
        StartCoroutine(Action());
    }

    IEnumerator CheckState() //적 캐릭터의 상태를 검사하는 코루틴 함수
    {
        while (!isDie) //적 캐릭터가 사망하기 전까지 도는 무한루프
        {
            //상태가 사망이면 코루틴 함수를 종료시킴
            if (state == State.DIE)
            {
                yield break;
            }
            //주인공과 적 캐릭터 간의 사이를 계산
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            //else
            //{
            //    state = State.PATROL;
            //}
            yield return ws;
        }
    }
    IEnumerator Action()
    {
        while (!isDie) //적 캐릭터가 사망하기 전까지 도는 무한루프
        {
            yield return ws;
            //상태에 따라 분기처리
            switch (state)
            {
                case State.PATROL:
                    ////총알 발사 정지
                    //enemyFire.isFire = false;
                    //순찰 모드를 활성화
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    //총알 발사 정지
                    //enemyFire.isFire = false;
                    //주인공의 위치를 넘겨 추적 모드를 변경
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false); //(빨갛게 되는 애니매이션과 다이빙하는 애니매이션 추가)
                    animator.SetBool(hashDive, true); //다이빙하는애니메이션
                    //폭발함수 넣기

                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";
                    isDie = true;
                    ////총알 발사 정지
                    //enemyFire.isFire = false;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetInteger(hashDieidx, Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    //GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void Update()
    {
        //Speed  파라미터에 이동속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);

        float dist = Vector3.Distance(playerTr.position, enemyTr.position);
        if (dist <= 5)
        {
            Invoke("EXPENEMY", 1);
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
       
    //        Destroy(gameObject);
    //    //폭발효과 프리팹을 동적생성
    //    GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
    //    Destroy(effect, 2.0f);
    //    //폭발력은 생성
    //    IndirectDamage(transform.position);
        
    //    //폭발음 발생
    //    _audio.PlayOneShot(expSfx, 1.0f);
       
    //}

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines(); //모든 코루틴 함수를 종료시킴
        animator.SetTrigger(hashPlayerDie);
    }

    void IndirectDamage(Vector3 pos)
    {
        //주변의 모든 드럼통 추출
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);
        foreach (var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의  rigidBody 컴포넌트를 추출
            var _rb = coll.GetComponent<Rigidbody>();
            //드럼통의 무게를 가볍게 함
            _rb.mass = 1.0f;
            //폭발력을 전달 인자는(횡 폭발력, 폭발원점, 폭발반경, 종 폭발력)
            _rb.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }

    void EXPENEMY()
    {
        Destroy(gameObject);
        //폭발효과 프리팹을 동적생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);
        //폭발력은 생성
        IndirectDamage(transform.position);

        //폭발음 발생
        _audio.PlayOneShot(expSfx, 1.0f);
    }
}
