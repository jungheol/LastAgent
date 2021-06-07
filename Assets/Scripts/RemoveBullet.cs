using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 스파크 효과
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BULLET" || collision.gameObject.tag == "ENEMY")
        {
            ShowEffect(collision);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision coll)
    {
        // 충돌 지점의 정보를 추출, 첫 번째로 충돌한 collider의 정보를 가져온다.
        ContactPoint contact = coll.contacts[0];
        /*
         collider -> 충돌한 게임오브젝트의 collider 컴포넌트
         contacts -> 물체 간의 충돌 지점으로 물리엔진에 의해 생성
         gameObject -> 충돌한 게임오브젝트
         impulse -> 충돌 시 발생한 충격량
         relativeVelocity -> 충돌한 두 객체의 상대적인 선 속도
         rigidbody -> 충돌한 게임오브젝트의 Rigidbody 컴포넌트
         */
        // 법선벡터가 이루는 회전 각도를 추출 (FromToRotation -> 특정 축을 기준으로 회전시킬 때 사용)
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);
        /*normal -> 충돌 지점의 법선, otherCollider -> 충돌 지점의 다른 Collider,
         *point -> 충돌 지점의 위치, separation -> 충돌한 두 Collider 간의 거리,
         *thisCollider -> 충돌 지점의 첫 번째 collider
         */
        GameObject spark = Instantiate(sparkEffect, contact.point, rot);
        // 스파크 효과의 부모를 드럼통 또는 벽으로 설정
        spark.transform.SetParent(this.transform);
    }
}
