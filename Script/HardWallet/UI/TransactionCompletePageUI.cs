using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;

namespace HardwareWallet
{
    public class TransactionCompletePageUI : BaseUI
    {
        public Image qrImage;
        public Button completeBtn;
        public Button backQRBtn;
        public Button nextQRBtn;
        public Text qrindex;

        List<string> qrDataList = new List<string>();
        private int cur = 0;

        public override void Ini()
        {
            base.Ini();
            listenBack = true;
            smooth = true;
            qrImage = transform.Find("QR").GetComponent<Image>();
            qrindex = transform.Find("Count").GetComponent<Text>();
            completeBtn = transform.Find("CompleteBtn").GetComponent<Button>();
            completeBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("ok");
                }
            });
            nextQRBtn = transform.Find("NextQR").GetComponent<Button>();
            nextQRBtn.onClick.AddListener(() => {
                cur++;
                cur = Mathf.Clamp(cur, 0, qrDataList.Count - 1);
                ShowQR(cur);
            });
            backQRBtn = transform.Find("BackQR").GetComponent<Button>();
            backQRBtn.onClick.AddListener(() => {
                cur--;
                cur = Mathf.Clamp(cur, 0, qrDataList.Count - 1);
                ShowQR(cur);
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            //FlowManager.Instance.qRCodeManager.EncodeQRCode(qrImage, (string)param);
            cur = 0;
            qrDataList = CreateQR((string)param);
            ShowQR(cur);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ShowQR(int index)
        {
            if (qrDataList.Count <= 1)
            {
                nextQRBtn.gameObject.SetActive(false);
                backQRBtn.gameObject.SetActive(false);
                qrindex.text = "";
            }
            else if (index == qrDataList.Count - 1)
            {
                qrindex.text = (index + 1) + " / " + qrDataList.Count;
                nextQRBtn.gameObject.SetActive(true);
                backQRBtn.gameObject.SetActive(true);
                backQRBtn.interactable = true;
                nextQRBtn.interactable = false;
            }
            else if (index == 0)
            {
                qrindex.text = (index + 1) + " / " + qrDataList.Count;
                nextQRBtn.gameObject.SetActive(true);
                backQRBtn.gameObject.SetActive(true);
                backQRBtn.interactable = false;
                nextQRBtn.interactable = true;
            }
            else
            {
                qrindex.text = (index + 1) + " / " + qrDataList.Count;
                nextQRBtn.gameObject.SetActive(true);
                backQRBtn.gameObject.SetActive(true);
                backQRBtn.interactable = true;
                nextQRBtn.interactable = true;
            }
            FlowManager.Instance.qRCodeManager.EncodeQRCode(qrImage, qrDataList[index]);
        }

        public List<string> CreateQR(string data)
        {
            List<string> result = new List<string>();
            while (data.Length > 390)
            {
                string temp = data.Substring(0, 390);
                result.Add(temp);
                data = data.Substring(390);
            }
            result.Add(data);
            for (int i = 1; i <= result.Count; i++)
            {
                result[i - 1] += i.ToString("000") + "/" + result.Count.ToString("000");
            }
            return result;
        }
    }
}
