using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ResumeWalletPanel : MonoBehaviour
{
    public InputField inputWalletName;

    public InputField inputPassword;
    public GameObject eyeOpenMark;
    public GameObject eyeClosedMark;

    public InputField inputDbPassword;
    public GameObject eyeOpenMark1;
    public GameObject eyeClosedMark1;

    public Text noteText;


    public List<InputField> seedWordInputList = new List<InputField>();

    //void Start()
    //{
    //    string bip = QRPayTools.CreateBipString();
    //    Debug.Log(bip);
    //    string numberBIP = QRPayTools.ChangeBipLanguageToSpNumber(bip);
    //    Debug.Log(numberBIP);
    //    Debug.Log(QRPayTools.ChangeBipNumberToSpChinese(numberBIP));


    //    string numberBip = seedWordInputList[0].text;
    //    for (int i = 1; i < seedWordInputList.Count; i++)
    //    {
    //        numberBip += " " + seedWordInputList[i].text;
    //    }
    //    Debug.Log(numberBip);
    //}
    public bool isTest;

    public void OpenMe()
    {
        gameObject.SetActive(true);
        inputWalletName.text = "";
        inputDbPassword.text = "";
        inputPassword.text = "";
        inputDbPassword.contentType = InputField.ContentType.Password;
        inputPassword.contentType = InputField.ContentType.Password;
        eyeOpenMark.SetActive(false);
        eyeClosedMark.SetActive(true);
        eyeOpenMark1.SetActive(false);
        eyeClosedMark1.SetActive(true);
        noteText.gameObject.SetActive(false);

        if (!isTest)
        {
            for (int i = 0; i < seedWordInputList.Count; i++)
            {
                seedWordInputList[i].text = "";
                seedWordInputList[i].onValueChanged.RemoveListener(CHeckInput);
                seedWordInputList[i].onValueChanged.AddListener(CHeckInput);
            }
        }
       
    }
    public GameObject lastSelectedGameObject;

    private void CHeckInput(string v)
    {
        GameObject g = EventSystem.current.currentSelectedGameObject;
        bool isSelect = false;
        int id = 0;
        for (int i = 0; i < seedWordInputList.Count; i++)
        {
            if (g == seedWordInputList[i].gameObject)
            {
                isSelect = true;
                id = i;
                break;
            }
        }

        if (isSelect && id != seedWordInputList.Count - 1)
        {
            if (seedWordInputList[id].text.Length == 4)
            {
                lastSelectedGameObject = seedWordInputList[id + 1].gameObject;
                g.GetComponent<InputField>().DeactivateInputField();
                TimerManager.Instance.AddTimer(1F, () => ActivateNext());
            }
        }
    }

    private void ActivateNext()
    {
        EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
        lastSelectedGameObject.GetComponent<InputField>().ActivateInputField();
    }


    public bool isWriteOver()
    {
        for (int i = 0; i < seedWordInputList.Count; i++)
        {
            if (seedWordInputList[i].text.Length != 4)
            {
                return false;
            }
        }

        return true;
    }


    public void ShowOrHidePassword(InputField inputText)
    {
        if (inputText.contentType == InputField.ContentType.Password)
        {
            inputText.contentType = InputField.ContentType.Standard;
            if (inputText == inputPassword)
            {
                eyeOpenMark.SetActive(true);
                eyeClosedMark.SetActive(false);
            }
            else
            {
                eyeOpenMark1.SetActive(true);
                eyeClosedMark1.SetActive(false);
            }
        }
        else
        {
            inputText.contentType = InputField.ContentType.Password;
            if (inputText == inputPassword)
            {
                eyeOpenMark.SetActive(false);
                eyeClosedMark.SetActive(true);
            }
            else
            {
                eyeOpenMark1.SetActive(false);
                eyeClosedMark1.SetActive(true);
            }
        }

        inputText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(true);
    }


    public void OnClickResumeBtn()
    {
        if (!isWriteOver())
        {
            ShowNoticeText("请输入完整的助记词");
            return;
        }

        if (string.IsNullOrEmpty(inputWalletName.text))
        {
            ShowNoticeText("钱包名不能为空");
            return;
        }

        if (string.IsNullOrEmpty(inputPassword.text))
        {
            ShowNoticeText("密码不能为空");
            return;
        }
        else
        {
            if (inputPassword.text.Length > 12 || inputPassword.text.Length < 8)
            {
                ShowNoticeText("密码长度必须在8-12位之间");
                return;
            }

            if (!SeedKeyManager.Instance.IsContainLetter(inputPassword.text) || !SeedKeyManager.Instance.IsContainNumber(inputPassword.text))
            {
                ShowNoticeText("密码必须同时包含数字与字母");
                return;
            }
            else if (!inputPassword.text.Equals(inputDbPassword.text))
            {
                ShowNoticeText("两次输入的密码不相同");
                return;
            }
        }

        string numberBip = seedWordInputList[0].text;
        for (int i = 1; i < seedWordInputList.Count; i++)
        {
            numberBip += " " + seedWordInputList[i].text;
        }

        PanelManager._Instance.loadingPanel.SetActive(true);

        string bip = QRPayTools.ChangeBipNumberToSpChinese(numberBip);
        if (string.IsNullOrEmpty(bip))
        {
            ShowNoticeText("输入的助记词不正确");
            PanelManager._Instance.loadingPanel.SetActive(false);
            return;
        }

        SeedKeyManager.Instance.firstSeedBip = bip;
        SeedKeyManager.Instance.SetSeedBipArr(bip);
        //SeedKeyManager.Instance.masterPuKey = QRPayTools.GetMastPubKey(bip);
        SeedKeyManager.Instance.pingCode = inputPassword.text;

        string walletName = "";
        if (inputWalletName.text.Length < 4)
        {
            walletName = inputWalletName.text;
        }
        else
        {
            walletName = inputWalletName.text.Substring(0, 4);
        }
        //1621 0901 1915 1026 0891 1778 0163 1912 1867 0277 0496 1318
        SeedKeyManager.Instance.walletSN = walletName;

        PanelManager._Instance._loginPanel.InitApp();

        PanelManager._Instance.loadingPanel.SetActive(false);

        PanelManager._Instance._createOrResume.gameObject.SetActive(false);
    }

    private void ShowNoticeText(string notex)
    {
        CancelInvoke();
        noteText.gameObject.SetActive(true);
        noteText.text = notex;
        Invoke("HideNoticeText", 2);
    }

    private void HideNoticeText()
    {
        noteText.gameObject.SetActive(false);
    }

}
