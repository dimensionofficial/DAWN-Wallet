using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HardwareWallet
{
    public class PinCodeUI : MonoBehaviour
    {
        public static PinCodeUI Instace;

        [SerializeField]
        Text title;
        [SerializeField]
        SafeKeyBoard keyBoard;
        [SerializeField]
        ViewPsd viewPsd;
        [SerializeField]
        Button backBtn;
        [SerializeField]
        Text info;
        [SerializeField]
        Text toptitle;
        private Text pincodeWord;

        private System.Action<string> OnFinished;
        private System.Action OnBack;
        private string pincode = "";
        float infoPos = 0;

        public string PinCode
        {
            get { return pincode; }
            set {
                pincode = value;
                pincodeWord.text = "";
                if (pincode.Length > 8)
                {
                    pincode = pincode.Substring(0, 8);
                }
                for (int i = 0; i < pincode.Length; i++)
                {
                    if (viewPsd.IsOn)
                    {
                        pincodeWord.text += pincode[i];
                    }
                    else
                    {
                        pincodeWord.text += "*";
                    }
                }
            }
        }

        void Awake()
        {
            Instace = this;
            pincodeWord = transform.Find("psw/star/star").GetComponent<Text>();
            backBtn.onClick.AddListener(() => {
                Hide();
                if (OnBack != null)
                {
                    OnBack();
                }
            });
            viewPsd.OnValueChange = () => {
                PinCode = pincode;
            };
            Hide();
            infoPos = info.GetComponent<RectTransform>().anchoredPosition.x;
        }

        public void Show(System.Action<string> OnFinished, string title = null, System.Action OnBack = null, string iniInfo = "", string toptitle="")
        {
            if (title == null)
            {
                title = FlowManager.Instance.language.GetWord(Words.输入密码);
            }
            info.text = iniInfo;
            this.gameObject.SetActive(true);
            this.OnFinished = OnFinished;
            this.OnBack = OnBack;
            if (OnBack == null)
            {
                backBtn.gameObject.SetActive(false);
            }
            else
            {
                backBtn.gameObject.SetActive(true);
            }
            viewPsd.IsOn = false;
            PinCode = "";
            this.title.text = title;
            this.toptitle.text = toptitle;
            keyBoard.Show(
                (i) => {
                    if (FlowManager.Instance.GetHandLockSec() > 0)
                    {
                        return;
                    }
                    PinCode += "" + i;
                    info.text = "";
                }, 
                () => {
                    if (FlowManager.Instance.GetHandLockSec() > 0)
                    {
                        return;
                    }
                    if (FlowManager.PINCODEVERYFI ==
                    SaveAndLoad.LoadString(FlowManager.PINCODEVERYFI, System.Text.UTF8Encoding.UTF8.GetBytes(pincode)))
                    {
                        string bip = SaveAndLoad.LoadString(FlowManager.BIPKEYWORDS, System.Text.UTF8Encoding.UTF8.GetBytes(pincode));
                        if (bip == null || !QRPayTools.VerifyBip(bip))
                        {
                            //PopBox.Instance.ShowMsg("cant read bip from local");
                            info.text = FlowManager.Instance.language.GetWord(Words.密码错误);
                            Handheld.Vibrate();
                            StartCoroutine(ShakInfo());
                            FlowManager.Instance.LockHandCodeAndPinCode();
                            return;
                        }
                        Hide();
                        FlowManager.Instance.ClearHandCodeErrorCode();
                        if (OnFinished != null)
                        {
                            OnFinished(pincode);
                        }
                    }
                    else
                    {
                        if (FlowManager.Instance.GetHandLockSec() > 0)
                        {
                            return;
                        }
                        if (pincode.Length < 8)
                        {
                            info.text = FlowManager.Instance.language.GetWord(Words.请输入6位数字密码);
                            PinCode = "";
                            Handheld.Vibrate();
                            StartCoroutine(ShakInfo());
                        }
                        else if (PlayerPrefs.HasKey(FlowManager.BIPKEYWORDS))
                        {
                            info.text = FlowManager.Instance.language.GetWord(Words.密码错误);
                            PinCode = "";
                            Handheld.Vibrate();
                            FlowManager.Instance.LockHandCodeAndPinCode();
                            StartCoroutine(ShakInfo());
                        }
                        else
                        {
                            Hide();
                            FlowManager.Instance.ClearHandCodeErrorCode();
                            if (OnFinished != null)
                            {
                                OnFinished(pincode);
                            }
                        }
                    }
                }, 
                () => {
                    if (FlowManager.Instance.GetHandLockSec() > 0)
                    {
                        return;
                    }
                    PinCode = PinCode.Substring(0, (int)Mathf.Clamp(PinCode.Length - 1, 0, Mathf.Infinity));
                }
                );
            if (!string.IsNullOrEmpty(iniInfo))
            {
                StartCoroutine(ShakInfo());
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            keyBoard.Hide();
        }

        public IEnumerator ShakInfo()
        {
            int i = 0;
            while (i < 10)
            {
                float offset = i % 2 == 0 ? -10 : 10;
                info.GetComponent<RectTransform>().anchoredPosition = new Vector2(infoPos + offset,
                    info.GetComponent<RectTransform>().anchoredPosition.y);
                yield return new WaitForSeconds(0.05f);
                i++;
            }
            info.GetComponent<RectTransform>().anchoredPosition = new Vector2(infoPos,
                   info.GetComponent<RectTransform>().anchoredPosition.y);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OnBack != null)
                {
                    Hide();
                    OnBack();
                }
            }
            if (FlowManager.Instance.GetHandLockSec() > 0)
            {
                this.info.text = FlowManager.Instance.language.GetWord(Words.设备停用).Replace("{0}",
                    FlowManager.Instance.GetHandLockSec().ToString());
                return;
            }
            else if (this.info.text.StartsWith(FlowManager.Instance.language.GetWord(Words.设备停用).Substring(0, 4)))
            {
                this.info.text = "";
            }
        }
    }
}
