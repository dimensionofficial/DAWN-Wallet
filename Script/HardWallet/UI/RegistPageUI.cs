using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class RegistPageUI : BaseUI
    {
        /*pincode*/
        public GameObject pincodepanel;
        public InputField pincode;
        public InputField pincodeconfirm;
        public Button pincodeNextBtn;
        /*import*/
        public GameObject importPanel;
        public DyInputLayout bipImportText;
        public Button importNextBtn;
        public Button importBackBtn;
        /*create new*/
        public GameObject createPanel;
        public Text bipCreateText;
        public Button createNextBtn;
        /*select mode*/
        public GameObject selectPanel;
        public Button selectCreateNextBtn;
        public Button selectImportNextBtn;
        public Button selectCancelBtn;

        public string tempPincode = "";
        public string tempBip = "";
        public string tempHandCode = "";

        public override void Ini()
        {
            base.Ini();
            pincodepanel = transform.Find("PinCode").gameObject;
            pincode = transform.Find("PinCode/PwdInputField").GetComponent<InputField>();
            pincodeconfirm = transform.Find("PinCode/PwdConfirmField").GetComponent<InputField>();
            pincodeNextBtn = transform.Find("PinCode/NextBtn").GetComponent<Button>();
            pincodeNextBtn.onClick.AddListener(() => {
                if (pincode.text != pincodeconfirm.text)
                {
                    PopBox.Instance.ShowMsg("Error PinCode");
                    pincode.text = "";
                    pincodeconfirm.text = "";
                    return;
                }
                else if (pincode.text.Length < 6 || pincode.text.Length > 16)
                {
                    PopBox.Instance.ShowMsg("Pincode must be 6-16 chars");
                    pincode.text = "";
                    pincodeconfirm.text = "";
                    return;
                }
                tempPincode = pincode.text;
                ShowSelectPanel();
            });

            importPanel = transform.Find("BipInput").gameObject;
            bipImportText = transform.Find("BipInput/bac").GetComponent<DyInputLayout>();
            importNextBtn = transform.Find("BipInput/Next").GetComponent<Button>();
            importNextBtn.onClick.AddListener(() => {
                string bip = bipImportText.GetText();
                bip = QRPayTools.ChangeBipNumberToSpChinese(bip);
                if (QRPayTools.VerifyBip(bip))
                {
                    if (events != null)
                    {
                        events(bip + "%" + tempPincode + "%" + tempHandCode);
                    }
                    PinCodeBip.Instace.Hide();
                }
                else
                {
                    bip = QRPayTools.ChangeBipLanguageToSpChinese(bip);
                    if (QRPayTools.VerifyBip(bip))
                    {
                        if (events != null)
                        {
                            events(bip + "%" + tempPincode + "%" + tempHandCode);
                        }
                    }
                    else
                    {
                        PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.助记词错误));
                    }
                }
            });
            importBackBtn = transform.Find("BipInput/BackBtn").GetComponent<Button>();
            importBackBtn.onClick.AddListener(() => {
                //bipImportText.text = "";
                ShowSelectPanel();
            });

            createPanel = transform.Find("BipRecord").gameObject;
            bipCreateText = transform.Find("BipRecord/bac/BipShow").GetComponent<Text>();
            createNextBtn = transform.Find("BipRecord/Next").GetComponent<Button>();
            createNextBtn.onClick.AddListener(() => {
                if (string.IsNullOrEmpty(bipCreateText.text))
                {
                    PopBox.Instance.ShowMsg("Create bip words failed");
                }
                else
                {
                    if (events != null)
                    {
                        events(bipCreateText.text + "%" + tempPincode + "%" + tempHandCode);
                    }
                }
            });

            selectPanel = transform.Find("CreateOrNew").gameObject;
            selectCreateNextBtn = transform.Find("CreateOrNew/Create").GetComponent<Button>();
            selectCancelBtn = transform.Find("CreateOrNew/BackBtn").GetComponent<Button>();
            selectCancelBtn.gameObject.SetActive(false);
            selectCreateNextBtn.onClick.AddListener(() => {
                //ShowCreatePanel();
                string bip = QRPayTools.CreateBipString();
                if (events != null)
                {
                    events(bip + "%" + tempPincode + "%" + tempHandCode);
                }
            });
            selectImportNextBtn = transform.Find("CreateOrNew/Import").GetComponent<Button>();
            selectImportNextBtn.onClick.AddListener(() => {
                ShowImportPanel();
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.设置密码1));
            PinCodeUI.Instace.Show(Step1, FlowManager.Instance.language.GetWord(Words.设置密码),
                null, "", FlowManager.Instance.language.GetWord(Words.请输入6位数字密码));
        }


        private void Step1(string pin)
        {
            tempPincode = pin;
            PinCodeUI.Instace.Show(Step2, FlowManager.Instance.language.GetWord(Words.重复密码),
                 null, "", FlowManager.Instance.language.GetWord(Words.请重复6位数字密码));
        }

        private void Step2(string pin)
        {
            if (tempPincode != pin)
            {
                Handheld.Vibrate();
                PinCodeUI.Instace.Show(Step1, FlowManager.Instance.language.GetWord(Words.设置密码), 
                    null, FlowManager.Instance.language.GetWord(Words.两次输入的密码不相同),
                    FlowManager.Instance.language.GetWord(Words.请输入6位数字密码));
            }
            else
            {
                Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.手势锁2));
                HandCodeUI.Instance.Show(Step3, FlowManager.Instance.language.GetWord(Words.请输入手势锁));
            }
        }

        private void Step3(string handCode)
        {
            tempHandCode = handCode;
            HandCodeUI.Instance.Show(Step4, FlowManager.Instance.language.GetWord(Words.请重复手势锁));
        }

        private void Step4(string handCode)
        {
            if (tempHandCode != handCode)
            {
                Handheld.Vibrate();
                HandCodeUI.Instance.Show(Step3, FlowManager.Instance.language.GetWord(Words.请输入手势锁), 
                    FlowManager.Instance.language.GetWord(Words.两次手势不相同));
            }
            else
            {
                //over
                ShowSelectPanel();
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ShowPincodePanel()
        {
            pincodepanel.SetActive(true);
            importPanel.SetActive(false);
            createPanel.SetActive(false);
            selectPanel.SetActive(false);
        }

        public void ShowCreatePanel()
        {
            pincodepanel.SetActive(false);
            importPanel.SetActive(false);
            createPanel.SetActive(true);
            selectPanel.SetActive(false);
            string bip = QRPayTools.CreateBipString();
            bipCreateText.text = bip;
        }

        public void ShowImportPanel()
        {
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.钱包恢复));
            PinCodeBip.Instace.Show(CheckBip);//显示程序输入法
            pincodepanel.SetActive(false);
            importPanel.SetActive(true);
            createPanel.SetActive(false);
            selectPanel.SetActive(false);
        }

        public void ShowSelectPanel()
        {
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.创建OR恢复));
            PinCodeBip.Instace.Hide();
            pincodepanel.SetActive(false);
            importPanel.SetActive(false);
            createPanel.SetActive(false);
            selectPanel.SetActive(true);
        }

        public override void Update()
        {
            if (importPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                //uubipImportText.text = "";
                ShowSelectPanel();
            }
        }

        public void CheckBip(string bip)
        {
            importNextBtn.onClick.Invoke();
        }
    }
}
