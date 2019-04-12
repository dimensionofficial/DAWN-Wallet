using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NBitcoin;
using ZXing;
using System.Threading;

namespace HardwareWallet
{
    public class ScanQRPageUI : BaseUI
    {
        public RawImage webCameraImage;
        public WebCamTexture webCamTexture;
        public Button cancelBtn;
        public Text arIndex;
        int dot = 0;
        Result result;
        string qrcode = "";
        string[] temp = null;

        public override void Ini()
        {
            base.Ini();
            smooth = true;
            listenBack = true;
            arIndex = transform.Find("Count").GetComponent<Text>();
            webCameraImage = transform.Find("QR").GetComponent<RawImage>();
            cancelBtn = transform.Find("Back").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() => {
                if (events != null)
                {
                    events("backBtn");
                }
            });
        }

        public override void Show(object param)
        {
            base.Show(param);
            webCamTexture = new WebCamTexture(Screen.width, Screen.height);
            webCameraImage.texture = webCamTexture;
            webCamTexture.Play();
            dot = 0;
            result = null;
            qrcode = "";
            arIndex.text = "";
            temp = null;
            StartCoroutine(Scanning());
        }

        public override void Hide()
        {
            StopAllCoroutines();
            base.Hide();
            if (webCamTexture != null)
            {
                if (webCamTexture.isPlaying)
                    webCamTexture.Stop();
                webCamTexture = null;
            }
        }

        IEnumerator TestScanning()
        {
            WWW www = new WWW("file://" + Application.dataPath + "/Test/ETH.txt");
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                string resultData = www.text;
                if (events != null)
                {
                    events(resultData);
                }
            }
            else
            {
                Debug.Log(www.error);
            }
        }

        IEnumerator Scanning()
        {
            IBarcodeReader iBR = new BarcodeReader();

            while (webCamTexture.width == 16)
                yield return null;

            result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);

            Vector2 scaleTo = new Vector2(webCamTexture.width, webCamTexture.height);
            float scale = 1440f / webCamTexture.width;
            scaleTo *= scale;
            if (scaleTo.y < 900)
            {
                scale = 900 / scaleTo.y;
                scaleTo *= scale;
            }
            webCameraImage.rectTransform.sizeDelta = scaleTo;
            webCameraImage.rectTransform.localEulerAngles = Vector3.forward * -90f;
            
#if UNITY_IPHONE
            webCameraImage.rectTransform.localScale = new Vector3(1, -1, 1);
#endif
            while (result == null)
            {
                if (dot % 2 == 0)
                {
                    Color32[] colors = webCamTexture.GetPixels32();
                    int w = webCamTexture.width;
                    int h = webCamTexture.height;
                    dot = 1;
                    ThreadPool.QueueUserWorkItem((o) => {
                        if (result == null)
                        {
                            result = iBR.Decode(colors, w, h);
                            dot = 0;
                        }
                    }, null);
                }
                yield return new WaitForFixedUpdate();
            }
            try
            {
                int cur = 0;
                int tol = 0;
                try
                {
                    string count = result.Text.Substring(result.Text.Length - 7, 7);
                    string[] num = count.Split('/');
                    cur = int.Parse(num[0]);
                    tol = int.Parse(num[1]);
                }
                catch
                {
                    StartCoroutine(Scanning());
                    throw new System.Exception("continue");
                }
                if (temp == null)
                {
                    temp = new string[tol];
                }
                if (temp.Length != tol)
                { 
                    throw new System.Exception("error qr");
                }
                else
                {
                    bool reScan = !string.IsNullOrEmpty(temp[cur - 1]);
                    temp[cur - 1] = result.Text.Substring(0, result.Text.Length - 7);
                    if (cur == tol)
                    {
                        string resultData = "";
                        foreach (var v in temp)
                        {
                            resultData += v;
                        }
                        if (events != null)
                        {
                            events(resultData);
                        }
                    }
                    else
                    {
                        PopBox.Instance.ShowMsg(cur + " / " + tol + FlowManager.Instance.language.GetWord(Words.扫描完成) + 
                            (reScan? FlowManager.Instance.language.GetWord(Words.此二维码已扫过) : 
                            FlowManager.Instance.language.GetWord(Words.请扫描下一张二维码)), 
                            ()=> {
                                StartCoroutine(Scanning());
                                arIndex.text = (cur + 1) + " / " + tol;
                            }, FlowManager.Instance.language.GetWord(Words.提示));
                    }
                }
            }
            catch (System.Exception e)
            {
                if (e.Message != "continue")
                {
                    if (events != null)
                    {
                        events("ERROR QR!");
                    }
                }
            }
        }
    }
}
