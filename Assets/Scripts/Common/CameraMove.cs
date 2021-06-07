using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;//추적할 대상
    //이동 속도 계수, 회전속도 계수, 추적 대상과의 거리, 추적 대상과의 높이, 추적 좌표의 오프셋
    public float moveDamping = 15.0f, rotateDamping = 10.0f, distance = 5.0f, height = 4.0f, targetOffset = 2.0f;
    //cameraTig의  Transform 컴포넌트
    Transform tr;
    public float Zoomdistance = 10.0f;
    bool Zoomstat = false;
    public float rotSpeed = 80.0f;
    float y = 0.0f, r = 0.0f;
    void Start()
    {
        tr = GetComponent<Transform>();
    }


    void LateUpdate() //주인공 캐릭터의 이동 로직이 완료된 후 처리하기 위해 LateUpdate에서 구현
    {

        if (Input.GetMouseButtonUp(1))
        {
            if (Zoomstat == false)
            {
                Zoomstat = true;
                PlayerCam.GetComponent<Camera>().fieldOfView = fieldview;
                fieldview -= 50;


            }
            else
            {
                Zoomstat = false;
                PlayerCam.GetComponent<Camera>().fieldOfView = fieldview;
                fieldview += 50;
            }

        }
        //카메라의 높이와 거리를 계산
        var camPos = target.position - (target.forward * distance) + (target.up * height);
        //이동할 때의 속도 계수를 적용
        tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * moveDamping);
        //회전할 때의 속도 계수를 적용
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);
        //카메라를 추적 대상으로 z축을 회전시킴
        tr.LookAt(target.position + (target.up * targetOffset));
    }
    public float fieldview = 60;
    public GameObject PlayerCam;

    //public float rotSpeed = 200;
    //float mx, my;
    //private void Update()
    //{
    //    float h = Input.GetAxis("Mouse X");
    //    float v = Input.GetAxis("Mouse Y");

    //    mx += h * rotSpeed * Time.deltaTime;
    //    my += v * rotSpeed * Time.deltaTime;

    //    if(my >= 90)
    //    {
    //        my = 90;
    //    }
    //    else if (my>= -90)
    //    {
    //        my = -90;
    //    }
    //    transform.eulerAngles = new Vector3(-my, mx, 0);

    //}


}
