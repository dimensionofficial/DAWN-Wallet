using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class RecievCodePanel : BasePanel
{
    public Image qrCodeImage;
    public Text qrAddress;
    //private WebCamTexture webCamTexture;
   // bool isScanning;

    public void OnClickBack()
    {
        GameManager.Instance.OpenPanel(this, GameManager.Instance.recievSendPanel);
    }


    public override void Open()
    {
        base.Open();
        Init();
        GameManager.Instance.EncodeQRCode(qrCodeImage);
        GameManager.Instance.ShowCode(qrAddress);
    }

    //public void OnReScanBtn()
    //{
    //    Init();
    //    OnScan();
    //}

    private void Init()
    {
   //     qrCodeImage.gameObject.SetActive(false);
        qrAddress.text = "";
    }

    public override void OnHide()
    {
        base.OnHide();
        //if (webCamTexture != null)
        //{
        //    if (webCamTexture.isPlaying)
        //        webCamTexture.Stop();
        //    webCamTexture = null;
        //}
    }

    //void DecodeQRCode(int bitcoin)
    //{
    //    float width = qrCameraImage.GetComponent<RectTransform>().sizeDelta.x;
    //    float height = qrCameraImage.GetComponent<RectTransform>().sizeDelta.y;
    //    webCamTexture = new WebCamTexture((int)height, (int)width);
    //    OnScan();
    //}

    //void OnScan()
    //{
    //    if (isScanning)
    //        StopCoroutine(Scanning());

    //    qrAddress.text = "Scaning";

    //    if (webCamTexture != null)
    //    {
    //        webCamTexture.Play();
    //        qrCameraImage.texture = webCamTexture;

    //        qrAddress.gameObject.SetActive(true);
    //        qrCameraImage.gameObject.SetActive(true);

    //        StartCoroutine(Scanning());
    //    }
    //}

    //IEnumerator Scanning()
    //{
    //    isScanning = true;
    //    IBarcodeReader iBR = new BarcodeReader();

    //    while (webCamTexture.width == 16)
    //        yield return null;

    //    var result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
    //    int dot = 0;

    //    //qrCameraImage.rectTransform.sizeDelta = new Vector2(webCamTexture.width, webCamTexture.height);
    //    qrCameraImage.rectTransform.localEulerAngles = Vector3.forward * -90f;

    //    while (result == null)
    //    {
    //        qrAddress.text = "Scaning" + new string('.', dot % 8);
    //        if (dot % 2 == 0)
    //            result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
    //        dot++;
    //        yield return null;
    //    }

    //    qrAddress.text = result.Text;

    //    isScanning = false;
    //    qrCodeImage.gameObject.SetActive(true);
    //    qrCameraImage.gameObject.SetActive(false);
    //    GameManager.Instance.EncodeQRCode(qrCodeImage, result.Text);
    //}
}
