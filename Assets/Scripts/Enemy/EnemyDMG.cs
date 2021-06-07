using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDMG : MonoBehaviour
{
    const string bulletTag = "BULLET";
    //생명게이지
    public float hp = 100.0f;
    //초기 생명 수치
    float initHp = 100.0f;
    GameObject bloodEffect; //피격 시 사용할 혈흔 효과

   

    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
        //생명 게이지의 생성 및 초기화
        
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == bulletTag)
        {
            //혈흔 효과를 생성
            ShowBloodEffect(collision);
            //총알 삭제
            collision.gameObject.SetActive(false);
            //생명 게이지 차감
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
           
            if (hp <= 0.0f)
            {
                //적 캐릭터의 상태를 DIE로 변경
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
               
                GetComponent<CapsuleCollider>().enabled = false;
                ////적 캐릭터의 사망 횟수를 누적시키는 함수 호출
                //GameManager.instance.IncKillCount();
            }
        }
    }
    void ShowBloodEffect(Collision coll)
    {
        //총알이 충돌한 지점 산출
        Vector3 pos = coll.contacts[0].point;
        //총알의 충돌했을 떄의 법선 벡터
        Vector3 _normal = coll.contacts[0].normal;
        //총알의 충돌 시 방향 벡터의 회전값 계산
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        //혈흔 효과 생성 및 삭제
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
}
