using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;

namespace HardwareWallet
{
    public class SelectLanguageUI : BaseUI
    {
        public Button nextBtn;
        public Button jp;
        public Button spchinese;
        public Button chinese;
        public Button english;

        public override void Ini()
        {
            base.Ini();
            jp = transform.Find("jp").GetComponent<Button>();
            spchinese = transform.Find("sp").GetComponent<Button>();
            chinese = transform.Find("chinese").GetComponent<Button>();
            english = transform.Find("english").GetComponent<Button>();
            jp.onClick.AddListener(() => {
                if (events != null)
                {
                    events("jp");
                }
            });
            spchinese.onClick.AddListener(() => {
                if (events != null)
                {
                    events("spchinese");
                }
            });
            chinese.onClick.AddListener(() => {
                if (events != null)
                {
                    events("chinese");
                }
            });
            english.onClick.AddListener(() => {
                if (events != null)
                {
                    events("english");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
