using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    public Button buttonW;
    public Button buttonS;
    public Button buttonA;
    public Button buttonD;
    public Button buttonQ;
    public Button buttonR;
    public Button buttonI;
    public Button buttonMouse0;
    public Button buttonMouse1;
    public bool isRun = false;
    bool isInventoryOpen = false;
    void Start()
    {

    }

    private void Update()
    {
        ButtonUp();
        ButtonDown();
        ButtonLeft();
        ButtonRight();
        ButtonRun();
        ButtonReload();
        ButtonInventory();
        ButtonMouse0();
        ButtonMouse1();
        if (isInventoryOpen == true)
        {
            GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ButtonW").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ButtonS").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ButtonD").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ButtonA").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ButtonQ").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("MoveText").gameObject.SetActive(false);


        }
        else
        {
            GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("ButtonW").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ButtonS").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ButtonD").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ButtonA").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("ButtonQ").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("MoveText").gameObject.SetActive(true);
        }
    }


    void ButtonUp()
    {
        if (Input.GetKey("w"))
        {
            buttonW.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonW.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonDown()
    {
        if (Input.GetKey("s"))
        {
            buttonS.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonS.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonLeft()
    {
        if (Input.GetKey("a"))
        {
            buttonA.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonA.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonRight()
    {
        if (Input.GetKey("d"))
        {
            buttonD.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonD.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonRun()
    {
        if (Input.GetKeyUp("q"))
        {
            isRun = !isRun;
            buttonQ.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonQ.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonReload()
    {
        if (Input.GetKey("r"))
        {
            buttonR.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonR.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonInventory()
    {
        if (Input.GetKeyUp("i"))
        {
            isInventoryOpen = !isInventoryOpen;
            buttonI.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonI.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonMouse0()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonMouse0.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonMouse0.image.color = new Color(1, 1, 1, 1);
        }
    }
    void ButtonMouse1()
    {
        if (Input.GetMouseButtonDown(1))
        {
            buttonMouse1.image.color = new Color(1, 0, 0, 1);
        }
        else
        {
            buttonMouse1.image.color = new Color(1, 1, 1, 1);
        }
    }
}
