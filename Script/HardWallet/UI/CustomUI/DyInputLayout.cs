using HardwareWallet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DyInputLayout : MonoBehaviour {

    private List<Button> inputList = new List<Button>();
    private Vector2 space = new Vector2(13, 13);
    private Vector2 size = new Vector2(200, 200);
    private float totleW = 854.2f;
    private Text targetText;
    private int targetID = 0;
    private bool canMove = true;

    public void OnEnable()
    {
        if (inputList.Count == 0)
        {
            foreach (Transform v in this.transform)
            {
                if (v.GetComponent<Button>() != null)
                {
                    inputList.Add(v.GetComponent<Button>());
                    v.GetComponent<Button>().onClick.AddListener(delegate()
                    {
                        ResetTargetBackGround();
                        PinCodeBip.Instace.PinCode = v.GetComponentInChildren<Text>().text;
                        targetText = v.GetComponentInChildren<Text>();
                        v.GetComponent<Button>().image.color = Color.gray;
                        targetID = GetIndex(v.GetComponent<Button>());Debug.Log(targetID);
                        canMove = false;
                    });
                }
            }
        }
        totleW = this.GetComponent<RectTransform>().sizeDelta.x;
        foreach (var v in inputList)
        {
            v.GetComponentInChildren<Text>().text = "";
        }
        ResetTargetBackGround();
        ResetTargetText();
        canMove = true;
        targetID = 0;
        targetText = inputList[0].GetComponentInChildren<Text>();
        inputList[0].image.color = Color.gray;
        //ReLayout("");
    }

    public string GetText()
    {
        string result = "";
        for(int i = 0; i < inputList.Count; i++)
        { 
            result += inputList[i].GetComponentInChildren<Text>().text;
            if (i != inputList.Count - 1)
            {
                result += " ";
            }
        }
        return result;
    }

    //public void ReLayout(string s)
    //{
    //    float currentX = 0;
    //    float currentY = 0;
    //    foreach (var v in inputList)
    //    {
    //        float preW = size.x;
    //        if (v.GetComponentInChildren<Text>().text.Length > 1)
    //        {
    //            preW = Mathf.Clamp(v.preferredWidth + size.x / 2f, size.x, totleW);
    //        }
    //        v.GetComponent<RectTransform>().sizeDelta = new Vector2(preW, size.y);
    //        if (currentX == 0)
    //        {
    //            v.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX, currentY);
    //            currentX += preW;
    //        }
    //        else if ((totleW - currentX - space.x) >= preW)
    //        {
    //            currentX += space.x;
    //            v.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX, currentY);
    //            currentX += preW;
    //        }
    //        else
    //        {
    //            currentY = currentY - size.y - space.y;
    //            currentX = 0;
    //            v.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX, currentY);
    //            currentX += preW;
    //        }
    //    }
    //}
    private void Update()
    {
        if(PinCodeBip.Instace.PinCode.Length > 4)
        {
            PinCodeBip.Instace.PinCode = PinCodeBip.Instace.PinCode.Substring(0, 4);
        }
        if (PinCodeBip.Instace.PinCode != targetText.text)
            canMove = true;
        targetText.text = PinCodeBip.Instace.PinCode;
        if(targetText.text.Length>=4 && canMove)
        {
            MoveNext();
        }
    }

    private void ResetTargetBackGround()
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            inputList[i].image.color = Color.white;
        }
    }

    private void ResetTargetText()
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            inputList[i].GetComponentInChildren<Text>().text = "";
        }
    }

    private void MoveNext()
    {
        if(targetID < 11)
        {
            inputList[targetID].image.color = Color.white;
            targetID++;
            targetText = inputList[targetID].GetComponentInChildren<Text>();
            PinCodeBip.Instace.PinCode = inputList[targetID].GetComponentInChildren<Text>().text;
            inputList[targetID].image.color = Color.gray;
        }
    }

    private int GetIndex(Button v)
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            if (inputList[i].gameObject.name == v.gameObject.name)
                return i;
        }
        return 0;
    }
}
