using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class MainPageAUI : BaseUI
    {
        public GameObject itemPrefab;
        public Button settingBtn;
        public Button addCoinViewBtn;
        public Button backUpViewBtn;
        public Button powerBtn;
        public Transform anchor;
        public GameObject topTips;
        public GameObject empty;
        public GameObject srollView;

        public override void Ini()
        {
            base.Ini();
            empty = transform.Find("Empty").gameObject;
            itemPrefab = transform.Find("Scroll View/Viewport/Content/Item").gameObject;
            settingBtn = transform.Find("buttom/Setting").GetComponent<Button>();
            addCoinViewBtn = transform.Find("buttom/AddCoin").GetComponent<Button>();
            backUpViewBtn = transform.Find("buttom/BackupBtn").GetComponent<Button>();
            powerBtn = transform.Find("buttom/Power").GetComponent<Button>();
            anchor = transform.Find("Scroll View/Viewport/Content");
            srollView = transform.Find("Scroll View").gameObject;
            topTips = transform.Find("TopTips").gameObject;

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
            addCoinViewBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("AddCoin");
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
            itemPrefab.SetActive(false);
        }

        public override void Show(object param)
        {
            base.Show(param);
            List<WalletType> data = (List<WalletType>)param;
            foreach (var v in data)
            {
                GameObject item = GameObject.Instantiate(itemPrefab, anchor);
                item.GetComponent<RectTransform>().localScale = Vector3.one;
                item.gameObject.SetActive(true);
                item.transform.SetAsFirstSibling();
                item.transform.Find("icon").GetComponent<Image>().sprite = FlowManager.Instance.coinIcon[(int)v];
                item.transform.Find("Text").GetComponent<Text>().text = FlowManager.Instance.coinDes[(int)v];
                item.GetComponent<Button>().onClick.AddListener(() => {
                    OnItemClick(v);
                });
            }
            empty.SetActive(data.Count <= 0);
            int top = FlowManager.Instance.IsBackUp() ? 100 : 280;
            topTips.SetActive(!FlowManager.Instance.IsBackUp());
            //backUpViewBtn.gameObject.SetActive(!FlowManager.Instance.IsBackUp());
            srollView.GetComponent<RectTransform>().offsetMax = 
                new Vector2(srollView.GetComponent<RectTransform>().offsetMax.x, -top);
        }

        public void OnItemClick(WalletType type)
        {
            if (events != null)
            {
                events("" + (int)type);
            }
        }

        public override void Hide()
        {
            base.Hide();
            List<Transform> temp = new List<Transform>();
            foreach (Transform v in anchor)
            {
                if (v.gameObject.activeSelf && v.gameObject.name != "AddCoin")
                {
                    temp.Add(v);
                }
            }
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                Destroy(temp[i].gameObject);
            }
        }
    }
}
