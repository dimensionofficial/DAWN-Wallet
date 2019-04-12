using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class PinCodeBip : MonoBehaviour
    {
        public static PinCodeBip Instace;

        [SerializeField]
        SafeKeyBoard keyBoard;

        private System.Action<string> OnFinished;
        private System.Action OnBack;
        private string pincode = "";
        float infoPos = 0;

        public string PinCode
        {
            get { return pincode; }
            set
            {
                pincode = value;
            }
        }

        void Awake()
        {
            Instace = this;
            Hide();
            //infoPos = info.GetComponent<RectTransform>().anchoredPosition.x;
        }

        public void Show(System.Action<string> OnFinished, string title = null, System.Action OnBack = null, string iniInfo = "", string toptitle = "")
        {
            this.gameObject.SetActive(true);
            this.OnFinished = OnFinished;
            this.OnBack = OnBack;
            PinCode = "";
            keyBoard.Show(
                (i) => {
                    if (FlowManager.Instance.GetHandLockSec() > 0)
                    {
                        return;
                    }
                    PinCode += "" + i;
                    //info.text = "";
                },
                () => {
                    if (OnFinished != null)
                    {
                        OnFinished(pincode);
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
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            //keyBoard.Hide();
        }


        void Update()
        {

        }

    }
}

