using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class AddCoinViewUI : BaseUI
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
            backBtn = transform.Find("BackBtn").GetComponent<Button>();
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
                GameObject item = GameObject.Instantiate(itemPrefab, anchor);
                item.GetComponent<RectTransform>().localScale = Vector3.one;
                item.gameObject.SetActive(true);
                item.transform.Find("icon").GetComponent<Image>().sprite = FlowManager.Instance.coinIcon[(int)v];
                item.transform.Find("Text").GetComponent<Text>().text = FlowManager.Instance.coinDes[(int)v];
                GameObject en = item.transform.Find("toggle").gameObject;
                GameObject dis = item.transform.Find("toggle-dis").gameObject;
                if (data.Contains(v))
                {
                    en.SetActive(true);
                    dis.SetActive(false);
                }
                else
                {
                    en.SetActive(false);
                    dis.SetActive(true);
                }
                item.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => {
                    if (data.Contains(v))
                    {
                        FlowManager.Instance.DeleteViewWalletCoinType(v);
                        en.SetActive(false);
                        dis.SetActive(true);
                    }
                    else
                    {
                        FlowManager.Instance.AddViewWalletCoinType(v);
                        en.SetActive(true);
                        dis.SetActive(false);
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
