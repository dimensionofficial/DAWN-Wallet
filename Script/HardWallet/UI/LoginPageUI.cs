using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HardwareWallet
{
    public class LoginUI : BaseUI
    {
        public Button loginBtn;
        public InputField passwordField;

        public override void Ini()
        {
            base.Ini();
            loginBtn = transform.Find("ConfirmBtn").GetComponent<Button>();
            passwordField = transform.Find("InputField").GetComponent<InputField>();
        }

        public override void Show(object param)
        {
            base.Show(param);
            passwordField.text = "";
            loginBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events(passwordField.text);
                }
            });
        }

        public override void Hide()
        {
            base.Hide();
            passwordField.text = "";
        }


    }
}
