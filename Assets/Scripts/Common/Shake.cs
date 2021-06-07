using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //셰이크 효과를 줄 카메라의 Transform을 저장할 변수
    public Transform shakeCamera;
    //회전 시킬 것인지를 판단할 변수
    public bool shakeRotate = false;
    //초기 좌표와 회전값을 저장할 변수
    Vector3 originPos;
    Quaternion originRot;

    // Start is called before the first frame update
    void Start()
    {
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        float passTime = 0.0f;
        while (passTime < duration)
        {
            //불규칙한 위치를 산출
            Vector3 shakePos = Random.insideUnitSphere; // 반경이 1인 구체 내부의 3차원 좌표값을 불규칙하게 반환
            //카메라의 위치를 변경
            shakeCamera.localPosition = originPos - (shakePos * magnitudePos);
            if (shakeRotate) // 불규칙한 회전값까지 사용하겠다면..
            {
                /*
                 * 펄린노이즈는 0과 1사이의 난수를 발생시키지만 일반 난수 발생기와는 다르게
                 * 연속성이 있는 난수를 발생시키는 것으로 무작위한 Terrain, Texture, 구름 등을 생성할 때
                 * 유용하게 사용되며, 실제로 유니티에 탑재된 Terrain Engine의 풀을 심는 기능에서도 사용하고 있다.
                    - 유니티에서는 2차원 펄린 노이즈 함수만 제공함.
                 */
                //불규칙한 회전값을 펄린 노이즈 함수를 이용해 추출
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                //카메라의 회전값을 변경
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            //진동 시간을 누적
            passTime += Time.deltaTime;
            yield return null;
        }
        //진동이 끝난 후 카메라의 초기값으로 설정
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;

    }

}
