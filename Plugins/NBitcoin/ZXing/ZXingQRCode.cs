using ZXing;
using ZXing.QrCode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZXingQRCode : MonoBehaviour {
	
	[SerializeField] Image				qrCodeImage;
	[SerializeField] RawImage			qrCameraImage;
	[SerializeField] Text				qrAddress;
	[SerializeField] Text				qrBitcoin;

	[SerializeField] Button				qrClose;
	[SerializeField] Button				qrSend;
	[SerializeField] Button				qrReScan;

	WebCamTexture						webCamTexture;
	bool								isScanning;
	int									sendCoinAmount;
	Image								qrAddressBack;
	Image								qrBitcoinBack;

	void Awake()
	{
		qrClose.onClick.AddListener(OnClose);
		qrSend.onClick.AddListener(OnSend);
		qrReScan.onClick.AddListener(OnScan);
		qrAddressBack = qrAddress.transform.parent.GetComponent<Image>();
		qrBitcoinBack = qrBitcoin.transform.parent.GetComponent<Image>();
	}

	void Start()
	{
		//EncodeQRCode("Bitcoin:cUwC2Dk7VvVyxF3jGyHdz5HTtxHYqHuQgWX1pnYvqckwCyUGStd3");
		DecodeQRCode(1);
	}

	void OnClose()
	{
		if (webCamTexture!=null)
		{
			if(webCamTexture.isPlaying)
				webCamTexture.Stop();
			webCamTexture = null;
		}
		   
		gameObject.SetActive(false);
	}

	protected virtual void OnSend()
	{
		// todo : 
	}

	public void DecodeQRCode(int bitcoin)
	{
		gameObject.SetActive(true);

		qrCodeImage.gameObject.SetActive(false);
		qrCameraImage.gameObject.SetActive(false);
		qrAddress.gameObject.SetActive(false);

		qrClose.gameObject.SetActive(true);
		qrSend.gameObject.SetActive(true);
		qrReScan.gameObject.SetActive(true);

		qrSend.interactable		= false;
		qrReScan.interactable	= false;

		webCamTexture	= new WebCamTexture(Screen.width, Screen.height);
		sendCoinAmount	= bitcoin;

		OnScan();
	}

	void OnScan()
	{
		qrCodeImage.gameObject.SetActive(false);

		if (isScanning)
			StopCoroutine(Scanning());

		Color back = Color.white;
		back.a = 0.5f;

		qrAddress.text			= "Scaning";
		qrAddressBack.color		= back;
		qrBitcoin.text			= sendCoinAmount + " BTC";
		qrBitcoinBack.color		= back;

		if (webCamTexture != null) {
			webCamTexture.Play();
			qrCameraImage.texture		= webCamTexture;

			qrAddress.gameObject.SetActive(true);
			qrCameraImage.gameObject.SetActive(true);

            StartCoroutine(Scanning());
		}
	}

	IEnumerator Scanning()
	{
		isScanning				= true;
		qrSend.interactable		= false;
		qrReScan.interactable	= false;
		IBarcodeReader	iBR		= new BarcodeReader ();

		while (webCamTexture.width == 16)
			yield return null;

		var result				= iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
		int dot					= 0;

		qrCameraImage.rectTransform.sizeDelta			= new Vector2(webCamTexture.width, webCamTexture.height);
		qrCameraImage.rectTransform.localEulerAngles	= Vector3.forward * -90f;

		while (result == null)
		{
			qrAddress.text	= "Scaning" + new string('.', dot%8);
			if(dot%2==0)
				result = iBR.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
			dot++;
			yield return null;
		}

		qrAddress.text			= result.Text;
		qrAddressBack.color		= Color.white;
		qrBitcoinBack.color		= Color.white;

		isScanning				= false;
		qrSend.interactable		= true;
		qrReScan.interactable	= true;
        EncodeQRCode(result.Text, true);
	}

	public void EncodeQRCode(string code, bool isSendMode = false)
	{
		gameObject.SetActive(true);

		qrCodeImage.gameObject.SetActive(false);
		qrCameraImage.gameObject.SetActive(false);
		qrAddress.gameObject.SetActive(false);

		qrClose.gameObject.SetActive(true);
		qrSend.gameObject.SetActive(false);
		qrReScan.gameObject.SetActive(false);

		qrAddress.text	= code;
		qrBitcoin.text	= isSendMode?sendCoinAmount + " BTC" : "";

		var encoded	= new Texture2D(256, 256);
		var color32	= Encode(code, encoded.width, encoded.height);
		encoded.SetPixels32(color32);
		encoded.Apply();
		qrCodeImage.sprite	=  Sprite.Create(encoded, Rect.MinMaxRect(0,0,encoded.width,encoded.height), Vector2.one);

		qrCodeImage.gameObject.SetActive(true);
		qrCameraImage.gameObject.SetActive(isSendMode);
		qrAddress.gameObject.SetActive(true);

		qrClose.gameObject.SetActive(true);
		qrSend.gameObject.SetActive(isSendMode);
		qrReScan.gameObject.SetActive(isSendMode);
	}

	static Color32[] Encode(string textForEncoding, int width, int height)
	{
		var writer = new BarcodeWriter
		{
			Format	= BarcodeFormat.QR_CODE,
			Options	= new QrCodeEncodingOptions
			{
				Height	= height,
				Width	= width,
				Margin	= 0
			}
		};
		return writer.Write(textForEncoding);
	}
}