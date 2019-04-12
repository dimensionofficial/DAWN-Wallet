using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class OpenCameraScan : MonoBehaviour {

    public RawImage qrCameraImage;
    public WebCamTexture webCamTexture;
    bool isScanning;
    public ScanBar m_bar;
    Result result = null;
    public Action<string> ScanEndBack;
    public Action OnCloseScan;

    public void OpenMe(Action<string> _onScanEnd, Action _onCloseScan)
    {
        gameObject.SetActive(true);
        OnScan();
        ScanEndBack = _onScanEnd;
        OnCloseScan = _onCloseScan;
    }

    public void OnClosedMe()
    {
        gameObject.SetActive(false);
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
                webCamTexture.Stop();
            webCamTexture = null;
        }

        if (OnCloseScan != null)
        {
            OnCloseScan();
            OnCloseScan = null;
        }
    }

    void OnScan()
    {

        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture(Screen.width, Screen.height);
        }


        if (isScanning)
            StopCoroutine(Scanning());

        if (webCamTexture != null)
        {
            webCamTexture.Play();
            qrCameraImage.texture = webCamTexture;

            qrCameraImage.gameObject.SetActive(true);

            StartCoroutine(Scanning());
        }
    }

    IEnumerator Scanning()
    {
        m_bar.StartDo();
        isScanning = true;
        IBarcodeReader iBR = new BarcodeReader();

        while (webCamTexture.width == 16)
            yield return null;

        result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
        int dot = 0;

        float height = 1600F;
        float width = 900F;

        if (Camera.main.pixelHeight > height)
            height = Camera.main.pixelHeight;
        if (Camera.main.pixelWidth > width)
            width = Camera.main.pixelWidth;

        Vector2 scaleTo = new Vector2(webCamTexture.width, webCamTexture.height);
        float scale = height * 1.0F / webCamTexture.width;
        scaleTo *= scale;
        if (scaleTo.y < width)
        {
            scale = width / scaleTo.y;
            scaleTo *= scale;
        }
        qrCameraImage.rectTransform.sizeDelta = scaleTo;
        qrCameraImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaleTo.x);
        qrCameraImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scaleTo.y);
#if UNITY_ANDROID   
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0,0, -90);
#endif
#if UNITY_IPHONE
        qrCameraImage.rectTransform.localEulerAngles = new Vector3(0, 0, 90);
        qrCameraImage.rectTransform.localScale = new Vector3(-1, 1, 1);
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
                    result = iBR.Decode(colors, w, h);
                    dot = 0;
                }, null);
            }
            yield return new WaitForFixedUpdate();
        }

        isScanning = false;

        OnClosedMe();

        if (ScanEndBack != null)
        {
            ScanEndBack(result.Text);
            ScanEndBack = null;
        }
    }
}
