using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    const string bulletTag = "BULLET";
    const string enemyTag = "ENEMY";
    float initHp = 100.0f;
    //float initSt = 100.0f;
    public float currHp;
    //public float currSt;
    //BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;
    //Hp Bar Image 를 저장하기 위한 변수
    public Image hpBar;

    public Button buttonHp;
    public Text hpPotionText;
    public int hpPotion;

    //생명 게이지의 처음 색상(녹색)
    readonly Color initHpColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    //readonly Color initStColor = new Vector4(0, 0, 1.0f, 1.0f);
    Color currHpColor;
    //Color currStColor;
    // Start is called before the first frame update
    void Start()
    {
        currHp = initHp;
        //currSt = initSt;
        //생명 게이지의 초기 생상을 설정
        hpBar.color = initHpColor;
        //stBar.color = initStColor;
        currHpColor = initHpColor;
        //currStColor = initStColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == bulletTag)
        {
            Destroy(other.gameObject);
            //혈흔 효과를 표현할 코루팀 함수 호출
            StartCoroutine(ShowBloodScreen());
            currHp -= 5.0f;
            //생명 게이지의 색상 및 크기 변경 함수를 호출
            DisplayHpbar();
            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }
    void PlayerDie()
    {
        //"ENEMY" 태그로 지정된 모든 적 캐릭터를 추출해 배열에 저장
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        /*
         * SendMessage 함수는 특정 게임오브젝트에 포함된 스크립트를 하나씩 검색해 
         * 호출하려는 함수가 있으면 실행하라고 메시지를 전달하는 것
         * 호출한 함수가 해당 게임오브젝트에 포함된 스크립트에 없다면 오류메시지가 반환되는데,
         * SendMessageOptions 는 이런 오류메시지를 리턴 받을 것인지 여부를 결정하는 것임
         */
    }
    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }
    void DisplayHpbar()
    {
        //생명 수치가 50% 일 때까지는 녹색에서 노란색으로 변경
        if((currHp / initHp) > 0.5f)
        {
            currHpColor.r = (1 - (currHp / initHp)) * 2.0f;
        }
        else // 생명 수치가 0%일 때 까지는 노란색에서 빨간색으로 변경
        {
            currHpColor.g = (currHp / initHp) * 2.0f;
        }

        hpBar.color = currHpColor;
        hpBar.fillAmount = (currHp / initHp);
    }

    public void PlusHp()
    {
        if (hpPotion > 0 && currHp != initHp)
        {
            currHp = initHp;
            hpPotion--;
            hpPotionText.text = hpPotion.ToString();
        }
    }
}
