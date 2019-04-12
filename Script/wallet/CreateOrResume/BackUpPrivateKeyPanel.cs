using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class BackUpPrivateKeyPanel : MonoBehaviour
{

    public GameObject introduceObject;

    public GameObject showWordObject;
    public List<Text> wordList = new List<Text>();

    private string[] seedWorldList;


    public GameObject confirmObject;
    public List<BtnWordItem> upWordItemList = new List<BtnWordItem>();
    public List<BtnWordItem> downWordItemList = new List<BtnWordItem>();


    public void OpenMe()
    {
        gameObject.SetActive(true);
        introduceObject.SetActive(true);
        showWordObject.SetActive(false);
        confirmObject.SetActive(false);
    }


    public void OnClickIntroDuceBackBtn()
    {
        gameObject.SetActive(false);
    }
    public void OnClickShowSeedWordBackBtn()
    {
        introduceObject.SetActive(true);
        showWordObject.SetActive(false);
    }
    public void OnClickCofirmBackBtn()
    {
        showWordObject.SetActive(true);
        confirmObject.SetActive(false);
    }

    public void OnClickShowSeedWordBtn()
    {
        introduceObject.SetActive(false);
        showWordObject.SetActive(true);
        confirmObject.SetActive(false);

        string str = QRPayTools.ChangeBipLanguageToSpNumber(SeedKeyManager.Instance.firstSeedBip);

        string[] sArray = Regex.Split(str, " ", RegexOptions.IgnoreCase);
        for (int i = 0; i < sArray.Length; i++)
        {
            wordList[i].text = sArray[i];
        }
        seedWorldList = sArray;
    }

    public void OnClickConfirmBtn()
    {
        showWordObject.SetActive(false);
        confirmObject.SetActive(true);

        for (int i = 0; i < upWordItemList.Count; i++)
        {
            upWordItemList[i].Text.text = "";
            upWordItemList[i].targetItem = null;
        }

        string[] radomStr = GetDisruptedItems(seedWorldList);
        for (int i = 0; i < downWordItemList.Count; i++)
        {
            downWordItemList[i].Btn.interactable = true;
            downWordItemList[i].Text.text = radomStr[i];
        }
    }

    public string[] GetDisruptedItems(string[] item)
    {
        //生成一个新数组：用于在之上计算和返回
        string[] temp;
        temp = new string[item.Length];
        for (int i = 0; i < temp.Length; i++) { temp[i] = item[i]; }
        //打乱数组中元素顺序
        System.Random rand = new System.Random(DateTime.Now.Millisecond);
        for (int i = 0; i < temp.Length; i++)
        {
            int x, y; string t;
            x = rand.Next(0, temp.Length);
            do
            {
                y = rand.Next(0, temp.Length);
            } while (y == x);
            t = temp[x];
            temp[x] = temp[y];
            temp[y] = t;
        }
        return temp;
    }

    public void OnClickOkBtn()
    {
        for (int i = 0; i < seedWorldList.Length; i++)
        {
            if (!upWordItemList[i].Text.text.Equals(seedWorldList[i]))
            {
                PopupLine.Instance.Show("请填写正确的助记词");
                return;
            }
        }

        SeedKeyManager.Instance.SetBackup();

        gameObject.SetActive(false);
        PanelManager._Instance._mainPanel.noColdWalletShow.SetActive(false);
    }

    /// <summary>
    /// 点击下面的选着
    /// </summary>
    /// <param name="item"></param>
    public void OnClickSetShowWord(BtnWordItem item)
    {
        for (int i = 0; i < upWordItemList.Count; i++)
        {
            if (upWordItemList[i].targetItem == null)
            {
                item.Btn.interactable = false;
                upWordItemList[i].targetItem = item;
                upWordItemList[i].Text.text = item.Text.text;
                return;
            }
        }
    }
    /// <summary>
    /// 点击上面的取消
    /// </summary>
    /// <param name="item"></param>
    public void OnClickReSetShowWord(BtnWordItem item)
    {
        if (item.targetItem != null)
        {
            item.targetItem.Btn.interactable = true;
            item.Text.text = "";
            item.targetItem = null;
        }
    }

    void Start()
    {
        //string str = QRPayTools.ChangeBipLanguageToSpNumber(SeedKeyManager.Instance.seedBip);
        //Debug.Log(str);
        //string[] sArray = Regex.Split(str, " ", RegexOptions.IgnoreCase);
        //string[] abs = GetDisruptedItems(sArray);
        //for (int i = 0; i < abs.Length; i++)
        //{
        //    wordList[i].text = abs[i];
        //}
    }
}
