using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class MainPageBUI : BaseUI
    {
        public Button settingBtn;
        public Button backUpViewBtn;
        public Button powerBtn;
        public Button pareBtn;
        public GameObject topTips;

        public override void Ini()
        {
            base.Ini();
            settingBtn = transform.Find("buttom/Setting").GetComponent<Button>();
            backUpViewBtn = transform.Find("buttom/SignBtn").GetComponent<Button>();
            powerBtn = transform.Find("buttom/Power").GetComponent<Button>();
            pareBtn = transform.Find("buttom/PareBtn").GetComponent<Button>();
            topTips = transform.Find("TopTips").gameObject;

            pareBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Pare");
                }
            });

            powerBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Power");
                }
            });

            settingBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Setting");
                }
            });
            backUpViewBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Sign");
                }
            });
            topTips.GetComponent<Button>().onClick.AddListener(() => {
                if (events != null)
                {
                    events("BackupBtn");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            topTips.SetActive(!FlowManager.Instance.IsBackUp());
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
