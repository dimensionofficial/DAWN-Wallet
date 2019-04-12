using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class LockScreenTimeUI : BaseUI
    {
        Text title;
        Slider slider;
        Text time;
        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            time = transform.Find("Text (2)").GetComponent<Text>();
            title = transform.Find("Text (1)").GetComponent<Text>();
            Button backBtn = transform.Find("BackBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });

            slider = transform.Find("Slider").GetComponent<Slider>();
            slider.onValueChanged.AddListener((f) => {
                if (title.text == FlowManager.Instance.language.GetWord(Words.锁屏时间))
                {
                    if (f == 60)
                    {
                        time.text = "永不锁屏";
                    }
                    else
                    {
                        time.text = ((int)f).ToString() + FlowManager.Instance.language.GetWord(Words.秒);
                    }
                }
                if (events != null)
                {
                    events(((int)f).ToString());
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            title.text = (string)param;
            if (title.text == FlowManager.Instance.language.GetWord(Words.锁屏时间))
            {
                int i = 15;
                if (PlayerPrefs.HasKey(FlowManager.LOCKTIME))
                {
                    i = PlayerPrefs.GetInt(FlowManager.LOCKTIME);
                }
                time.text = i + FlowManager.Instance.language.GetWord(Words.秒);
                slider.value = i;
            }
            else
            {
                float t = 0.8f;
                if (PlayerPrefs.HasKey(FlowManager.SCREENLIGHT))
                {
                    t = PlayerPrefs.GetFloat(FlowManager.SCREENLIGHT);
                }
                slider.value = 15f + 45f * t;
                time.text = "";
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
