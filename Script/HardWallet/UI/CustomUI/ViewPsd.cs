using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class ViewPsd : MonoBehaviour
    {
        public System.Action OnValueChange;
        private bool isOn = false;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;

                if (onPic == null)
                {
                    onPic = transform.Find("on").gameObject;
                }
                if (offPic == null)
                {
                    offPic = transform.Find("off").gameObject;
                }
                offPic.SetActive(!isOn);
                onPic.SetActive(isOn);
            }
        }
        private Button btn;
        GameObject onPic;
        GameObject offPic;
        // Use this for initialization
        void Start()
        {
            btn = this.gameObject.GetComponent<Button>();
            IsOn = false;
            btn.onClick.AddListener(() =>
            {
                IsOn = !isOn;
                if (OnValueChange != null)
                {
                    OnValueChange();
                }
            });
        }
    }
}
