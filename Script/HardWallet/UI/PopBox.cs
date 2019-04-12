using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class PopBox : MonoBehaviour
    {
        private static PopBox instance;
        public static PopBox Instance
        {
            get {
                return instance;
            }
        }

        [SerializeField]
        Text info;
        [SerializeField]
        Text title;
        [SerializeField]
        GameObject achor;

        System.Action onFinished;

        void Awake()
        {
            instance = this;
        }

 //       [Obfuscator.Attribute.DoNotRename]
        public void ShowMsg(string msg, System.Action onFinished = null, string title = "")
        {
            if (title != "")
            {
                this.title.text = title;
            }
            else
            {
                this.title.text = FlowManager.Instance.language.GetWord(Words.错误);
            }
            info.text = msg;
            this.onFinished = onFinished;
            achor.SetActive(true);
        }

//        [Obfuscator.Attribute.DoNotRename]
        public void Hide()
        {
            achor.SetActive(false);
            if (onFinished != null)
            {
                onFinished();
            }
        }
    }
}
