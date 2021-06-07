using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    //순찰 지점들을 저장하기 위한 list 타입의 변수
    public List<Transform> wayPoints;
    //다음 순찰 지점의 배열의 Index
    public int nextIdx;

    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4.0f;
    //회전할 때의 속도를 조절하는 계수
    float damping = 1.0f;
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    //순찰 여부를 판단하는 변수
    bool _patrolling;
    //patrolling프로퍼티 정의(getter, setter)
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                //순찰 상태의 회전계수
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }
    //추적 대상의 위치를 지정하는 변수
    Vector3 _traceTarget;
    public Vector3 traceTarget //프로퍼티 정의 (getter, setter)
    {
        get
        {
            return _traceTarget;
        }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }
    NavMeshAgent agent;
    //적 캐릭터의 Transform 컴포넌트를 저장할 변수
    Transform enemyTr;

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성화
        agent.autoBraking = false;
        //자동으로 회전하는 기능을 비활성화
        agent.updateRotation = false;
        agent.speed = patrolSpeed;
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            //wayPointGroup 하위에 있는 모든 Transfrom 컴포넌트를 추출한 후 wayPoints에 추가
            group.GetComponentsInChildren<Transform>(wayPoints);
            //배열의 첫 번째 항목 삭제
            wayPoints.RemoveAt(0);
            //첫 번째로 이동할 위치를 불규칙하게 추출
            nextIdx = Random.Range(0, wayPoints.Count);
        }
        MoveWayPoint();
    }

    void MoveWayPoint()//다음 목적지 까지 이동명령을 내리는 함수
    {
        //최단거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        if (agent.isPathStale) return;
        //다음 목적지를 wayPoint 배열에서 추출한 위치로 다음 목적지를 지정
        agent.destination = wayPoints[nextIdx].position;
        //내비게이션 기능을 활성화해서 이동을 시작함
        agent.isStopped = false;
    }
    void TraceTarget(Vector3 pos)//플레이어 추적 함수
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }
    //순찰 및 추적을 정지시키는 함수
    public void Stop()
    {
        agent.isStopped = true;
        //바로 정지하기 위해 속도를 0으로 설정
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
    public void StopKey()
    {
        //바로 정지하기 위해 속도를 0으로 설정
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
    void Update()
    {
        //적 캐릭터가 이동 중일 떄만 회전
        if (agent.isStopped == false)
        {
            //NavMeshAgent가 가야할 방향 벡터를 쿼터니언 타입의 각도로 변환
            /*desiredVelocity 속성은 NavMeshAgent가 이동할 때 다음 목적지로 향하는 속도를 의미함
              속도는 단위 시간동안 이동한 변위 값을 나타내는 벡터량으로 쿼터니언 회전 값을 산축하기 위해
              아래 함수를 사용함 인자는 속도! 인자로 전달된 벡터가 바라보는 방향으로의 회전 각도를 쿼터니언 타입으로 반환*/
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //보간 함수를 사용해 점진적으로 회전시킴
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
        if (!_patrolling)
        {
            return;
        }
        //NavMeshAgent 이동하고 있고, 목적지에 도착했는지 여부를 계산
        //velocity는 제곱근 계산을해서 제곱근값으로 넣어줘야한다
        if (agent.velocity.sqrMagnitude >= 0.2f + 0.2f && agent.remainingDistance <= 0.5f)
        {
            //다음 목적지의 배열 첨자를 계산
            //nextIdx = ++nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count);
            //다음 목적지로 이동 명령을 수행
            MoveWayPoint();
        }
    }
}
