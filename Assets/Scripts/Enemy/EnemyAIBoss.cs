using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBoss : MonoBehaviour
{
    public enum State
    {
        ATTACK, DIE, SKILL
    }
    //상태를 저장할 변수
    public State state = State.ATTACK;
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

    public GameObject ENE1;
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
    readonly int hashDie = Animator.StringToHash("IsDie");
    readonly int hashDieidx = Animator.StringToHash("DieIdx");
    //readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashSKILL = Animator.StringToHash("SKILL");
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
        InvokeRepeating("SKILL", 10, 10);

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
         
            yield return ws;
        }
    }
    public IEnumerator Action()
    {
        while (!isDie) //적 캐릭터가 사망하기 전까지 도는 무한루프
        {
            yield return ws;
            //상태에 따라 분기처리
            switch (state)
            {
                
                case State.ATTACK:
                    if (enemyFire.isFire == false)
                    {
                        //총알 발사 
                        enemyFire.isFire = true;
                    }
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    //Invoke("SKILL", 1);
                    break;
                case State.SKILL:
                    if (enemyFire.isFire == true)
                    {
                        enemyFire.isFire = false;
                    }
                    animator.SetBool(hashSKILL, true);
                    ////순찰 및 추적을 정지
                    //moveAgent.Stop();
                    Instantiate(ENE1, new Vector3(2.0f, 0, 0), Quaternion.identity);
                    animator.SetBool(hashMove, false);
                    state = State.ATTACK;
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
                    break;
            }
        }
    }

    void Update()
    {
        //Speed  파라미터에 이동속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines(); //모든 코루틴 함수를 종료시킴
        animator.SetTrigger(hashPlayerDie);
    }
    
    void SKILL()
    {
        state = State.SKILL;
        
    }

   
}

