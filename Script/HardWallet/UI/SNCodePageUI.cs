using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class SNCodePageUI : BaseUI
    {
        SafeKeyBoard keyBoard;
        Text pincodeWord;
        private string pincode = "";
        GameObject complete;
        Button nextBtn;

        public string PinCode
        {
            get { return pincode; }
            set
            {
                pincode = value;
                pincodeWord.text = "";
                if (pincode.Length > 4)
                {
                    pincode = pincode.Substring(0, 4);
                }
                for (int i = 0; i < pincode.Length; i++)
                {
                    pincodeWord.text += pincode[i];
                }
            }
        }
        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            complete = transform.Find("StartIniCompletePanel").gameObject;
            complete.SetActive(false);
            pincodeWord = transform.Find("psw/star/star").GetComponent<Text>();
            keyBoard = transform.Find("SafeKeyBoard").GetComponent<SafeKeyBoard>();
            Button backBtn = transform.Find("BackBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            Button next = transform.Find("StartIniCompletePanel/Next").GetComponent<Button>();
            next.onClick.AddListener(() => {
                if (events != null)
                {
                    events("create");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            complete.gameObject.SetActive(false);
            PinCode = "";
            keyBoard.Show(
                (i) => {
                    PinCode += "" + i;
                }, 
                () => {
                    if (pincode.Length < 4)
                    {
                        Handheld.Vibrate();
                    }
                    else
                    {
                        if (events != null)
                        {
                            events(pincode);
                        }
                        ShowComplete();
                    }
                }, 
                () => {
                    PinCode = PinCode.Substring(0, (int)Mathf.Clamp(PinCode.Length - 1, 0, Mathf.Infinity));
                });
        }

        private void ShowComplete()
        {
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.钱包创建完成));
            complete.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
