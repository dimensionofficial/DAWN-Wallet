using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class LockTimeUI : BaseUI
    {
        List<Button> btnList = new List<Button>();
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
            Transform panel = transform.Find("Panel").transform;
            foreach (Transform v in panel)
            {
                Button btn = v.GetComponent<Button>();
                btnList.Add(btn);
                btn.onClick.AddListener(() => {
                    SelectBtn(btn.gameObject.name);
                });
            }
        }

        public override void Show(object param)
        {
            base.Show(param);
            int i = 30;
            if (PlayerPrefs.HasKey(FlowManager.LOCKTIME))
            {
                i = PlayerPrefs.GetInt(FlowManager.LOCKTIME);
            }
            if (i <= 30)
            {
                i = 30;
            }
            else if (i <= 60)
            {
                i = 60;
            }
            else if (i <= 120)
            {
                i = 120;
            }
            else if (i <= 300)
            {
                i = 300;
            }
            else
            {
                i = 1000;
            }
            switch (i)
            {
                case 30:
                    SelectBtn("1");
                    break;
                case 60:
                    SelectBtn("2");
                    break;
                case 120:
                    SelectBtn("3");
                    break;
                case 300:
                    SelectBtn("4");
                    break;
                case 1000:
                    SelectBtn("5");
                    break;
            }
        }

        void SelectBtn(string name)
        {
            foreach (var v in btnList)
            {
                if (v.gameObject.name == name)
                {
                    v.transform.Find("Image (1)").gameObject.SetActive(true);
                    v.interactable = false;
                    int i = 30;
                    switch (name)
                    {
                        case "1":
                            i = 30;
                            break;
                        case "2":
                            i = 60;
                            break;
                        case "3":
                            i = 120;
                            break;
                        case "4":
                            i = 300;
                            break;
                        case "5":
                            i = 1000;
                            break;
                    }
                    if (events != null)
                    {
                        events(i.ToString());
                    }
                }
                else
                {
                    v.transform.Find("Image (1)").gameObject.SetActive(false);
                    v.interactable = true;
                }
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
