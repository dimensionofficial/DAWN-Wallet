using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HardwareWallet
{
    public class HandCodeUI : MonoBehaviour {

        public static HandCodeUI Instance;
        public MoveOverListener[] items = new MoveOverListener[9];
        public Text title1;
        public Text title2;
        public Button backBtn;
        private string currentSelect = "";
        System.Action<string> onFinished;
        System.Action onBack;
        bool isBegin = false;
        float title2Pos = 0;
        void Awake()
        {
            Instance = this;
            Transform back = transform.Find("bac");
            for (int i = 1; i <= 9; i++)
            {
                items[i - 1] = back.Find(i.ToString()).GetComponent<MoveOverListener>();
            }
            foreach (var v in items)
            {
                v.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                v.OnMoveEnter += OnMoveOverListener;
            }
            backBtn.onClick.AddListener(() => {
                Hide();
                if (onBack != null)
                {
                    onBack();
                }
            });
            Hide();
            title2Pos = title2.GetComponent<RectTransform>().anchoredPosition.x;
        }

        void OnMoveOverListener(MoveOverListener o)
        {
            if (FlowManager.Instance.GetHandLockSec() > 0)
            {
                return;
            }
            if(!currentSelect.Contains(o.gameObject.name))
            {
                //判断是否相临
                if (currentSelect.Length > 0)
                {
                    string last = currentSelect[currentSelect.Length - 1].ToString();
                    if (!CheckIsAlign(last, o.gameObject.name))
                    {
                        return;
                    }
                }
                this.title2.text = "";
                o.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                currentSelect += o.gameObject.name;
                UpdateLines();
            }
        }

        public void Show(System.Action<string> onFinished, string title, string info = "", System.Action onBack = null)
        {
            this.onBack = onBack;
            backBtn.gameObject.SetActive(onBack != null);
            currentSelect = "";
            foreach (var v in items)
            {
                v.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
            }
            UpdateLines();
            isBegin = false;
            this.onFinished = onFinished;
            this.title1.text = title;
            this.gameObject.SetActive(true);
            if (FlowManager.Instance.GetHandLockSec() > 0)
            {
                this.title2.text = "";
                return;
            }
            if (!string.IsNullOrEmpty(info))
            {
                this.title2.text = info;
            }
            else
            {
                this.title2.text = "";
            }
            if (!string.IsNullOrEmpty(info))
            {
                StartCoroutine(ShakInfo());
            }
        }

        public void Hide()
        {
            foreach (var v in items)
            {
                v.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            this.gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (onBack != null)
                {
                    Hide();
                    onBack();
                }
            }
            if (FlowManager.Instance.GetHandLockSec() > 0)
            {
                this.title2.text = FlowManager.Instance.language.GetWord(Words.设备停用).Replace("{0}",
                    FlowManager.Instance.GetHandLockSec().ToString());
                return;
            }
            else if (this.title2.text.StartsWith(FlowManager.Instance.language.GetWord(Words.设备停用).Substring(0, 4)))
            {
                this.title2.text = "";
            }
            if (!isBegin && (
                (Input.touchCount > 0 && Application.platform == RuntimePlatform.Android) || 
                (Input.GetMouseButtonDown(0) && Application.platform == RuntimePlatform.WindowsEditor)))
            {
                isBegin = true;
            }
            if (isBegin && (
                (Input.touchCount <= 0 && Application.platform == RuntimePlatform.Android) ||
                (Input.GetMouseButtonUp(0) && Application.platform == RuntimePlatform.WindowsEditor)))
            {
                isBegin = false;
                if (currentSelect.Length >= 4)
                {
                    Hide();
                    if (onFinished != null)
                    {
                        onFinished(currentSelect);
                    }
                }
                else if(currentSelect.Length >= 1)
                {
                    Hide();
                    Handheld.Vibrate();
                    this.title2.text = FlowManager.Instance.language.GetWord(Words.至少经过四个点);
                    currentSelect = "";
                    UpdateLines();
                    isBegin = false;
                    this.gameObject.SetActive(true);
                    StartCoroutine(ShakInfo());
                }
            }

        }

        public IEnumerator ShakInfo()
        {
            int i = 0;
            while (i < 10)
            {
                float offset = i % 2 == 0 ? -10 : 10;
                title2.GetComponent<RectTransform>().anchoredPosition = new Vector2(title2Pos + offset,
                    title2.GetComponent<RectTransform>().anchoredPosition.y);
                yield return new WaitForSeconds(0.05f);
                i++;
            }
            title2.GetComponent<RectTransform>().anchoredPosition = new Vector2(title2Pos,
                   title2.GetComponent<RectTransform>().anchoredPosition.y);
        }

        void UpdateLines()
        {
            foreach (var v in items)
            {
                foreach (Transform t in v.transform)
                {
                    t.gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < currentSelect.Length - 1; i++)
            {
                if (currentSelect.Length > i + 1)
                {
                    int line = GetLineIndex(currentSelect[i].ToString(), currentSelect[i + 1].ToString());
                    int index = int.Parse(currentSelect[i].ToString()) - 1;
                    items[index].transform.Find(line.ToString()).gameObject.SetActive(true);
                }
            }
        }

        bool CheckIsAlign(string name1, string name2)
        {
            int index1 = int.Parse(name1);
            int index2 = int.Parse(name2);
            int row1 = (index1 - 1) / 3 + 1;
            int row2 = (index2 - 1) / 3 + 1;
            int line1 = (index1 - 1) % 3 + 1;
            int line2 = (index2 - 1) % 3 + 1;
            return (Mathf.Abs(row1 - row2) <= 1) && (Mathf.Abs(line1 - line2) <= 1);
        }

        /// <summary>
        /// 获取连接线id
        /// </summary>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <returns></returns>
        int GetLineIndex(string name1, string name2)
        {
            int index1 = int.Parse(name1);
            int index2 = int.Parse(name2);
            int row1 = (index1 - 1) / 3 + 1;
            int row2 = (index2 - 1) / 3 + 1;
            int line1 = (index1 - 1) % 3 + 1;
            int line2 = (index2 - 1) % 3 + 1;
            //左中右
            int lmr = 0;//0左，1中，2右
            if (line1 < line2)
            {
                lmr = 2;
            }
            else if (line1 > line2)
            {
                lmr = 0;
            }
            else
            {
                lmr = 1;
            }
            //上中下
            int umd = 0;//0上，1中，2下
            if (row1 > row2)
            {
                umd = 0;
            }
            else if (row1 < row2)
            {
                umd = 2;
            }
            else
            {
                umd = 1;
            }
            /////////////////////////////////////////
            if (umd == 0 && lmr == 0)
            {
                return 8;
            }
            else if (umd == 0 && lmr == 1)
            {
                return 1;
            }
            else if (umd == 0 && lmr == 2)
            {
                return 2;
            }
            else if (umd == 1 && lmr == 0)
            {
                return 7;
            }
            else if (umd == 1 && lmr == 2)
            {
                return 3;
            }
            else if (umd == 2 && lmr == 0)
            {
                return 6;
            }
            else if (umd == 2 && lmr == 1)
            {
                return 5;
            }
            else if (umd == 2 && lmr == 2)
            {
                return 4;
            }
            return 0;
        }
    }
}
