using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class SettingPageUI : BaseUI
    {
        Button backUp;
        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            Button backBtn = transform.Find("BackBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            Button coinManagerBtn = transform.Find("Panel/cointypemanager").GetComponent<Button>();
            coinManagerBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("coinType");
                }
            });
            Button handCode = transform.Find("Panel/handcode").GetComponent<Button>();
            handCode.onClick.AddListener(() => {
                if (events != null)
                {
                    events("handCode");
                }
            });
            Button lockTime = transform.Find("Panel/locktime").GetComponent<Button>();
            lockTime.onClick.AddListener(() => {
                if (events != null)
                {
                    events("lockTime");
                }
            });
            backUp = transform.Find("Panel/backup").GetComponent<Button>();
            backUp.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backUp");
                }
            });
            Button language = transform.Find("Panel/language").GetComponent<Button>();
            language.onClick.AddListener(() => {
                if (events != null)
                {
                    events("language");
                }
            });
            Button reset = transform.Find("Panel/reset").GetComponent<Button>();
            reset.onClick.AddListener(() => {
                if (events != null)
                {
                    events("reset");
                }
            });
            Button lighting = transform.Find("Panel/lighting").GetComponent<Button>();
            lighting.onClick.AddListener(() => {
                if (events != null)
                {
                    events("lighting");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            if (FlowManager.Instance.IsBackUp())
            {
                backUp.gameObject.SetActive(false);
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
