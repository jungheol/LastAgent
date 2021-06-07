using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class PlayerCtrl : MonoBehaviour
{
    Image runSignal;

    float initSt = 100.0f;
    public float currSt;
    public Image stBar;
    readonly Color initStColor = new Vector4(0, 0, 1.0f, 1.0f);
    Color currStColor;

    public Button buttonSt;
    public Text stPotionText;
    public int stPotion;

    float h = 0.0f, v = 0.0f, r = 0.0f, y = 0.0f;
    Transform tr;
    public float moveSpeed = 5.0f, rotSpeed = 80.0f;
    TutorialButton tutorialButton;
    Animation animation;
    public AnimationClip[] animations;
    void Start()
    {
        animation = GetComponent<Animation>();
        animation.clip = animations[0];
        animation.Play();
        tr = GetComponent<Transform>();
        tutorialButton = GameObject.Find("ButtonCtrl").GetComponent<TutorialButton>();
        runSignal = GameObject.Find("RunSignal").GetComponent<Image>();

        currSt = initSt;
        stBar.color = initStColor;
        currStColor = initStColor;
    }

    void Update()
    {
        r = Input.GetAxis("Mouse X");
        //y = Input.GetAxis("Mouse Y");
        if (tutorialButton.isRun == true && currSt > 0)
        {
            PlayerMove(15.0f);
            runSignal.color = new Color(0, 1, 0, 1);
            StartCoroutine(UseStamina());
            DisplayStbar();
        }
        else
        {
            tutorialButton.isRun = false;
            PlayerMove(5.0f);
            runSignal.color = new Color(1, 1, 1, 1);
            DisplayStbar();
        }
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
        //tr.Rotate(Vector3.left * rotSpeed * Time.deltaTime * y);
        if (v >= 0.1f) // 앞으로 이동할 때
        {
            animation.CrossFade(animations[1].name, 0.3f);//전진 애니메이션 수행
        }
        else if (v <= -0.1f) // 뒤로 이동할 때
        {
            animation.CrossFade(animations[2].name, 0.3f);//후진 애니메이션 수행
        }
        else if (h >= 0.1f) // 오른쪽으로 이동할 때
        {
            animation.CrossFade(animations[3].name, 0.3f);//오른쪽 이동 애니메이션 수행
        }
        else if (h <= -0.1f) // 왼쪽으로 이동할 때
        {
            animation.CrossFade(animations[4].name, 0.3f);//왼쪽 이동 애니메이션 수행
        }
        else
        {
            animation.CrossFade(animations[0].name, 0.3f);//대기 애니메이션 수행
        }

    }

    public void PlayerMove(float movespeed)
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Debug.Log(movespeed);
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * movespeed * Time.deltaTime, Space.Self);
    }

    void DisplayStbar()
    {
        //생명 수치가 50% 일 때까지는 녹색에서 노란색으로 변경
        if ((currSt / initSt) > 0.5f)
        {
            currStColor.r = (1 - (currSt / initSt)) * 2.0f;
        }
        else // 생명 수치가 0%일 때 까지는 노란색에서 빨간색으로 변경
        {
            currStColor.g = (currSt / initSt) * 2.0f;
        }

        stBar.color = currStColor;
        stBar.fillAmount = (currSt / initSt);
    }

    IEnumerator UseStamina()
    {
        while (currSt > 0 && tutorialButton.isRun == true)
        {
            currSt -= 1.0f * Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void PlusStamina()
    {
        if (stPotion > 0 && currSt != initSt)
        {
            currSt = initSt;
            stPotion--;
            stPotionText.text = stPotion.ToString();
        }
    }
}
