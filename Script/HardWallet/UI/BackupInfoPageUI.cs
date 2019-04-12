using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class BackupInfoPageUI : BaseUI
    {
        public Toggle dontShowBackInFuture;
        public Button backBtn;
        public Button nextBtn;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            dontShowBackInFuture = transform.Find("Toggle").GetComponent<Toggle>();
            backBtn = transform.Find("BackBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            nextBtn = transform.Find("NextBtn").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("nextBtn");
                }
            });
            dontShowBackInFuture.onValueChanged.AddListener((b) => {
                if (events != null)
                {
                    events(b ? "true" : "false");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            dontShowBackInFuture.isOn = true;
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
