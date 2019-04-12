using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class AddCoinViewAUI : BaseUI
    {
        public GameObject itemPrefab;
        public Button backBtn;
        public Transform anchor;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            itemPrefab = transform.Find("Scroll View/Viewport/Content/Item").gameObject;
            backBtn = transform.Find("buttom/BackBtn").GetComponent<Button>();
            anchor = transform.Find("Scroll View/Viewport/Content");
            backBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            itemPrefab.SetActive(false);
        }

        public override void Show(object param)
        {
            base.Show(param);
            List<WalletType> data = (List<WalletType>)param;
            List<WalletType> temp = new List<WalletType>();
            temp.Add(WalletType.BTC);
            temp.Add(WalletType.ETH);
            temp.Add(WalletType.EOS);
            foreach (var v in temp)
            {
                if (data.Contains(v))
                {
                    continue;
                }
                GameObject item = GameObject.Instantiate(itemPrefab, anchor);
                item.GetComponent<RectTransform>().localScale = Vector3.one;
                item.gameObject.SetActive(true);
                item.transform.Find("icon").GetComponent<Image>().sprite = FlowManager.Instance.coinIcon[(int)v];
                item.transform.Find("Text").GetComponent<Text>().text = FlowManager.Instance.coinDes[(int)v];
                //GameObject en = item.transform.Find("toggle").gameObject;
                //GameObject dis = item.transform.Find("toggle-dis").gameObject;
                WalletType type = v;
                item.transform.GetComponent<Button>().onClick.AddListener(() => {
                    if (!data.Contains(v))
                    {
                        FlowManager.Instance.AddViewWalletCoinType(v);
                        FlowManager.Instance.walletType = type;
                        if (events != null)
                        {
                            events("viewaddress");
                        }
                    }
                });
            }
        }

        public override void Hide()
        {
            base.Hide();
            List<Transform> temp = new List<Transform>();
            foreach (Transform v in anchor)
            {
                if (v.gameObject.activeSelf)
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
