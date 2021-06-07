using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIKey : MonoBehaviour
{
    //2,3스테이지의 클리어조건 및 진행을 위한 아이템 KEY변수
    public int Key = 0;
    //적 캐릭터의 상태를 표현하기 위한 열거형 변수 정의
    public enum State
    {
        STAY, ATTACK, DIE
    }
    //상태를 저장할 변수
    public State state = State.STAY;
    //주인공의 위치를 저장할 변수
    Transform playerTr;
    //적 캐릭터의 위치를 저장할 변수
    Transform enemyTr;
    //공격 사정거리
    public float attackDist = 10.0f;
    ////추적 사정거리
    //public float traceDist = 10.0f;
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
            //else if (dist <= traceDist)
            //{
            //    state = State.TRACE;
            //}
            else
            {
                state = State.STAY;
            }
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
                case State.STAY:
                    moveAgent.Stop();
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    ////순찰 모드를 활성화
                    //moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                //case State.TRACE:
                //    //총알 발사 정지
                //    enemyFire.isFire = false;
                //    //주인공의 위치를 넘겨 추적 모드를 변경
                //    moveAgent.traceTarget = playerTr.position;
                //    animator.SetBool(hashMove, true);
                //    break;
                case State.ATTACK:
                    if (enemyFire.isFire == false)
                    {
                        //총알 발사 
                        enemyFire.isFire = true;
                    }
                    moveAgent.StopKey();
                    //순찰 및 추적을 정지
                    moveAgent.traceTarget = playerTr.position;
                   
                    animator.SetBool(hashMove, false);
                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";
                    isDie = true;
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetInteger(hashDieidx, Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    //GetComponent<CapsuleCollider>().enabled = false;
                    ++Key;
                    break;
            }
        }
    }

    //void Update()
    //{
    //    //Speed  파라미터에 이동속도를 전달
    //    animator.SetFloat(hashSpeed, moveAgent.speed);
    //}

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines(); //모든 코루틴 함수를 종료시킴
        animator.SetTrigger(hashPlayerDie);
    }
}
