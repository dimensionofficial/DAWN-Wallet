using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;
using System.Threading;

namespace HardwareWallet
{
    public class BackupUI : BaseUI
    {
        public Button nextBtn;
        public Button backBtn;
        public Transform grid;
        public GameObject itemPrefab;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            nextBtn = transform.Find("Next").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("nextBtn");
                }
            });
            nextBtn = transform.Find("BackBtn").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
            grid = transform.Find("up");
            itemPrefab = transform.Find("up/Item").gameObject;
            itemPrefab.SetActive(false);
        }

        public override void Show(object param)
        {
            base.Show(param);
            string bip = (string)param;
            int fontSize = 70;
            if (FlowManager.Instance.language.GetType() != typeof(SimpleChinese))
            {
                //bip = QRPayTools.ChangeBipLanguageToSpEnglish(bip);
                bip = QRPayTools.ChangeBipLanguageToSpNumber(bip);
                grid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(270, 150);
                fontSize = 70;
            }
            else
            {
                bip = QRPayTools.ChangeBipLanguageToSpNumber(bip);
                grid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(200, 200);
            }
            string[] words = bip.Split(' ');
            foreach (var v in words)
            {
                GameObject o = GameObject.Instantiate(itemPrefab, grid);
                o.transform.Find("Text").GetComponent<Text>().text = v;
                o.transform.Find("Text").GetComponent<Text>().fontSize = fontSize;
                o.SetActive(true);
            }
        }

        public override void Hide()
        {
            base.Hide();
            foreach (Transform v in grid)
            {
                if (v.gameObject != itemPrefab)
                {
                    Destroy(v.gameObject);
                }
            }
        }
    }
}
