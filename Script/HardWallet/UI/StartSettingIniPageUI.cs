using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;
using System.Threading;

namespace HardwareWallet
{
    public class StartSettingIniPageUI : BaseUI
    {
        public Button cancelBtn;
        public Button nextBtn;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            cancelBtn = transform.Find("BackBtn").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            nextBtn = transform.Find("Next").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("Next");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            Battery.Instance.SetInfo(FlowManager.Instance.language.GetWord(Words.欢迎));
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
