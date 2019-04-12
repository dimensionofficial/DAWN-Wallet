using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;
using System.Threading;

namespace HardwareWallet
{
    public class BackupCheckUI : BaseUI
    {
        public Button nextBtn;
        public Button backBtn;
        public Transform gridUp;
        public Transform gridBackUp;
        public GameObject upitemPrefab;
        public Transform gridDown;
        public GameObject downItemPrefab;
        public Transform gridBackDown;
        public GameObject downBackItemPrefab;

        public string bip = "";

        public override void Ini()
        {
            base.Ini();
            listenBack = true;
            nextBtn = transform.Find("Next").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                string result = "";
                foreach (Transform v in gridUp)
                {
                    if (v.gameObject != upitemPrefab)
                    {
                        result = result + v.Find("Text").GetComponent<Text>().text + " ";
                    }
                }
                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                }
                if (result != bip)
                {
                    PopBox.Instance.ShowMsg(FlowManager.Instance.language.GetWord(Words.助记词错误));
                }
                else
                {
                    if (events != null)
                    {
                        events("nextBtn");
                    }
                }
            });
            nextBtn = transform.Find("BackBtn").GetComponent<Button>();
            nextBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });

            gridBackUp = transform.Find("upBack");

            gridUp = transform.Find("up");
            upitemPrefab = transform.Find("up/Item").gameObject;
            upitemPrefab.SetActive(false);

            gridDown = transform.Find("down");
            downItemPrefab = transform.Find("down/Item").gameObject;
            downItemPrefab.SetActive(false);

            gridBackDown = transform.Find("downBack");
            downBackItemPrefab = transform.Find("downBack/Item").gameObject;
            downBackItemPrefab.SetActive(false);
        }

        public override void Show(object param)
        {
            base.Show(param);
            gridDown.GetComponent<GridLayoutGroup>().enabled = true;
            bip = (string)param;
            int fontSize = 70;
            if (FlowManager.Instance.language.GetType() != typeof(SimpleChinese))
            {
                //bip = QRPayTools.ChangeBipLanguageToSpEnglish(bip);
                bip = QRPayTools.ChangeBipLanguageToSpNumber(bip);
                gridDown.GetComponent<GridLayoutGroup>().cellSize = new Vector2(256, 134);
                gridUp.GetComponent<GridLayoutGroup>().cellSize = new Vector2(256, 134);
                gridBackDown.GetComponent<GridLayoutGroup>().cellSize = new Vector2(256, 134);
                gridBackUp.GetComponent<GridLayoutGroup>().cellSize = new Vector2(256, 134);
                fontSize = 70;
            }
            else
            {
                bip = QRPayTools.ChangeBipLanguageToSpNumber(bip);
                gridDown.GetComponent<GridLayoutGroup>().cellSize = new Vector2(190, 190);
                gridUp.GetComponent<GridLayoutGroup>().cellSize = new Vector2(190, 190);
                gridBackDown.GetComponent<GridLayoutGroup>().cellSize = new Vector2(190, 190);
                gridBackUp.GetComponent<GridLayoutGroup>().cellSize = new Vector2(190, 190);
            }

            string[] words = bip.Split(' ');
            //乱序
            var randomWords = GetRandomWords(words);
            foreach (var v in randomWords)
            {
                GameObject o = GameObject.Instantiate(downBackItemPrefab, gridBackDown);
                o.transform.Find("Text").GetComponent<Text>().text = v;
                o.transform.Find("Text").GetComponent<Text>().fontSize = fontSize;
                o.SetActive(true);
            }
            foreach (var v in randomWords)
            {
                GameObject o = GameObject.Instantiate(downItemPrefab, gridDown);
                o.transform.Find("Text").GetComponent<Text>().text = v;
                o.transform.Find("Text").GetComponent<Text>().fontSize = fontSize;
                o.SetActive(true);
                o.GetComponent<Button>().onClick.AddListener(() => {
                    gridDown.GetComponent<GridLayoutGroup>().enabled = false;
                    GameObject u = GameObject.Instantiate(upitemPrefab, gridUp);
                    u.transform.Find("Text").GetComponent<Text>().text = v;
                    u.transform.Find("Text").GetComponent<Text>().fontSize = fontSize;
                    u.SetActive(true);
                    o.gameObject.SetActive(false);
                    u.GetComponent<Button>().onClick.AddListener(()=> {
                        o.gameObject.SetActive(true);
                        Destroy(u.gameObject);
                    });
                });
            }
            //gridDown.GetComponent<GridLayoutGroup>().enabled = false;
        }

        public override void Hide()
        {
            base.Hide();
            foreach (Transform v in gridUp)
            {
                if (v.gameObject != upitemPrefab)
                {
                    Destroy(v.gameObject);
                }
            }
            foreach (Transform v in gridDown)
            {
                if (v.gameObject != downItemPrefab)
                {
                    Destroy(v.gameObject);
                }
            }
            foreach (Transform v in gridBackDown)
            {
                if (v.gameObject != downBackItemPrefab)
                {
                    Destroy(v.gameObject);
                }
            }
            bip = "";
        }

        private List<string> GetRandomWords(string[] words)
        {
            List<string> temp = new List<string>();
            foreach (var v in words)
            {
                temp.Add(v);
            }
            List<string> result = new List<string>();
            do {
                int index = Random.Range(0, temp.Count);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            } while (result.Count < words.Length);

            return result;
        }
    }
}
